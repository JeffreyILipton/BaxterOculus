#!/usr/bin/env python
from demo_base import *

import os # path.expanduser, path.exists, makedirs

# STILL UNDER CONSTRUCTION

class GraspDatabase(DemoBase):
    def __init__(self):
        DemoBase.__init__(self)

        self._obj_name = rospy.get_param('~object_name', 'new_object')
        self._dataset_path = rospy.get_param('~dataset_path', '~/dataset/grasping/'+self._obj_name+'/')
        self._dataset_path = os.path.expanduser(self._dataset_path) # in case path is under /home directory

        if not os.path.exists(self._dataset_path):
            os.makedirs(self._dataset_path)

        # service proxies
        rospy.loginfo("wait for service: save_cloud")
        rospy.wait_for_service("obj_segmentor/save_cloud")
        self._srv_savecloud = rospy.ServiceProxy("obj_segmentor/save_cloud", SaveCloud)

    def _get_offset(self, x0, x1, x2):
        # http://mathworld.wolfram.com/Point-LineDistance3-Dimensional.html
        t = -np.dot((x1-x0), (x2-x1))/(np.linalg.norm(x2-x1)**2)
        v = x1 + (x2-x1)*t
        return v-x0

    def calculate_azelof(self, cloud, center, pose):
        # calcualte azimuth, elevation, and offset from center
        # https://en.wikipedia.org/wiki/Azimuth
        # TODO: test these
        T = pm.toMatrix(pm.fromMsg(pose))
        u = T[:3, 2] # unit vector of z-axis
        x, y, z = u[0], u[1], u[2]
        az = np.arctan2(y, x)
        if az < 0.:
            az += 2.*np.pi

        assert az >= np.pi/2. and az <= np.pi*3./2.
        az -= np.pi
        assert az >= -np.pi/2. and az <= np.pi/2.

        el = np.arcsin(z)

        # offset
        x1 = T[:3, 3] # origin in end-effector frame
        x2 = T[:3, 3] + T[:3, 2] # unit vector in z-axis
        x0 = np.array([center.x, center.y, center.z])

        # appr_dir = self._get_ad(T)
        offset = self._get_offset(x0, x1, x2)

        return 0, 0, [0, 0, 0]

    def save_data(self, cloud, center, pose):
        fn_common = self._dataset_path + self._obj_name + '_' + str(time.time())
        fn_info = fn_common + '.npz'
        fn_cloud = fn_common + '.pcd'

        # save to pcd file via service
        req = SaveCloudRequest()
        req.cloud = cloud
        req.obj_center = center
        req.filename.data = fn_cloud

        try:
            resp = self._srv_savecloud(req)
        except rospy.ServiceException, e:
            print "Service call failed: %s" % e

        ip.embed()

        # calculate azimuth & elevation & offset
        az, el, offset = self.calculate_azelof(cloud, center, pose)

        # save pose
        np.savez_compressed(fn_info, az=az, el=el, offset=offset)


    def run(self):
        print "starting run"

        # make sure hand is open
        self.hand.LeftOpen()
        self.hand.RightOpen()

        # move to position to spot objects
        success = self.arm.MoveToSpotLoc()

        if not success:
            print "Error moving to spot location!"
            return False

        # wait until we spot an object
        while True:
            print "waiting for object to be detected"

            # sorted left to right wrt baxter
            (obj_centers, obj_heights, obj_radius, cloud, cloud_segs) = self.find_obj()

            
            if len(obj_centers) == 1 and len(obj_heights) == 1:
                # let human user teach robot grasping
                while raw_input('press n key after put hand to the object') != 'n':
                    pass

                # get grasp pose (end-effector pose w.r.t. object)
                pose = self.arm.GetLeftHandPose()

                # save point cloud, grasp pose
                self.save_data(cloud_segs[0], obj_centers[0], pose)
                return False
            else:
                print('there are more than one segment')
                return False


def main(args):
    rospy.init_node('add_grasping_datset', anonymous=True)
    gd = GraspDatabase()
    shutdown = False

    try:
        while(not shutdown):
            no_error = gd.run()
            raw_input('##### press any key to proceed #####')
    except KeyboardInterrupt:
        print "auto grasp Shutting down"

if __name__ == '__main__':
    main(sys.argv)
