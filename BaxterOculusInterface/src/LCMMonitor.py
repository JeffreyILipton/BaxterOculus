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
if DEBUG: print "time:", curtime

def ProcessImage(lc,lcChannel,rosmsg):
    global curtime
    global CAM_TIME
    #print "tick"
    if time.time()-curtime >CAM_TIME:
        #print "ros: %ix%i"%(rosmsg.height,rosmsg.width)
        lcm_msg = image_t()
        lcm_msg.height = rosmsg.height
        lcm_msg.width = rosmsg.width
        lcm_msg.row_stride = rosmsg.step
        lcm_msg.data = rosmsg.data
        lcm_msg.size = len(rosmsg.data)
        #print "pub on ",lcChannel
        #print "ros data: ",len(rosmsg.data)
        #print "lcm data: ", len(lcm_msg.encode())
        #tm = trigger_t()
        #tm.trigger = True
        lc.publish(lcChannel,lcm_msg.encode())
        curtime = time.time()
    #lc.publish(LCM_L_CAMERA,tm.encode())
    #lc.publish(LCM_L_TRIGGER,tm.encode())


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
    rospy.init_node('part_listener', anonymous=True)
    full_param_name = rospy.search_param('part')
    param_value = rospy.get_param(full_param_name)
    part = param_value
    #print "PART IS:",part

    full_param_name = rospy.search_param('broadcastChannel')
    param_value = rospy.get_param(full_param_name)
    broadcastChannel = param_value


    scales=[1.00,1.00,1.00]# m/mm
    offsets = [0,00,0.0]
    mins = [-5,-5,-5]
    maxs = [5,5,5]
    


    timeout =0.1
    sub_func = None
    channel = ""
    msgType = None

    connection_list = []
    lc = lcm.LCM("udpm://239.255.76.67:7667:?ttl=1")
    ardPort = "/dev/ttyACM2"

    if part == 'left_camera':

        lcChannel = LCM_L_CAMERA
        #lc = lcm.LCM("udpm://239.255.76.67:7667:?ttl=1")
        channel = broadcastChannel
        msgType = Image
        sub_func = lambda x : ProcessImage(lc,lcChannel,x)
        #sub_func = partial(ProcessImage,lc,lcChannel)
        connection_list.append((channel,msgType,sub_func))
        connection_list.append((channel,msgType,sub_func))

        l_cam = CameraController('left_hand_camera')
        l_cam.resolution=(320,200)#(1280, 800)
        l_cam.open()

    elif part == 'right_camera':
        lcChannel = LCM_R_CAMERA
        channel = broadcastChannel
        msgType = Image

        sub_func = lambda x: ProcessImage(lc,lcChannel,x)
        #sub_func = partial(ProcessImage,lc,lcChannel)
        connection_list.append((channel,msgType,sub_func))

        r_cam = CameraController('right_hand_camera')
        r_cam.resolution=(320,200)#(1280, 800)
        r_cam.open()


    else :
        print "unknown part:", part
        return 0
     

    
    #Start movement
    print "starting part: ",part
    curtime = time.time()
    rs = RobotEnable(CHECK_VERSION)
    for connection in connection_list:
        channel,msgType,sub_func = connection
        rospy.Subscriber(channel, msgType, sub_func)
        


    rospy.spin()


    #print "done"
    #rs.disable()
    return 0
        


if __name__ == "__main__":
    sys.exit(int(main() or 0))



