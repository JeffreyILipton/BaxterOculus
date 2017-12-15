#!/usr/bin/env python
from demo_base import *
import IPython as ip
from cnn3d.utils.grid_manip import tf_grid
import cnn3d.viz as viz
from baxter_soft_hand.softhand.SoftHand import SoftHand, HandType
from cnn3d_grasping.srv import Start, StartResponse, Stop, StopResponse, GetThreshold, GetThresholdResponse, SetThreshold, SetThresholdResponse, NeedHelp, NeedHelpRequest
from geometry_msgs.msg import Point, Pose, Quaternion
import os # os.path.join
from std_msgs.msg import Float32

class Grasping(DemoBase):
    def __init__(self):
        DemoBase.__init__(self, ht=HandType.hard)
        self._top_only = True
        # self._top_only = False

        self._obj_selection_mode = 0 # leftmost

        self._p_th = 0.5

        # this is for quick evaluation
        # it does not go to the bin, just grasp and lift and put it back
        # should be 'False' in general
        # self.fast_eval_ = True
        self.fast_eval_ = False

        self._num_samples = 10

        self._run_flag = False

        s = rospy.Service('~start', Start, self.srv_start)
        s = rospy.Service('~stop', Stop, self.srv_stop)
        s = rospy.Service('~get_th', GetThreshold, self.srv_get_th)
        s = rospy.Service('~set_th', SetThreshold, self.srv_set_th)

        self._pub_conf = rospy.Publisher('~confidence', Float32, queue_size=1)
        self._conf = 0.0
        self._grasp_trial_counter = 0

        self._new_data_dir_name = 'new_grasp_data'
        if not os.path.exists(self._new_data_dir_name):
            os.makedirs(self._new_data_dir_name)
        self._help_srv_name = '/test_srv/grasp_help'

    def srv_start(self, req):
        rospy.loginfo('srv_start')
        self._run_flag = True
        resp = StartResponse()
        return resp

    def srv_stop(self, req):
        rospy.loginfo('srv_stop')
        self._run_flag = False
        resp = StopResponse()
        return resp

    def srv_get_th(self, req):
        rospy.loginfo('srv_get_th')
        resp = GetThresholdResponse()
        resp.th = self._p_th
        return resp

    def srv_set_th(self, req):
        rospy.loginfo('srv_set_th')
        self._p_th = req.th
        resp = SetThresholdResponse()
        return resp

    def dist_orientation(self, p1, p2):
        m1 = pm.toMatrix(pm.fromMsg(p1))
        m2 = pm.toMatrix(pm.fromMsg(p2))
        r1 = m1[:3, :3]
        r2 = m2[:3, :3]
        r1tr2 = np.dot(r1.transpose(), r2)
        # theta = acos((Tr(R)-1)/2)
        return np.arccos((np.trace(r1tr2)-1)/2)

    def pose2label(self, pose):
        # 5: --, 6: /, 7: |, 8: \
        # obtained via 'self.arm.GetLeftHandPose().orientation'
        canonical_poses = [Pose(orientation=Quaternion(x=0.723746861786, y=0.689847737773, z=0.00714593609215, w=-0.0157960230006)), 
            Pose(orientation=Quaternion(x=0.710284661022, y=-0.702817993763, z=-0.03458999993, w=-0.0186037594072)), 
            Pose(orientation=Quaternion(x=0.376531643148, y=0.925715989406, z=0.029478728631, w=-0.0201204677792)), 
            Pose(orientation=Quaternion(x=0.91798789321, y=-0.396580317129, z=-0.00388747640715, w=0.00267722096921)), 
            Pose(orientation=Quaternion(x=0.999176142385, y=0.0327934115187, z=-0.0198606803395, w=0.0133109738892)), 
            Pose(orientation=Quaternion(x=-0.0108028572931, y=0.999847404784, z=0.0131965521556, w=-0.00378370608193)), 
            Pose(orientation=Quaternion(x=-0.368017894787, y=0.92972719378, z=0.0130444482728, w=-0.000128964270903)), 
            Pose(orientation=Quaternion(x=0.948413558196, y=0.315592999437, z=0.0111310369426, w=-0.0280870317418))]

        idx2label = [5, 5, 6, 6, 7, 7, 8, 8]
        dist = [0.0] * len(canonical_poses)
        for i, p in enumerate(canonical_poses):
            dist[i] = self.dist_orientation(p, pose)

        i_min = np.argmin(dist)
        return [idx2label[i_min]]

    def call_help(self):
        try:
            rospy.wait_for_service(self._help_srv_name)
            srv = rospy.ServiceProxy(self._help_srv_name, NeedHelp)
            req = NeedHelpRequest()
            req.conf = self._conf
            resp = srv(req)
        except rospy.ServiceException, e:
            rospy.logerr("Service call failed: {}".format(e))

        p = resp.pose
        if p.position == Point() and p.orientation == Quaternion():
            rospy.logwarn('empty pose, do not update')
            return

        # gt label from from req.pose
        gt = self.pose2label(p)

        print(p)
        print(gt)

        # save self._grid and gt into npz file
        fn = os.path.join(self._new_data_dir_name, str(time.time()) + '.npz')
        np.savez_compressed(fn, g=self._grid, gt=gt)

        # stop execution
        self._run_flag = False


    def _get_prob_from_vg(self, grid):
        # grid: 1D tuple
        req = DetectGraspingRequest()
        req.grid = grid

        try:
            resp = self._srv_detgrasping(req)
        except rospy.ServiceException, e:
            print "Service call failed: %s" % e

        p = resp.grasp_dir
        return p

    def get_approaching_direction(self, grid):
        p = self._get_prob_from_vg(grid)
        # p[4:] has probability of approaching directions
        return [self.argnmax(p[4:])]

    def get_approaching_direction_all(self, grid, p_th=0.5):
        # return all directions sorted in decreasing order of probability
        p = self._get_prob_from_vg(grid)
        # p[4:] has probability of approaching directions
        p = p[4:]

        print('appr: ')
        print(p)

        sorted_idx = sorted(range(len(p)), key=lambda k: p[k])[::-1]
        return [i for i in sorted_idx if p[i] > p_th]

    def get_wrist_orientation(self, grid, p_th=0.5):
        p = self._get_prob_from_vg(grid)
        idx_wrist_ori = self.argnmax(p[:4])
        if p[idx_wrist_ori] > p_th:
            return [idx_wrist_ori], p[idx_wrist_ori]
        else:
            return [], p[idx_wrist_ori]

    def get_wrist_orientation_all(self, grid, p_th=0.5):
        p = self._get_prob_from_vg(grid)
        # p[:4] has probability of wrist orientation
        p = p[:4]

        print('wrist: ')
        print(p)

        sorted_idx = sorted(range(len(p)), key=lambda k: p[k])[::-1]
        return [i for i in sorted_idx if p[i] > p_th]

    def get_grasping_indices_all(self, cloud_seg, obj_center, obj_height, p_th=0.5):
        # get all good hypothesis over threshold values
        grid = self.get_occugrid(cloud_seg, obj_center, obj_height)

        # check if grid is empty or not
        if np.max(grid) == -1.0:
            print('gird is empty')
            return -1, -1

        # save grid for learning
        self._grid = np.array(grid).reshape((32, 32, 32)).astype(np.float32)

        # get approaching direction
        indices_app_dir = self.get_approaching_direction_all(grid, p_th=p_th)

        pair_app_dir_wrist_ori = []

        for idx_app_dir in indices_app_dir:
            idx2name = ['top', 'front', 'left', 'right', 'front_left', 'front_right']
            # transform grid according to approaching direction
            g = np.array(grid)
            g = g.reshape((32, 32, 32))
            g = tf_grid(g, app_dir=idx2name[idx_app_dir])
            grid = tuple(g.ravel())

            indices_wrist_ori = self.get_wrist_orientation_all(grid, p_th=p_th)

            for idx_wrist_ori in indices_wrist_ori:
                pair_app_dir_wrist_ori.append((idx_app_dir, idx_wrist_ori))
        
        return pair_app_dir_wrist_ori

    def get_grasping_indices(self, cloud_seg, obj_center, obj_height, top_only=False, p_th=0.5):
        # get only one best hypothesis
        grid = self.get_occugrid(cloud_seg, obj_center, obj_height)

        # check if grid is empty or not
        if np.max(grid) == -1.0:
            print('gird is empty')
            return -1, -1

        # save grid for learning
        self._grid = np.array(grid).reshape((32, 32, 32)).astype(np.float32)

        # get approaching direction
        if top_only:
            indices_app_dir = [0] # always top
        else:
            indices_app_dir = self.get_approaching_direction(grid)

        pair_app_dir_wrist_ori = []

        for idx_app_dir in indices_app_dir:
            idx2name = ['top', 'front', 'left', 'right', 'front_left', 'front_right']
            # transform grid according to approaching direction
            g = np.array(grid)
            g = g.reshape((32, 32, 32))
            g = tf_grid(g, app_dir=idx2name[idx_app_dir])
            grid = tuple(g.ravel())

            indices_wrist_ori, conf = self.get_wrist_orientation(grid, p_th)

            # publish confidence
            self._conf = conf
            self._pub_conf.publish(Float32(conf))

            for idx_wrist_ori in indices_wrist_ori:
                pair_app_dir_wrist_ori.append((idx_app_dir, idx_wrist_ori))
        
        return pair_app_dir_wrist_ori

    def run(self):
        if not self._run_flag:
            time.sleep(5)
            rospy.loginfo('waiting for start command')
            return True

        # make sure hand is open
        self.hand.LeftOpen()
        self.hand.RightOpen()

        # move to position to spot objects
        success = self.arm.MoveToSpotLoc()

        print "waiting for object to be detected"
        # sorted left to right wrt baxter
        (obj_centers, obj_heights, obj_radius, cloud, cloud_segs) = self.find_obj()
        
        if len(obj_centers) == 0 or len(obj_heights) == 0:
            print('there is no object on the table.')
            time.sleep(5)
            return True
        else:
            if self.use_left_arm:
                if self._obj_selection_mode == 0:
                    (c_l, h_l, r_l, cloud_seg) = (obj_centers[0], obj_heights[0], obj_radius[0], cloud_segs[0]) # leftmost
                elif self._obj_selection_mode == 1:
                    i = np.random.randint(len(obj_centers))
                    (c_l, h_l, r_l, cloud_seg) = (obj_centers[i], obj_heights[i], obj_radius[i], cloud_segs[i]) # random

                # pair_app_dir_wrist_ori = self.get_grasping_indices_all(cloud_seg, c_l, h_l, self._p_th)
                pair_app_dir_wrist_ori = self.get_grasping_indices(cloud_seg, c_l, h_l, top_only=self._top_only, p_th=self._p_th)

                print(pair_app_dir_wrist_ori)
                
                for pair in pair_app_dir_wrist_ori:
                    idx_app_dir = pair[0]
                    idx_wrist_ori = pair[1]
                    print('idx_app_dir: %d' % idx_app_dir)
                    print('idx_wrist_ori: %d' % idx_wrist_ori)

                    if idx_app_dir == -1 or idx_wrist_ori == -1:
                        print('empty occupancy grid')
                        return True
                    
                    (success, pose, pose_appr, arm_group) = self.grasp(c_l, h_l, r_l, idx_app_dir, idx_wrist_ori)

                    if success:
                        self.hand.LeftClose()
                        time.sleep(0.5)

                        if idx_app_dir > 0:
                            pose_appr.position.z += 0.15 # TODO: tune this value

                        success = self.arm.MoveTo(pose_appr, arm_group)

                        # TODO: add counter and try multiple times
                        if not self.hand.LeftIsGrasped():
                            rospy.logwarn('could not grasped the object!')
                            self._grasp_trial_counter += 1
                            # after two unsuccessful trials, call human's help
                            if self._grasp_trial_counter >= 2:
                                self._grasp_trial_counter = 0
                                self.call_help()
                            
                            return True

                        if self.fast_eval_:
                            success = self.arm.MoveTo(pose, arm_group)
                            self.hand.LeftOpen()
                            time.sleep(0.5)
                        else:
                            self.arm.MoveToBin('left', appr=True)
                            
                            self.arm.MoveToBin('left', appr=False)

                            self.hand.LeftOpen()
                            time.sleep(0.5)

                            self.arm.MoveToBin('left', appr=True)

                        # move to position to spot objects
                        success = self.arm.MoveToSpotLoc()
                        return True
                    else:
                        print "Error in motion!"
                        return True
                # there is no valid poses
                # call help
                rospy.logwarn('not confident')
                self.call_help()
                return True


def main(args):
    rospy.init_node('grasping_demo_homunculus', anonymous=True)
    g = Grasping()
    run_flag = True
    try:
        while not rospy.is_shutdown() and run_flag:
            run_flag = g.run()
    except KeyboardInterrupt:
        print "grasping_demo Shutting down"

if __name__ == '__main__':
    main(sys.argv)
