#!/usr/bin/env python
import os
import sys
if os.name!='nt':
    import lcm
    import rospy
    from baxter_interface import *
    from std_msgs.msg import *
    from geometry_msgs.msg import (
        Pose,
        Point,
        Quaternion,
    )


 from cnn3d_grasping.srv import Start, StartRequest, Stop, StopRequest, \
    GetThreshold, GetThresholdRequest, \
    SetThreshold, SetThresholdRequest, \
    NeedHelp, NeedHelpResponse

from oculuslcm import *
from functools import *
from Comms import *


def PointQuatToPose(baxter_pos,orientation):
    p = Pose()
    p.position = Point(
                    x=baxter_pos[0],
                    y=baxter_pos[1],
                    z=baxter_pos[2],
                 )
    p.orientation=Quaternion(
                    x=orientation[1],
                    y=orientation[2],
                    z=orientation[3],
                    w=orientation[0],
                )
    return p


def LCMGripperCMDToRos(RosPub,LcmChannel,lcmData):
    lcm_msg = cmd_t.decode(lcmData)
    print "LCM Gripper published: ",lcm_msg.command
    RosPub.publish(lcm_msg.command)

def LCMGripperVelToRos(RosPub,LcmChannel,lcmData):
    lcm_msg = velocity_t.decode(lcmData)
    RosPub.publish(lcm_msg.velocity)


def LCMPoseToRos(RosPub,LcmChannel,lcmData):
    lcm_msg = pose_t.decode(lcmData)
    p = PointQuatToPose(lcm_msg.position, lcm_msg.orientation)
    #print "lcm pose:",lcm_msg.position,lcm_msg.orientation
    #print "ROS pose:",p
    RosPub.publish(p)
    
def LCMBoolToRos(RosPub,lcmChannel, lcmData):
    lcm_msg = trigger_t.decode(lcmData)
    RosPub.publish(lcm_msg.trigger)

class LCMInterface():
    def __init__(self):
        self.lc = lcm.LCM("udpm://239.255.76.67:7667:?ttl=1")
        self.subscriptions={}

        connections = [(ROS_LEFT,LCM_LEFT,Pose),
                       (ROS_L_CMD,LCM_L_CMD,UInt16),
                       (ROS_L_VEL,LCM_L_VEL,Float64),
                       (ROS_L_TRIGGER,LCM_L_TRIGGER,Bool),
                       (ROS_RIGHT,LCM_RIGHT,Pose),
                       (ROS_R_CMD,LCM_R_CMD,UInt16),
                       (ROS_R_VEL,LCM_R_VEL,Float64),
                       (ROS_R_TRIGGER,LCM_R_TRIGGER,Bool)]#,

        

        for connection in connections:
            ros_channnel,lcm_channel,ros_msg_type = connection
            pub = rospy.Publisher(ros_channnel, ros_msg_type, queue_size=1)
            if ros_msg_type == Pose:
                subscriber = partial(LCMPoseToRos,pub)
            elif ros_msg_type == Bool:
                subscriber = partial(LCMBoolToRos,pub)
            elif ros_msg_type == UInt16:
                subscriber = partial(LCMGripperCMDToRos,pub)
            elif ros_msg_type == Float64:
                subscriber = partial(LCMGripperVelToRos,pub)

        


            self.subscriptions[ros_channnel] = self.lc.subscribe(lcm_channel,subscriber)

        rospy.wait_for_service('demo_grasping/start')
        self._srv_start = rospy.ServiceProxy('demo_grasping/start', Start)

        rospy.wait_for_service('demo_grasping/stop')
        self._srv_stop = rospy.ServiceProxy('demo_grasping/stop', Stop)

        rospy.wait_for_service('demo_grasping/get_th')
        self._srv_get_th = rospy.ServiceProxy('demo_grasping/get_th', GetThreshold)

        rospy.wait_for_service('demo_grasping/set_th')
        self._srv_set_th = rospy.ServiceProxy('demo_grasping/set_th', SetThreshold)

        s = rospy.Service('~grasp_help', NeedHelp, self.srv_grasp_help)

    def run(self):
        try:
            while not rospy.is_shutdown():
                 self.lc.handle()
                 #print "."
        except KeyboardInterrupt:
            pass

    def start(self):
        req = StartRequest()
        resp = self._srv_start(req)
        print('done _test_start')

    def stop(self):
        req = StopRequest()
        resp = self._srv_stop(req)
        print('done _test_stop')

    def get_th(self):
        req = GetThresholdRequest()
        resp = self._srv_get_th(req)
        print('threshold value: {}'.format(resp.th))
        print('done _test_get_th')

    def set_th(self):
        req = SetThresholdRequest()
        req.th = 0.2
        resp = self._srv_set_th(req)
        print('done _test_set_th')

def main():
    print "lcm start"
    LCMI = LCMInterface()
    rospy.init_node('LCM_rebroadcast',anonymous = True)
    try:
        LCMI.run()
    except rospy.ROSInterruptException:pass



if __name__ == "__main__":

    sys.exit(int(main() or 0))

