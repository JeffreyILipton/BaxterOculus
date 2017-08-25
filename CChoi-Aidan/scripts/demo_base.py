#!/usr/bin/env python
import rospy
import sys
import datetime
from baxter_soft_hand.arm.ArmInterfacePlanner import *
from baxter_soft_hand.softhand.SoftHand import * # SoftHand, HandType
from baxter_soft_hand.srv import DetectObject, InHandLocalization, InHandLocalizationRequest, InHandLocalizationResponse
from geometry_msgs.msg import Point, Pose, Quaternion
from std_msgs.msg import Empty, String
from sensor_msgs.msg import Image, PointCloud2
from cv_bridge import CvBridge, CvBridgeError
import tf_conversions.posemath as pm # for pm.toMatrix & pmfromMsg
from pcl_planar_segmentation.msg import *
from pcl_planar_segmentation.srv import *
from moveit_msgs.msg import *
from moveit_msgs.srv import *
from soft_grasp.srv import *
import cv2
import tf
import random
import copy
import time
import IPython as ip
import rospkg
import sensor_msgs.point_cloud2 as pc2

class DemoBase:
    def __init__(self, ht=HandType.soft):
        # publishers and subscribers
        self.arm = ArmInterface()
        self.hand_type = ht
        self.hand = SoftHand(ht=ht)
        print "open hand"
        self.hand.Open()
        print "right before CV bridge"
        self.bridge = CvBridge()
        print "waiting for service"
        self.image_pub = rospy.Publisher('/robot/xdisplay', Image, queue_size=10)

        rospack = rospkg.RosPack()
        self.pkg_path = rospack.get_path('baxter_soft_hand')

        print "publish image"
        face = cv2.imread(self.pkg_path + '/face_imgs/face.jpg')
        img = self.bridge.cv2_to_imgmsg(face, encoding='bgr8')
        self.image_pub.publish(img)

        self.listener = tf.TransformListener()

        rospy.sleep(2)

        try: 
            (t, q) = self.listener.lookupTransform('/world', '/camera_rgb_optical_frame', rospy.Time(0))
            # better way to do this
            self.wTc_pose = Pose()
            self.wTc_pose.position.x = t[0]
            self.wTc_pose.position.y = t[1]
            self.wTc_pose.position.z = t[2]

            self.wTc_pose.orientation.x = q[0]
            self.wTc_pose.orientation.y = q[1]
            self.wTc_pose.orientation.z = q[2]
            self.wTc_pose.orientation.w = q[3]

            print self.wTc_pose
        except (tf.LookupException, tf.ConnectivityException, tf.ExtrapolationException):
            rospy.logerr('There is no wTc available')

        # threshold for safty
        # self.th_z = -0.06

        self.use_left_arm = True
        self.use_right_arm = False

        self._pub_ps = rospy.Publisher('planning_scene', PlanningScene)
        rospy.wait_for_service('/get_planning_scene', 10.0)
        self._srv_gps = rospy.ServiceProxy('/get_planning_scene', GetPlanningScene)

        self.hand.close_value = 1.0 # good for any object grasping

        # service proxies
        rospy.loginfo("wait for service: get_objcenterlist")
        rospy.wait_for_service("obj_segmentor/get_objcenterlist")
        self._srv_objcenterlist = rospy.ServiceProxy("obj_segmentor/get_objcenterlist", GetObjCenterList)

        rospy.loginfo("wait for service: obj_segmentor/get_occugrid")
        rospy.wait_for_service("obj_segmentor/get_occugrid")
        self._srv_occugrid = rospy.ServiceProxy("obj_segmentor/get_occugrid", GetOccuGrid)

        rospy.loginfo("wait for service: detect_grasping")
        rospy.wait_for_service("detect_grasping")
        self._srv_detgrasping = rospy.ServiceProxy('detect_grasping', DetectGrasping)

        # self._obj_selection_mode = 1
        # 0: leftmost
        # 1: random

    def ignoreHandCollision(self, ignore, name_list=['left_soft_gripper_single', 'left_soft_gripper_double', 'right_soft_gripper_single', 'right_soft_gripper_double']):
        req = PlanningSceneComponents(components=PlanningSceneComponents.ALLOWED_COLLISION_MATRIX)
        resp = self._srv_gps(req)

        acm = resp.scene.allowed_collision_matrix
        for name in name_list:
            if name in acm.default_entry_names:
                idx = acm.default_entry_names.index(name)
                acm.default_entry_values[idx] = ignore
            else:
                acm.default_entry_names += [name]
                acm.default_entry_values += [ignore]

        planning_scene_diff = PlanningScene(
                is_diff=True,
                allowed_collision_matrix=acm)

        self._pub_ps.publish(planning_scene_diff)
        rospy.sleep(1.0)

    def camera2world(self, cTo_pose):
        cTo = pm.toMatrix(pm.fromMsg(cTo_pose))
        wTc = pm.toMatrix(pm.fromMsg(self.wTc_pose))

        wTo = np.dot(wTc, cTo)

        wTo_pose = pm.toMsg(pm.fromMatrix(wTo))
        wTo_pose.orientation = self.normalize_quat(wTo_pose.orientation)
        return wTo_pose

    def world2camera(self, wTo_pose):
        wTo = pm.toMatrix(pm.fromMsg(wTo_pose))
        wTc = pm.toMatrix(pm.fromMsg(self.wTc_pose))

        cTo = np.dot(np.linalg.inv(wTc), wTo)

        cTo_pose = pm.toMsg(pm.fromMatrix(cTo))
        cTo_pose.orientation = self.normalize_quat(cTo_pose.orientation)
        return cTo_pose

    def normalize_quat(self, q):
        n = np.linalg.norm([q.x, q.y, q.z, q.w])
        q.x /= n
        q.y /= n
        q.z /= n
        q.w /= n

        return q

    def _get_obj_centers(self, cloud):
        try:
            resp = self._srv_objcenterlist(cloud)
        except rospy.ServiceException, e:
            print "Service call failed: %s" % e

        # copy header
        for seg in resp.segments:
            seg.header = resp.cloud.header

        return (resp.list, resp.cloud, resp.segments)

    def find_obj(self):
        rospy.loginfo("wait for point cloud: %s", "/camera/depth_registered/points")
        cloud = rospy.wait_for_message("/camera/depth_registered/points", PointCloud2)

        (center_list, cloud, cloud_segs) = self._get_obj_centers(cloud)

        if len(center_list.center) > 0:
            # if self._obj_selection_mode == 0:
                # sorting: leftmost first
            [center_sorted, height_sorted, radius_sorted, cloud_sorted] = [list(x) for x in zip(*sorted(zip(center_list.center, center_list.height, center_list.radius, cloud_segs), key=lambda pair: -pair[0].y))]
            # elif self._obj_selection_mode == 1:
            #     # randomly choose one
            #     n = len(center_list.center)
            #     i = np.random.randint(n)
            #     [center_sorted, height_sorted, radius_sorted, cloud_sorted] = [center_list.center[i], center_list.height[i], center_list.radius[i], cloud_segs[i]]

            return (center_sorted, height_sorted, radius_sorted, cloud, cloud_sorted)
        else:
            return ([], [], [], [], [])


    def get_occugrid(self, cloud_segs, obj_center, obj_height):
        try:
            req = GetOccuGridRequest()
            req.cloud = cloud_segs
            req.obj_center = obj_center
            req.obj_height = obj_height
            
            resp = self._srv_occugrid(req)
        except rospy.ServiceException, e:
            print "Service call failed: %s" % e

        return resp.occu_grid

    def argnmax(self, p):
        # when there are more than one maximum, randomly sample one of them
        maxes = np.argwhere(p == np.amax(p)).flatten().tolist()
        return maxes[np.random.randint(len(maxes))]

    def grasp(self, obj_position, obj_height, obj_radius, dir_idx, wrist_idx):
        # dir: approaching direction
        # wrist_idx (0 - 5)
        assert dir_idx >= 0 and dir_idx <= 5
        assert wrist_idx >= 0 and wrist_idx <= 3

        if dir_idx == 0: # top
            q = Quaternion(x=0.0257775800661, y=0.999447656154, z=0.0121237801412, w=-0.0171146992458)
        elif dir_idx == 1: # front
            q = Quaternion(x=0.008523264789506494, y=0.772262331809102, z=-0.04019356839387756, w=0.6339737548872694)
        elif dir_idx == 2: # left
            q = Quaternion(x=0.558970429433, y=0.556661579531, z=-0.44939524066, w=0.419194301686)
        elif dir_idx == 3: # right
            q = Quaternion(x=0.541524910559, y=-0.538733648231, z=-0.454520010756, w=-0.458179427004)
        elif dir_idx == 4: # front_left
            # q = Quaternion(x=0.41796896828074054, y=0.8305806072521388, z=-0.18036227109336575, w=0.32079159524065626)
            q = Quaternion(x=0.291713728019, y=0.706069450228, z=-0.252957326362, w=0.593617404883)
        elif dir_idx == 5: # front_right
            # q = Quaternion(x=-0.33130897503484297, y=0.8227365472826587, z=0.17272480043139044, w=0.428374929404964)
            q = Quaternion(x=-0.262823622412, y=0.716500015439, z=0.255970882766, w=0.593321479936)
        else:
            print('unknown dir_idx')

        return self.arm.GoToGrasp(obj_position, obj_height, obj_radius, q, dir_idx, wrist_idx, appr_dist=0.20)



    def grasp_with_dir(self, obj_position, obj_height, obj_radius, gdir):
        # TODO: need to add width or radius from object segmentation
        if gdir == 0:
            q = Quaternion(x=0.6853411623513512, y=0.6385792943267595, z=-0.26787144003237595, w=0.22531947909361455)
            return self.arm.GoToLeftSideGrasp(obj_position, obj_height, obj_radius, q, rotz=0.0, appr_dist=0.20)
        elif gdir == 1:
            q = Quaternion(x=0.41796896828074054, y=0.8305806072521388, z=-0.18036227109336575, w=0.32079159524065626)
            return self.arm.GoToLeftSideGrasp(obj_position, obj_height, obj_radius, q, rotz=-45.0, appr_dist=0.20)
        elif gdir == 2:
            # q = Quaternion(x=0.010254934932972134, y=0.9013326525684179, z=-0.015336685284765235, w=0.43273441255344625)
            # q = Quaternion(x=-0.04053793381635134, y=0.9097652207646533, z=-0.010854013257653172, w=0.4129965004756646)
            # q = Quaternion(x=, y=, z=, w=)
            # q = Quaternion(x=0.05014142855232167, y=0.8922749315890535, z=-0.04509288295767298, w=0.44642795108174843)

            # q = Quaternion(x=0.08812754474240818, y=0.8622988217814488, z=-0.06235959137767171, w=0.4947580814652851)

            # q = Quaternion(x=-0.01023943231673163, y=0.8821033090457832, z=-0.006529370184744928, w=0.4708994303684184)
            # q = Quaternion(x=0.006, y=0.902, z=-0.009, w=0.432)

            q = Quaternion(x=0.008523264789506494, y=0.772262331809102, z=-0.04019356839387756, w=0.6339737548872694)
            # set hight to zero
            return self.arm.GoToLeftSideGrasp(obj_position, 0.0, obj_radius, q, rotz=-90.0, appr_dist=0.20)
        elif gdir == 3:
            q = Quaternion(x=-0.33130897503484297, y=0.8227365472826587, z=0.17272480043139044, w=0.428374929404964)
            return self.arm.GoToLeftSideGrasp(obj_position, obj_height, obj_radius, q, rotz=-135.0, appr_dist=0.20)
        elif gdir == 4:
            q = Quaternion(x=-0.6370268689830207, y=0.6655327491682272, z=0.2654093166264428, w=0.2842900325831973)
            return self.arm.GoToLeftSideGrasp(obj_position, obj_height, obj_radius, q, rotz=-180.0, appr_dist=0.20)
        elif gdir == 5:
            return self.arm.GoToTopGraspLeft(obj_position, obj_height, rotz=90.0)
        elif gdir == 6:
            return self.arm.GoToTopGraspLeft(obj_position, obj_height, rotz=45.0)
        elif gdir == 7:
            return self.arm.GoToTopGraspLeft(obj_position, obj_height, rotz=0.0)
        elif gdir == 8:
            return self.arm.GoToTopGraspLeft(obj_position, obj_height, rotz=-45.0)
        else:
            ip.embed()
            print 'unknown grasping direction'

    def Grasp2Msg(self, g, offset = 0.08):
        # convert from agile_grasp2/GraspMsg to Pose
        m = np.identity(4)
        m[0, 0] = g.axis.x;     m[1, 0] = g.axis.y;     m[2, 0] = g.axis.z
        m[0, 1] = g.binormal.x; m[1, 1] = g.binormal.y; m[2, 1] = g.binormal.z
        m[0, 2] = g.approach.x; m[1, 2] = g.approach.y; m[2, 2] = g.approach.z
        
        tran = m[:3, 2] * -offset
        m[0, 3] = g.surface.x + tran[0]
        m[1, 3] = g.surface.y + tran[1]
        m[2, 3] = g.surface.z + tran[2]

        return pm.toMsg(pm.fromMatrix(m))
    def run(self):
        print('demo_base class has empty run function')
        return
