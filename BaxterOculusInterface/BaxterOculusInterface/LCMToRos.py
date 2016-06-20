import os
import sys
if os.name!='nt':
    import lcm
    import rospy
    import roshelper
    from baxter_interface import *
    from std_msgs.msg import Bool
    from geometry_msgs.msg import (
        Pose,
        Point,
        Quaternion,
    )

from oculuslcm import *
from functools import *
from Comms import *


def PointQuatToPose(baxter_pos,orientation):
    p = Pose(
                position=Point(
                    x=baxter_pos[0],
                    y=baxter_pos[1],
                    z=baxter_pos[2],
                ),
                orientation=Quaternion(
                    x=orientation[1],
                    y=orientation[2],
                    z=orientation[3],
                    w=orientation[0],
                ),
           )
    return p

def LCMPoseToRos(RosPub,LcmChannel,lcmData):
    lcm_msg = hand_t.decode(lcmData)
    p = Pose(lcm_msg.position, lcm_msg.orientation)
    RosPub.publish(p)
    
def LCMBoolToRos(RosPub,lcmChannel, lcmData):
    lcm_msg = trigger_t.decode(lcmData)
    b = Bool(lcm_msg.trigger)
    RosPub.publish(b)

class LCMInterface():
    def __init__(self):
        self.lc = lcm.LCM()
        self.subscriptions={}
        connections = [(ROS_LEFT,LCM_LEFT,Pose)]#,


        for connection in connections:
            ros_channnel,lcm_channel,ros_msg_type = connnection
            pub = rospy.Publisher(ros_channnel, ros_msg_type, queue_size=1)
            if ros_msg_type == Pose:
                subscriber = partial(LCMPoseToRos,pub)
            elif ros_msg_type == Bool:
                subscriber = partial(LCMBoolToRos,pub)

            self.subscriptions[ros_channnel] = self.lc.subscribe(lcm_channel,subscriber)

    def run(self):
        try:
            while not rospy.is_shutdown():
                 self.lc.handle()
                 print "."
        except KeyboardInterrupt:
            pass    