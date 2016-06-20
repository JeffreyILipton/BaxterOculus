#!/usr/bin/env python
import os
import sys

if os.name!='nt':
    import rospy
    from baxter_interface import *
    from geometry_msgs.msg import (
        PoseStamped,
        Pose,
        Point,
        Quaternion,
    )
    from std_msgs.msg import Header
    from std_msgs.msg import Bool
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


wave_1 = {'left_s0': -0.459, 'left_s1': -0.202, 'left_e0': 1.807, 'left_e1': 1.714, 'left_w0': -0.906, 'left_w1': -1.545, 'left_w2': -0.276}
wave_2 = {'left_s0': -0.395, 'left_s1': -0.202, 'left_e0': 1.831, 'left_e1': 1.981, 'left_w0': -1.979, 'left_w1': -1.100, 'left_w2': -0.448}


def minMax(min_val,max_val,val):
    return max(min_val,min(val,max_val))

def XYZRescale(scales, offsets, mins, maxs, xyz):
    return [ minMax(mins[i],maxs[i], scales[i]*(xyz[i]+offsets[i])) for i in range(0,3)]

def YawFromQuat(theta_min,theta_max, quat):
    # https://en.wikipedia.org/wiki/Conversion_between_quaternions_and_Euler_angles
    q0,q1,q2,q3 = quat
    return atan2(2*(q0*q1+q2*q3),1-2*(q1*q1+q2*q2))


def poseFromPosQuatLib(hdr,limb,baxter_pos,orientation):
    poses = {
        limb: PoseStamped(
            header=hdr,
            pose=Pose(
                position=Point(
                    x=baxter_pos[0],
                    y=baxter_pos[1],
                    z=baxter_pos[2],
                ),
                orientation=Quaternion(
                    x=orientation[0],
                    y=orientation[1],
                    z=orientation[2],
                    w=orientation[3],
                ),
            ),
        )
    }
    return poses


def startIKService():
    rospy.init_node("rsdk_ik_service_client")

def iksvcForLimb(limb):
    ns = "ExternalTools/" + limb + "/PositionKinematicsNode/IKService"
    iksvc = rospy.ServiceProxy(ns, SolvePositionIK)
    return iksvc,ns

def ProcessHand(iksvc,ns,timeout,handToBaxter, limb,limb_obj, data):
    #http://sdk.rethinkrobotics.com/wiki/IK_Service_-_Code_Walkthrough 
    print "hand called for : ",limb

    baxter_pos = handToBaxter(data.position)
    ikreq = SolvePositionIKRequest()
    hdr = Header(stamp=rospy.Time.now(), frame_id='base')
    pose  = poseFromPosQuatLib(hdr,limb,baxter_pos,data.orientation)
    ikreq.pose_stamp.append(pose[limb])
    try:
        print "trying"
        print ikreq
        rospy.wait_for_service(ns, timeout)
        resp = iksvc(ikreq)
        if (resp.isValid[0]):
            print "SUCCESS - Valid Joint Solution Found:"
            # Format solution into Limb API-compatible dictionary
            limb_joints = dict(zip(resp.joints[0].name, resp.joints[0].position))
            print limb_joints
            limb_obj.move_to_joint_positions(limb_joints)
        else:
            print "ERROR - No valid Join Solution:",limb
            print resp
        #if msg.position[0]>10: limb_obj.move_to_joint_positions(wave_1)
        #else: limb_obj.move_to_joint_positions(wave_2)

            

    except (rospy.ServiceException, rospy.ROSException), e:
        print "except"
        rospy.logerr("Service call failed: %s" % (e,))
    
    
def ProcessHead(Head,OculusToAngle,data):
    ang = OculusToAngle(data.orientation)
    Head.set_pan(ang)

def ProcessTrigger(arduino,data):
    if data.data: arduino.trigger()

def main():
    """BaxterInterface
 
   A program from MIT's DRL for using Baxter via LCM messages
 
   Run this by passing the *limb*,head, or trigger, and the node 
   for controlling that part will spin up
   """
    arg_fmt = argparse.RawDescriptionHelpFormatter
    parser = argparse.ArgumentParser(formatter_class=arg_fmt,
                                     description=main.__doc__)
    parser.add_argument(
        '-p', '--part', choices=['left', 'right','head','trigger'], required=True,
        help="the part to control, 'left', 'right','head','trigger'"
    )
    args = parser.parse_args(rospy.myargv()[1:])
    
    print "turning on"
    rospy.init_node('BaxterLCM')
    #startIKService()
    dt = 0.05    
    scales=[0.001,0.001,0.001]# m/mm
    offsets = [600,600,600]
    mins = [0.2,0,0]
    maxs = [200,200,200]
    


    timeout =1.0
    sub_func = None
    channel = ""
    msgType = None
    if args.part == 'left':
        channel = ROS_LEFT
        handToBaxter = partial(XYZRescale,scales, offsets, mins, maxs)
        l = 'left'
        left_limb = Limb(l)
        iksvc_l,ns_l = iksvcForLimb(l)
        sub_func = partial(ProcessHand,iksvc_l,ns_l,timeout,handToBaxter, l,left_limb)
        msgType = Pose   
         
    elif args.part == 'right':
        channel = ROS_RIGHT
        handToBaxter = partial(XYZRescale,scales, offsets, mins, maxs)
        r = 'right'
        right_limb = Limb(r)
        iksvc_r = iksvcForLimb(r)
        sub_func = partial(ProcessHand,thresh,arduino,iksvc_l,timeout,handToBaxter, r,right_limb)
        msgType = Pose     
         
    elif args.part == 'head':
        theta_max = pi
        theta_min = -pi
    
        OculusToAngle = partial(YawFromQuat,theta_min,theta_max)

        head = Head()
        channel = ROS_HEAD
        sub_func = partial(ProcessHead,head,OculusToAngle)
        msgType = Pose  

    elif args.part == 'trigger':
        channel = ROS_TRIGGER
        arduino = ArduinoInterface()
        sub_func = partial(ProcessTrigger,arduino)
        msgType = Bool

    else :
        print "unknown part:", args.part
        return 0
     

    rospy.init_node('part_listener', anonymous=True)
    #Start movement
    print "starting part: ",args.part
    rs = RobotEnable(CHECK_VERSION)
    rospy.Subscriber(channel, msgType, sub_func)
    rospy.spin()

    print "done"
    #rs.disable()
    return 0
        


if __name__ == "__main__":
    sys.exit(int(main() or 0))

