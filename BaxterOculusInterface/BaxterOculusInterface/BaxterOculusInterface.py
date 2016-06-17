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
from oculuslcm import *
from Threadsys import *

def minMax(min_val,max_val,val):
    return max(min_val,min(val,max_val))

def XYZRescale(scales, offsets, mins, maxs, xyz):
    return [ minMax(mins[i],maxs[i], scales[i]*(xyz[i]+offsets[i])) for i in range(0,3)]

def YawFromQuat(theta_min,theta_max, quat):
    # https://en.wikipedia.org/wiki/Conversion_between_quaternions_and_Euler_angles
    q0,q1,q2,q3 = quat
    return atan2(2*(q0*q1+q2*q3),1-2*(q1*q1+q2*q2))

wave_1 = {'left_s0': -0.459, 'left_s1': -0.202, 'left_e0': 1.807, 'left_e1': 1.714, 'left_w0': -0.906, 'left_w1': -1.545, 'left_w2': -0.276}
wave_2 = {'left_s0': -0.395, 'left_s1': -0.202, 'left_e0': 1.831, 'left_e1': 1.981, 'left_w0': -1.979, 'left_w1': -1.100, 'left_w2': -0.448}


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
                    x=orientation[1],
                    y=orientation[2],
                    z=orientation[3],
                    w=orientation[0],
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
    return iksvc

def ProcessHand(thresh,arduino,iksvc,timeout,handToBaxter, limb,limb_obj, data):
    #http://sdk.rethinkrobotics.com/wiki/IK_Service_-_Code_Walkthrough 
    print "hand called for : ",limb
    msg = hand_t.decode(data)
    baxter_pos = handToBaxter(msg.position)
    ns = "ExternalTools/" + limb + "/PositionKinematicsNode/IKService"
    ikreq = SolvePositionIKRequest()
    hdr = Header(stamp=rospy.Time.now(), frame_id='base')
    pose  = poseFromPosQuatLib(hdr,limb,baxter_pos,msg.orientation)
    ikreq.pose_stamp.append(pose[limb])
    try:
        print "trying"
        print ikreq
        #rospy.wait_for_service(ns, timeout)
        #resp = iksvc(ikreq)
        #if (resp.isValid[0]):
        #    print "SUCCESS - Valid Joint Solution Found:"
        #    # Format solution into Limb API-compatible dictionary
        #    limb_joints = dict(zip(resp.joints[0].name, resp.joints[0].position))
        #    print limb_joints
        #    limb_obj.move_to_joint_positions(limb_joints)
        #else:
        if msg.position[0]>10: limb_obj.move_to_joint_positions(wave_1)
        else: limb_obj.move_to_joint_positions(wave_2)
        print "huh"
        #print resp
            

    except (rospy.ServiceException, rospy.ROSException), e:
        print "except"
        rospy.logerr("Service call failed: %s" % (e,))


    
    if (msg.trigger > thresh) and (type(arduino) != None) :
        arduino.trigger()
    
    
def ProcessHead(Head,OculusToAngle,data):
    msg = oculus_t.decode(data)
    ang = OculusToAngle(msg.orientation)
    Head.set_pan(ang)


def setupBaxterPart(dt,channel,processor):
    l_channel = channel
    l_lock = Lock()
    l_holder = MessageHolder(l_lock,'')
    l_Controller = BaxterPartInterface(dt,l_holder,processor)
    return l_holder,l_Controller

def main():
    print "turning on"
    rospy.init_node('BaxterLCM')
    #startIKService()
    dt = 0.05    
    scales=[1,1,1]
    offsets = [0,0,0]
    mins = [0,0,0]
    maxs = [200,200,200]
    handToBaxter = partial(XYZRescale,scales, offsets, mins, maxs)



    thresh=100
    arduino=None
    timeout =1.0
    l = 'left'
    left_limb = Limb(l)
    iksvc_l = iksvcForLimb(l)
    LeftHand = partial(ProcessHand,thresh,arduino,iksvc_l,timeout,handToBaxter, l,left_limb)    

    #r = 'right'
    #right_limb = Limb(r)
    #iksvc_r = iksvcForLimb(r)
    #RightHand = partial(ProcessHand,thresh,arduino,iksvc_l,timeout,handToBaxter, r,right_limb)    


    #theta_max = pi
    #theta_min = -pi
    
    #OculusToAngle = partial(YawFromQuat,theta_min,theta_max)

    #head = Head()
    #HeadControl = partial(ProcessHead,head,OculusToAngle)

    print "parts made, setting up interface"
    lc = lcm.LCM()
    #r_subscription = lc.subscribe("Right", RightHand)
    #l_subscription = lc.subscribe("Left",LeftHand)
    #h_subscription = lc.subscribe("Head",HeadControl)
    l_holder, l_Controller = setupBaxterPart(dt,"Left",LeftHand)
    #r_holder, r_Controller = setupBaxterPart(dt,"Right",RightHand)
    #h_holder, h_Controller = setupBaxterPart(dt,"Head",HeadControl)
    channel_state_holder_dict={}
    channel_state_holder_dict['left']  = l_holder
    #channel_state_holder_dict['right'] = r_holder
    #channel_state_holder_dict['head']  = h_holder
    lcmInter = LCMInterface(lc,channel_state_holder_dict)
    threads = [lcmInter,l_Controller]#,r_LCM, r_Controller,h_LCM, h_Controller]

    
    #Start movement
    rs = RobotEnable(CHECK_VERSION)

    print "starting"
    for thread in threads: thread.start()
    for thread in threads: thread.join()
    print "done"
    #rs.disable()
    return 0
        


if __name__ == "__main__":
    sys.exit(int(main() or 0))
