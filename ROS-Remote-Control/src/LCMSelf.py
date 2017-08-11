#!/usr/bin/env python
import os
import sys
if os.name!='nt':
    import lcm
    import rospy
    from std_msgs.msg import *
    from sensor_msgs.msg import *
    from geometry_msgs.msg import (
        Pose,
        Point,
        Quaternion,
    )

from oculuslcm import *
from functools import *
from Comms import *

import operator
from itertools import *
from math import *
import time

DEBUG = False
CAM_TIME = 0.1
RANGE_TIME = 0.05
curtime = time.time()

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
    rospy.init_node('ROS_2_LCM', anonymous=True)
    full_param_name = rospy.search_param('part')
    param_value = rospy.get_param(full_param_name)
    part = param_value
    print "PART IS:",part
    
    full_prefix_name = rospy.search_param('prefix')
    prefix_value = rospy.get_param(full_prefix_name)
    prefix = ""
    if prefix_value: prefix = prefix_value
    print "PREFIX is:",prefix


    full_rate_name = rospy.search_param('rate')
    rate_value = rospy.get_param(full_rate_name)
    print "rate_value:",rate_value


    timeout =0.1
    sub_func = None
    channel = ""
    msgType = None

    connection_list = []
    lc = lcm.LCM("udpm://239.255.76.67:7667:?ttl=1")

    if part == 'left':
        lcChannel = LCM_LEFT_VALID
        msgType = Bool
        sub_func = partial(IsValid,lc,lcChannel)
        channel = prefix+ROS_LEFT_VALID
        connection_list.append((channel,msgType,sub_func)) 

        lcChannel = LCM_LEFT_CURRENTPOS
        msgType = Pose
        channel = prefix+ROS_LEFT_CURRENTPOS
        sub_func = partial(CurrentPos, lc, lcChannel)
        connection_list.append((channel,msgType,sub_func)) 


    elif part =='right':
        lcChannel = LCM_RIGHT_VALID
        msgType = Bool
        sub_func = partial(IsValid,lc,lcChannel)
        channel = prefix+ROS_RIGHT_VALID
        connection_list.append((channel,msgType,sub_func)) 

        lcChannel = LCM_RIGHT_CURRENTPOS
        msgType = Pose
        sub_func = partial(CurrentPos, lc, lcChannel)
        channel = prefix+ROS_RIGHT_CURRENTPOS
        connection_list.append((channel,msgType,sub_func)) 

    elif part == 'left_range':

        if rate_value: RANGE_TIME = 1.0/rate_value

        lcChannel = LCM_L_RANGE
        channel = prefix+ROS_L_RANGE
        msgType = Range
        sub_func = partial(ProcessRange,lc,lcChannel)
        connection_list.append((channel,msgType,sub_func))

    elif part == 'right_range':
        if rate_value: RANGE_TIME = 1.0/rate_value

        lcChannel = LCM_R_RANGE
        channel = prefix+ROS_R_RANGE
        msgType = Range
        sub_func = partial(ProcessRange,lc,lcChannel)
        connection_list.append((channel,msgType,sub_func))

    elif part == 'right_camera':

        if rate_value: CAM_TIME = 1.0/rate_value

        lcChannel = LCM_R_CAMERA
        channel = prefix+ROS_R_CAMERA
        msgType = Image

        sub_func = lambda x: ProcessImage(lc,lcChannel,x)
        #sub_func = partial(ProcessImage,lc,lcChannel)
        connection_list.append((channel,msgType,sub_func))


    elif part == 'left_camera':

        if rate_value: CAM_TIME = 1.0/rate_value

        lcChannel = LCM_L_CAMERA
        #lc = lcm.LCM("udpm://239.255.76.67:7667:?ttl=1")
        channel = prefix+ROS_L_CAMERA
        msgType = Image
        sub_func = lambda x : ProcessImage(lc,lcChannel,x)
        #sub_func = partial(ProcessImage,lc,lcChannel)
        connection_list.append((channel,msgType,sub_func))

    else :
        print "unknown part:", part
        return 0


    #print "lcm start"
    #LCMI = LCMInterface()
    #rospy.init_node('RosToLCM',anonymous = True)
    #try:
    #    LCMI.run()
    #except rospy.ROSInterruptException:pass

    #Start movement
    print "starting part: ",part
    curtime = time.time()
    for connection in connection_list:
        channel,msgType,sub_func = connection
        rospy.Subscriber(channel, msgType, sub_func)
        


    rospy.spin()


    #print "done"
    #rs.disable()
    return 0
        


if __name__ == "__main__":

    sys.exit(int(main() or 0))

