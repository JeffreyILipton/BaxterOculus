#!/usr/bin/env python
from demo_base import *
import IPython as ip
from cnn3d.utils.grid_manip import tf_grid
import cnn3d.viz as viz
import sys

class Grasping(DemoBase):
    def __init__(self):
        DemoBase.__init__(self)
        # self._top_only = True
        self._top_only = False

        self._avoid_top = True

        self._obj_selection_mode = 0 # leftmost

        # self._p_th = 0.5
        self._p_th = 0.01

        # this is for quick evaluation
        # it does not go to the bin, just grasp and lift and put it back
        # should be 'False' in general
        self.fast_eval_ = True
        # self.fast_eval_ = False

        self._num_samples = 10

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

        idx_p = [(i, j) for i, j in zip(range(len(p)), p)]
        idx_p_sorted = sorted(idx_p, key=lambda tup: tup[1])[::-1]
        return [i for i in idx_p_sorted if i[1] > p_th]

        # sorted_idx = sorted(range(len(p)), key=lambda k: p[k])[::-1]
        # return [i for i in sorted_idx if p[i] > p_th]

    def get_wrist_orientation(self, grid):
        p = self._get_prob_from_vg(grid)
        idx_wrist_ori = self.argnmax(p[:4])
        return [idx_wrist_ori]

    def get_wrist_orientation_all(self, grid, p_th=0.5):
        p = self._get_prob_from_vg(grid)
        # p[:4] has probability of wrist orientation
        p = p[:4]

        print('wrist: ')
        print(p)

        idx_p = [(i, j) for i, j in zip(range(len(p)), p)]
        idx_p_sorted = sorted(idx_p, key=lambda tup: tup[1])[::-1]
        return [i for i in idx_p_sorted if i[1] > p_th]

        # sorted_idx = sorted(range(len(p)), key=lambda k: p[k])[::-1]
        # return [i for i in sorted_idx if p[i] > p_th]

    def get_grasping_indices_all(self, cloud_seg, obj_center, obj_height, p_th=0.5):
        # get all good hypothesis over threshold values
        grid = self.get_occugrid(cloud_seg, obj_center, obj_height)

        # check if grid is empty or now
        if np.max(grid) == -1.0:
            print('gird is empty')
            return -1, -1

        # get approaching direction
        indices_app_dir = self.get_approaching_direction_all(grid, p_th=p_th)

        pair_app_dir_wrist_ori = []

        for idx_app_dir, p_app_dir in indices_app_dir:
            idx2name = ['top', 'front', 'left', 'right', 'front_left', 'front_right']
            # transform grid according to approaching direction
            g = np.array(grid)
            g = g.reshape((32, 32, 32))
            g = tf_grid(g, app_dir=idx2name[idx_app_dir])
            grid = tuple(g.ravel())

            indices_wrist_ori = self.get_wrist_orientation_all(grid, p_th=p_th)

            for idx_wrist_ori, p_wrist_ori in indices_wrist_ori:
                p = p_app_dir*p_wrist_ori
                if p > p_th:
                    pair_app_dir_wrist_ori.append((idx_app_dir, idx_wrist_ori, p))
        
        # ip.embed()

        if len(pair_app_dir_wrist_ori) > 1:
            # sort
            pair_app_dir_wrist_ori = sorted(pair_app_dir_wrist_ori, key=lambda tup: tup[2])[::-1]
        return pair_app_dir_wrist_ori

    def get_grasping_indices(self, cloud_seg, obj_center, obj_height, top_only=False, p_th=0.5):
        # get only one best hypothesis
        grid = self.get_occugrid(cloud_seg, obj_center, obj_height)

        # check if grid is empty or now
        if np.max(grid) == -1.0:
            print('gird is empty')
            return -1, -1

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

            indices_wrist_ori = self.get_wrist_orientation(grid)

            for idx_wrist_ori in indices_wrist_ori:
                pair_app_dir_wrist_ori.append((idx_app_dir, idx_wrist_ori))
        
        return pair_app_dir_wrist_ori

    def get_grasping_indices_clutter(self, cloud, grasp_points, p_th=0.5, top_only=True):
        
        pair_app_dir_wrist_ori = []

        for pt in grasp_points:
            grid = self.get_occugrid(cloud, pt, -1.0) # -1.0 height: automatically calculate lowest point and get right occugrid

            # check if grid is empty or now
            if np.max(grid) == -1.0:
                print('gird is empty')
                return -1, -1

            if False:
                viz.show_grid(np.array(grid, dtype=np.float32).reshape(32, 32, 32)).show()
                ip.embed()

            # get approaching direction
            if top_only:
                indices_app_dir = [0] # always top
            else:
                indices_app_dir = self.get_approaching_direction(grid)

            

            for idx_app_dir in indices_app_dir:
                idx2name = ['top', 'front', 'left', 'right', 'front_left', 'front_right']
                # transform grid according to approaching direction
                g = np.array(grid)
                g = g.reshape((32, 32, 32))
                g = tf_grid(g, app_dir=idx2name[idx_app_dir])
                grid = tuple(g.ravel())

                p = self._get_prob_from_vg(grid)
                idx_wrist_ori = self.argnmax(p[:4])
                p_idx_wrist_ori = p[idx_wrist_ori]
                # ip.embed()
                pair_app_dir_wrist_ori.append((pt, idx_app_dir, idx_wrist_ori, p_idx_wrist_ori))
                # indices_wrist_ori = self.get_wrist_orientation(grid)

                # for idx_wrist_ori in indices_wrist_ori:
        # ip.embed()
        # sort
        pair_app_dir_wrist_ori.sort(key=lambda tup: tup[3])

        return pair_app_dir_wrist_ori[::-1] # return reverse order (first one most likely)

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
            
            # ip.embed()

            if len(obj_centers) == 0 or len(obj_heights) == 0:
                print('there is no object on the table.')
                return True
            else:
                if self.use_left_arm:
                    if self._obj_selection_mode == 0:
                        (c_l, h_l, r_l, cloud_seg) = (obj_centers[0], obj_heights[0], obj_radius[0], cloud_segs[0]) # leftmost
                    elif self._obj_selection_mode == 1:
                        i = np.random.randint(len(obj_centers))
                        (c_l, h_l, r_l, cloud_seg) = (obj_centers[i], obj_heights[i], obj_radius[i], cloud_segs[i]) # random

                    pair_app_dir_wrist_ori = self.get_grasping_indices_all(cloud_seg, c_l, h_l, self._p_th)
                    # pair_app_dir_wrist_ori = self.get_grasping_indices(cloud_seg, c_l, h_l, top_only=self._top_only)


                    # pair_app_dir_wrist_ori = [(2, 2)]

                    print(pair_app_dir_wrist_ori)
                    # ip.embed()
                    
                    for pair in pair_app_dir_wrist_ori:
                        idx_app_dir = pair[0]
                        idx_wrist_ori = pair[1]
                        print('idx_app_dir: %d' % idx_app_dir)
                        print('idx_wrist_ori: %d' % idx_wrist_ori)

                        if self._avoid_top and idx_app_dir == 0:
                            continue

                        # ip.embed()

                        if idx_app_dir == -1 or idx_wrist_ori == -1:
                            print('empty occupancy grid')
                            return False
                        
                        (success, pose, pose_appr, arm_group) = self.grasp(c_l, h_l, r_l, idx_app_dir, idx_wrist_ori)

                        if success:
                            self.hand.LeftClose()
                            time.sleep(3)

                            if idx_app_dir > 0:
                                pose_appr.position.z += 0.15 # TODO: tune this value

                            success = self.arm.MoveTo(pose_appr, arm_group)

                            if self.fast_eval_:
                                success = self.arm.MoveTo(pose, arm_group)
                                self.hand.LeftOpen()
                                time.sleep(3)

                                # success = self.arm.MoveTo(pose_appr, arm_group)
                            else:
                                self.arm.MoveToBin('left', appr=True)
                                
                                self.arm.MoveToBin('left', appr=False)

                                self.hand.LeftOpen()
                                time.sleep(3)

                                self.arm.MoveToBin('left', appr=True)

                            # move to position to spot objects
                            success = self.arm.MoveToSpotLoc()
                            return True
                        else:
                            print "Error in motion!"
                    return False

    def generate_grasp_points(self):
        rospy.loginfo("wait for point cloud: %s", "/camera/depth_registered/points")
        cloud = rospy.wait_for_message("/camera/depth_registered/points", PointCloud2)

        (center_list, cloud, cloud_segs) = self._get_obj_centers(cloud)

        grasp_points = []
        for pt in pc2.read_points(cloud, skip_nans=True):
            pt_ = Point()
            pt_.x, pt_.y, pt_.z = pt[0], pt[1], pt[2]

            grasp_points.append(pt_)

        # randomly sample grasp points
        n = len(grasp_points)
        sample_indices = np.random.choice(range(n), self._num_samples, replace=False)
        
        return ([grasp_points[i] for i in sample_indices], cloud)

    def run_cluttered(self):
        print "starting run"

        # only consider top grasping in cluttered setting
        self._top_only = True
        # self._num_samples = 10

        # make sure hand is open
        self.hand.LeftOpen()
        self.hand.RightOpen()

        # move to position to spot objects
        success = self.arm.MoveToSpotLoc()

        if not success:
            print "Error moving to spot location!"
            return False

        # wait until we spot an object
        while not rospy.is_shutdown():
            print "waiting for object to be detected"

            # sorted left to right wrt baxter
            # (obj_centers, obj_heights, obj_radius, cloud, cloud_segs) = self.find_obj()
            (grasp_points, cloud) = self.generate_grasp_points()
            # grasp_point: contact Point()
            
            if len(grasp_points) == 0:
                print('there is no object on the table.')
                return True
            else:
                if self.use_left_arm:
                    pair_app_dir_wrist_ori = self.get_grasping_indices_clutter(cloud, grasp_points, top_only=True)

                    print(pair_app_dir_wrist_ori)

                    # ip.embed()
                    
                    for pair in pair_app_dir_wrist_ori:
                        grasp_point = pair[0]
                        c_l = grasp_point
                        h_l = 0.05
                        r_l = 0.0
                        idx_app_dir = pair[1]
                        idx_wrist_ori = pair[2]
                        print('idx_app_dir: %d' % idx_app_dir)
                        print('idx_wrist_ori: %d' % idx_wrist_ori)

                        # ip.embed()

                        if idx_app_dir == -1 or idx_wrist_ori == -1:
                            print('empty occupancy grid')
                            return False
                        
                        (success, pose, pose_appr, arm_group) = self.grasp(c_l, h_l, r_l, idx_app_dir, idx_wrist_ori)

                        if success:
                            self.hand.LeftClose()
                            time.sleep(3)

                            if idx_app_dir > 0:
                                pose_appr.position.z += 0.15 # TODO: tune this value

                            success = self.arm.MoveTo(pose_appr, arm_group)

                            if self.fast_eval_:
                                success = self.arm.MoveTo(pose, arm_group)
                                self.hand.LeftOpen()
                                time.sleep(3)

                                # success = self.arm.MoveTo(pose_appr, arm_group)
                            else:
                                self.arm.MoveToBin('left', appr=True)
                                
                                self.arm.MoveToBin('left', appr=False)

                                self.hand.LeftOpen()
                                time.sleep(3)

                                self.arm.MoveToBin('left', appr=True)

                            # move to position to spot objects
                            success = self.arm.MoveToSpotLoc()
                            return True
                        else:
                            print "Error in motion!"
                    return False

def main(args):
    rospy.init_node('grasping_demo', anonymous=True)
    g = Grasping()
    shutdown = False

    try:
        while not rospy.is_shutdown():
            no_error = g.run()
            # no_error = g.run_cluttered()
            # shutdown = not no_error
            print('##### press any key to proceed #####')
            raw_input()
    except KeyboardInterrupt:
        print "grasping_demo Shutting down"

if __name__ == '__main__':
    main(sys.argv)
