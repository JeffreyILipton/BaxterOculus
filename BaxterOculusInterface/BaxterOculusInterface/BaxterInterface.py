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
from ServiceTimeout import *



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


def iksvcForLimb(limb):
    ns = "ExternalTools/" + limb + "/PositionKinematicsNode/IKService"
    iksvc = rospy.ServiceProxy(ns, SolvePositionIK)
    return iksvc,ns

def ProcessHand(iksvc,ns,timeout,handToBaxter, limb,limb_obj, data):
    #http://sdk.rethinkrobotics.com/wiki/IK_Service_-_Code_Walkthrough 
    print "hand called for : ",limb
    position = [data.position.x,data.position.y,data.position.z]
    orientation = [data.orientation.x,data.orientation.y,data.orientation.z,data.orientation.w] 

    baxter_pos = handToBaxter(position)
    ikreq = SolvePositionIKRequest()
    hdr = Header(stamp=rospy.Time.now(), frame_id='base')
    pose  = poseFromPosQuatLib(hdr,limb,baxter_pos,orientation)
    ikreq.pose_stamp.append(pose[limb])
    try:
        print "trying"
        #print ikreq
        #rospy.wait_for_service(ns, timeout)
        resp = ServiceTimeouter(timeout,iksvc, ikreq).call()
        if (resp is resp.isValid[0]):
            print "SUCCESS - Valid Joint Solution Found:"
            # Format solution into Limb API-compatible dictionary
            limb_joints = dict(zip(resp.joints[0].name, resp.joints[0].position))
            print limb_joints
            limb_obj.move_to_joint_positions(limb_joints)
        else:
            print "ERROR - No valid Join Solution:",limb
            print "target:", baxter_pos," ",orientation
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
    #arg_fmt = argparse.RawDescriptionHelpFormatter
    #parser = argparse.ArgumentParser(formatter_class=arg_fmt,
    #                                 description=main.__doc__)
    #parser.add_argument(
    #    '-p', '--part', choices=['left', 'right','head','trigger'], required=True,
    #    help="the part to control, 'left', 'right','head','trigger'"
    #)
    #args = parser.parse_args(rospy.myargv()[1:])
    rospy.init_node('part_listener', anonymous=True)
    full_param_name = rospy.search_param('part')
    param_value = rospy.get_param(full_param_name)
    part = param_value
    print "PART IS:",part


    scales=[0.001,0.001,0.001]# m/mm
    offsets = [600,00,300]
    mins = [0.2,0,0]
    maxs = [200,200,200]
    


    timeout =0.5
    sub_func = None
    channel = ""
    msgType = None
    if part == 'left':
        channel = ROS_LEFT
        handToBaxter = partial(XYZRescale,scales, offsets, mins, maxs)
        l = 'left'
        left_limb = Limb(l)
        iksvc_l,ns_l = iksvcForLimb(l)
        sub_func = partial(ProcessHand,iksvc_l,ns_l,timeout,handToBaxter, l,left_limb)
        msgType = Pose   
         
    elif part == 'right':
        channel = ROS_RIGHT
        handToBaxter = partial(XYZRescale,scales, offsets, mins, maxs)
        r = 'right'
        right_limb = Limb(r)
        iksvc_r,ns_r = iksvcForLimb(r)
        sub_func = partial(ProcessHand,iksvc_r,ns_r,timeout,handToBaxter, r,right)
        msgType = Pose     
         
    elif part == 'head':
        theta_max = pi
        theta_min = -pi
    
        OculusToAngle = partial(YawFromQuat,theta_min,theta_max)

        head = Head()
        channel = ROS_HEAD
        sub_func = partial(ProcessHead,head,OculusToAngle)
        msgType = Pose  

    elif part == 'trigger':
        channel = ROS_TRIGGER
        arduino = ArduinoInterface()
        sub_func = partial(ProcessTrigger,arduino)
        msgType = Bool

    else :
        print "unknown part:", part
        return 0
     

    
    #Start movement
    print "starting part: ",part
    rs = RobotEnable(CHECK_VERSION)
    rospy.Subscriber(channel, msgType, sub_func)
    rospy.spin()

    print "done"
    #rs.disable()
    return 0
        


if __name__ == "__main__":
    sys.exit(int(main() or 0))



