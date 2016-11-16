#!/usr/bin/env python
import os
import sys
import rospy

if os.name!='nt':
    import lcm
    import rospy
    from baxter_interface import *
    from geometry_msgs.msg import (
        PoseStamped,
        Pose,
        Point,
        Quaternion,
    )
    from std_msgs.msg import Header
    from std_msgs.msg import *
    from sensor_msgs.msg import Range, Image
    from baxter_core_msgs.srv import (
        SolvePositionIK,
        SolvePositionIKRequest,
    )

import argparse
import operator
from itertools import *
from functools import *
from math import *
from Comms import *
from oculuslcm import*
import time
import numpy as np


class Echo(object):
    def __init__(self,inchannel,outchannel,msgType,defaultMsg):
        self.msg = defaultMsg

        #rospy.init_node('echoer')

        self.pub = rospy.Publisher(outchannel, msgType, queue_size=2, latch=True)
        rospy.Subscriber(inchannel, msgType, self.update_value)

    def update_value(self, msg):
        self.msg =msg

    def run(self,rate=10):
        r = rospy.Rate(rate)
        while not rospy.is_shutdown():
            if self.msg != None: self.pub.publish(self.msg)
            r.sleep()


def main():
    rospy.init_node('Echo', anonymous=True)
    full_param_name = rospy.search_param('part')
    param_value = rospy.get_param(full_param_name)
    part = param_value

    outchannel = ""
    inchannel = ""
    channeltype = Bool
    channelDefault = None
    rate = 10

    if part == 'left_valid':
        outchannel = ROS_LEFT_VALID
        inchannel = ROS_LEFT_VALID_STATE
        channelDefault = Bool(False)
    elif part == 'right_valid':
        outchannel = ROS_RIGHT_VALID
        inchannel = ROS_RIGHT_VALID_STATE
        channelDefault = Bool(False)

    elif part == "left_ros_currentpos":
        outchannel = ROS_LEFT_CURRENTPOS
        inchannel  = ROS_LEFT_CURRENTPOS_STATE
        channeltype = Pose
        channelDefault = None

    elif part == "right_ros_currentpos":
        outchannel = ROS_RIGHT_CURRENTPOS
        inchannel  = ROS_RIGHT_CURRENTPOS_STATE
        channeltype = Pose
        channelDefault = None

    elif part == "left_cam":
        inchannel  = ROS_LEFT_CAM
        outchannel = ROS_LEFT_CAM_ECHO
        channeltype = Image
        channelDefault = None
        rate = 5

    elif part == "right_cam":
        inchannel  = ROS_RIGHT_CAM
        outchannel = ROS_RIGHT_CAM_ECHO
        channeltype = Image
        channelDefault = None
        rate = 5

    else:
        print "dont know part "+ part

    e = Echo(inchannel,outchannel,channeltype,channelDefault)
    e.run(rate)

if __name__ == "__main__":
    sys.exit(int(main() or 0))
