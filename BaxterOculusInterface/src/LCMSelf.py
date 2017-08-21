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




#def ProcessHand(lc,lcChannel,lcPosChannel,iksvc,ns,timeout,handToBaxter, limb,limb_obj, data):

#def ProcessSelf(lc,lcChannel,data):
#    global curtime 
#    global SELF_TIME
#    #print "tick"
#    if time.time()-curtime  >SELF_TIME:
#        msg                     = robotself_t()
#        msg.id                  = data.id
#        msg.type                = data.type
#        msg.ability             = data.ability
#        msg.channels            = data.channels.split("|")
#        msg.channelCount        = len(msg.channels)
#        msg.queryChannel        = data.queryChannel
#        msg.leftNDIChannel      = data.leftNDIChannel
#        msg.rightNDIChannel     = data.rightNDIChannel
#        lc.publish(lcChannel,msg.encode())
#        curtime  = time.time()

def main():
    """BaxterInterface
 
   A program from MIT's DRL for using Baxter via LCM messages
 
   """
    rospy.init_node('self_sender', anonymous=True)
    full_param_name = rospy.search_param('id')
    param_value = rospy.get_param(full_param_name)
    print param_value
    id = int(param_value)
    #print "PART IS:",part

    full_param_name = rospy.search_param('type')
    param_value = rospy.get_param(full_param_name)
    type = int(param_value)

    full_param_name = rospy.search_param('ability')
    param_value = rospy.get_param(full_param_name)
    ability = param_value

    full_param_name = rospy.search_param('channels')
    param_value = rospy.get_param(full_param_name)
    channels = param_value

    full_param_name = rospy.search_param('queryChannel')
    param_value = rospy.get_param(full_param_name)
    queryChannel = param_value

    full_param_name = rospy.search_param('leftNDIChannel')
    param_value = rospy.get_param(full_param_name)
    leftNDIChannel = param_value

    full_param_name = rospy.search_param('rightNDIChannel')
    param_value = rospy.get_param(full_param_name)
    rightNDIChannel = param_value

    full_param_name = rospy.search_param('broadcastChannel')
    param_value = rospy.get_param(full_param_name)
    lcChannel = param_value

    data                    = robotself_t()
    data.id                 = id
    data.type               = type
    data.ability            = ability
    data.channels           = channels.split("|")
    data.channelCount       = len(data.channels)
    data.queryChannel       = queryChannel
    data.leftNDIChannel     = leftNDIChannel
    data.rightNDIChannel    = rightNDIChannel
    print data

    for i in range(0,data.channelCount):
        data.channels[i] = data.channels[i] + "|" + id

    lc = lcm.LCM("udpm://239.255.76.67:7667:?ttl=1")
    
    rate = rospy.Rate(1) # 1hz
    while not rospy.is_shutdown():
           print "tick"
           print lcChannel
           lc.publish(lcChannel,data.encode())
           rate.sleep()
    return 0
        


if __name__ == "__main__":
    print "STARTING"
    sys.exit(int(main() or 0))



