#!/usr/bin/env python
import os
import sys

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
from ArduinoInterface import *
from Comms import *
from ServiceTimeout import *
from oculuslcm import*
from Quaternion import *
import time
import numpy as np

DEBUG = False
CAM_TIME = 0.1
RANGE_TIME = 0.05
curtime = time.time()

confidence = 0
userID = -1
id
lcmChannel = ""

if DEBUG: print "time:", curtime




def ProcessConfidence(data):
    global confidence
    confidence = data
    
def ProcessQuery(data):
    receivedValue = int(data.data)
    global userID
    if (userID == -1) or (userID == -2) or (-receivedValue == userID) or (receivedValue == -1) or (receivedValue == -2):
        userID = receivedValue
    SendLCM()

def SendLCM():
    global userID
    global id
    global lcmChannel
    global confidence
    print "user ID: ", userID

    data                    = info_t()
    data.id                 = id
    data.confidence         = confidence
    data.threshold          = .5
    data.user               = userID
    data.enabled            = True

    lc = lcm.LCM("udpm://239.255.76.67:7667:?ttl=1")
    lc.publish(lcmChannel,data.encode())

def main():
    """BaxterInterface
 
   A program from MIT's DRL for using Baxter via LCM messages
 
   Run this by passing the *limb*,head, or trigger, and the node 
   for controlling that part will spin up
   """
    #arg_fmt = argparse.RawDescriptionHelpFormatter
    #parser = argparse.ArgumentParser(formatter_class=arg_fmt,
    #                                 description=main.__doc__)
    #parser.add_argument(
    #    '-p', '--part', choices=['left', 'right','head','right_trigger','right_gripper','left_gripper','left_range','right_range'], required=True,
    #    help="the part to control, 'left', 'right','head','right_trigger','right_gripper','left_gripper','left_range','right_range'"
    #)
    #args = parser.parse_args(rospy.myargv()[1:])

    global lcmChannel
    global id

    rospy.init_node('id', anonymous=True)
    full_param_name = rospy.search_param('id')
    param_value = rospy.get_param(full_param_name)
    id = param_value

    full_param_name = rospy.search_param('lcmChannel')
    param_value = rospy.get_param(full_param_name)
    lcmChannel = str(param_value)
    
    timeout =0.1
    sub_func = None
    channel = ""
    msgType = None

    connection_list = []




    channel = ROS_CONFIDENCE
    sub_func = ProcessConfidence
    msgType = Float32
    connection_list.append((channel,msgType,sub_func))   
         
    channel2 = ROS_QUERY
    sub_func2 = ProcessQuery
    msgType2 = Int16   
    connection_list.append((channel2,msgType2,sub_func2))  
        

     

    
    #Start movement
    curtime = time.time()
    rs = RobotEnable(CHECK_VERSION)
    for connection in connection_list:
        channel,msgType,sub_func = connection
        rospy.Subscriber(channel, msgType, sub_func)
        


    

    rate = rospy.Rate(1) # 1hz
    while not rospy.is_shutdown():
           print "tick"
           #print lcChannel
           SendLCM()
           #rospy.spin()
           rate.sleep()
    return 0


if __name__ == "__main__":
    sys.exit(int(main() or 0))



