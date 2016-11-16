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
if DEBUG: print "time:", curtime

### Runs on LCM
def ProcessImage(lc,lcChannel,rosmsg):
    global curtime
    global CAM_TIME

    if( (time.time()-curtime >CAM_TIME) or True ):
        lcm_msg = image_t()
        lcm_msg.height = rosmsg.height
        lcm_msg.width = rosmsg.width
        lcm_msg.row_stride = rosmsg.step
        lcm_msg.data = rosmsg.data
        lcm_msg.size = len(rosmsg.data)

        lc.publish(lcChannel,lcm_msg.encode())
        curtime = time.time()


### Runs on LCM take 
def ProcessRange(lc,lcChannel,rosmsg):
    global curtime 
    global RANGE_TIME

    if time.time()-curtime  >RANGE_TIME:
        msg = range_t()
        msg.range = rosmsg.range
        lc.publish(lcChannel,msg.encode())
        curtime  = time.time()


def IsValid(lc,lcChannel,rosmsg):
    global DEBUG
    if DEBUG: print "Channel:" + lcChannel + ": " + str(rosmsg.data)
    lcm_msg = trigger_t()
    lcm_msg.trigger = rosmsg.data
    lc.publish(lcChannel,lcm_msg.encode())

def CurrentPos(lc,lcChannel,rosmsg):
    lcm_pos_msg = pose_t()
    global DEBUG
    if DEBUG: print "Channel:" + lcChannel + "received pose"
    #print "Pose: " + rosmsg.position.x + "," + rosmsg.position.y + "," + rosmsg.position.z
    lcm_pos_msg.position = [rosmsg.position.x,rosmsg.position.y,rosmsg.position.z]
    lcm_pos_msg.orientation = [rosmsg.orientation.x,rosmsg.orientation.y,rosmsg.orientation.z,rosmsg.orientation.w]
    lc.publish(lcChannel,lcm_pos_msg.encode())




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
        channel = ROS_LEFT_VALID
        connection_list.append((channel,msgType,sub_func)) 

        lcChannel = LCM_LEFT_CURRENTPOS
        msgType = Pose
        channel = ROS_LEFT_CURRENTPOS
        sub_func = partial(CurrentPos, lc, lcChannel)
        connection_list.append((channel,msgType,sub_func)) 


    elif part =='right':
        lcChannel = LCM_RIGHT_VALID
        msgType = Bool
        sub_func = partial(IsValid,lc,lcChannel)
        channel = ROS_RIGHT_VALID
        connection_list.append((channel,msgType,sub_func)) 

        lcChannel = LCM_RIGHT_CURRENTPOS
        msgType = Pose
        sub_func = partial(CurrentPos, lc, lcChannel)
        channel = ROS_RIGHT_CURRENTPOS
        connection_list.append((channel,msgType,sub_func)) 

    elif part == 'left_range':
        lcChannel = LCM_L_RANGE
        channel = ROS_L_RANGE
        msgType = Range
        sub_func = partial(ProcessRange,lc,lcChannel)
        connection_list.append((channel,msgType,sub_func))

    elif part == 'right_range':
        lcChannel = LCM_R_RANGE
        channel = ROS_R_RANGE
        msgType = Range
        sub_func = partial(ProcessRange,lc,lcChannel)
        connection_list.append((channel,msgType,sub_func))

    elif part == 'right_camera':
        lcChannel = LCM_R_CAMERA
        channel = ROS_R_CAMERA
        msgType = Image

        sub_func = lambda x: ProcessImage(lc,lcChannel,x)
        #sub_func = partial(ProcessImage,lc,lcChannel)
        connection_list.append((channel,msgType,sub_func))


    elif part == 'left_camera':

        lcChannel = LCM_L_CAMERA
        #lc = lcm.LCM("udpm://239.255.76.67:7667:?ttl=1")
        channel = ROS_L_CAMERA
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

