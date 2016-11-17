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
    from sensor_msgs.msg import Range, Image, JointState
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

def minMax(min_val,max_val,val):
    return max(min_val,min(val,max_val))

def XYZRescale(scales, offsets, mins, maxs, xyz):
    return [ minMax(mins[i],maxs[i], scales[i]*(xyz[i]+offsets[i])) for i in range(0,3)]

def posFromPointAndList(Point,orientation):
    pose=Pose(
                position=Point,
                orientation=Quaternion(
                    x=orientation[1],
                    y=orientation[2],
                    z=orientation[3],
                    w=orientation[0],
                ),
            )
    return pose

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

##### Uses Float64
def ProcessGripperVel(gripper,rosmsg):
    gripper.set_velocity(rosmsg.data)

## used Bool
def ProcessTriggerCMDAsGripper(gripper,rosmsg):
    #print "gripper:",data.data
    if rosmsg.data == True:
        gripper.open()
    else:
        gripper.close()

### uses Pose msg    
def ProcessHead(Head,OculusToAngle,rosmg):
    ang = OculusToAngle(data.orientation)
    Head.set_pan(ang)

### uses Bool
def ProcessTrigger(arduino,rosmsg):
    if rosmsg.data: arduino.trigger(True)
    else: arduino.trigger(False)

# uses uint16
def ProcessGripperCMD(gripper,rosmsg):
    #print "gripper:",data.data
    if rosmsg.data <1:
        gripper.stop()
    elif rosmsg.data<2:
        gripper.open()
    elif rosmsg.data<3:
        gripper.close()

def iksvcForLimb(limb):
    ns = "ExternalTools/" + limb + "/PositionKinematicsNode/IKService"
    iksvc = rospy.ServiceProxy(ns, SolvePositionIK)
    return iksvc,ns


def ProcessLimbCommands(PosePub,limb_obj,resp):
    limb_joints = dict(zip(resp.name, resp.position))
    limb_obj.move_to_joint_positions(limb_joints)

    newpose = limb_obj.endpoint_pose()
    #print newpose
    xyz = newpose['position']
    orient = newpose['orientation']
    quat = [orient[3],orient[0],orient[1],orient[2]]
    quat = QuatForInverse(quat)
    hdr = Header(stamp=rospy.Time.now(), frame_id='base')
    
    posemsg = posFromPointAndList(xyz,quat)
    PosePub.publish(posemsg)


##### Uses Pose
def ProcessHand(IsValidPub,ResPub,iksvc,ns,timeout,handToBaxter, limb,limb_obj, data):
    #http://sdk.rethinkrobotics.com/wiki/IK_Service_-_Code_Walkthrough 
    print "\n\nhand called for : ",limb
    position = [data.position.x,data.position.y,data.position.z]
    orientation = [data.orientation.w,data.orientation.x,data.orientation.y,data.orientation.z] 
    #print "  Old Q:",orientation
    orientation = QuatTransform(orientation)
    #print "  New Q:",orientation


    baxter_pos = handToBaxter(position)
    ikreq = SolvePositionIKRequest()
    hdr = Header(stamp=rospy.Time.now(), frame_id='base')
    pose  = poseFromPosQuatLib(hdr,limb,baxter_pos,orientation)
    ikreq.pose_stamp.append(pose[limb])
    isvalid_msg = Bool()
    isvalid_msg.data = False
    
    try:
        print "\ntarget:", baxter_pos,"\n\t",orientation
        #print ikreq
        #rospy.wait_for_service(ns, timeout)
        resp = ServiceTimeouter(timeout,iksvc, ikreq).call()
        if (resp is not None and resp.isValid[0]):
            print "SUCCESS - Valid Joint Solution Found:"
            # Format solution into Limb API-compatible dictionary
            ##limb_joints = dict(zip(resp.joints[0].name, resp.joints[0].position))
            isvalid_msg.data = True
            #IsValidPub.publish(isvalid_msg)
            #print limb_joints
            ##limb_obj.move_to_joint_positions(limb_joints)
            ##print "response"
            #print type(resp)
            #print resp

            ResPub.publish(resp.joints[0])

        else:
            print "ERROR - No valid Joint Solution:",limb
            #isvalid_msg.data = False
            #IsValidPub.publish(isvalid_msg)
            
            #print resp
        #if msg.position[0]>10: limb_obj.move_to_joint_positions(wave_1)
        #else: limb_obj.move_to_joint_positions(wave_2)

            

    except (rospy.ServiceException, rospy.ROSException), e:
        print "except"
        #isvalid_msg.data = False
        rospy.logerr("Service call failed: %s" % (e,))
        #IsValidPub.publish(isvalid_msg)
    
    IsValidPub.publish(isvalid_msg)
    
    # return endpoint


    






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
    #    '-p', '--part', choices=['left', 'right','head','right_trigger','right_gripper','left_gripper'], required=True,
    #    help="the part to control, 'left', 'right','head','right_trigger','right_gripper','left_gripper'"
    #)
    #args = parser.parse_args(rospy.myargv()[1:])
    rospy.init_node('PartControls', anonymous=True)
    full_param_name = rospy.search_param('part')
    param_value = rospy.get_param(full_param_name)
    part = param_value
    #print "PART IS:",part


    scales=[1.00,1.00,1.00]# m/mm
    offsets = [0,00,0.0]
    mins = [-5,-5,-5]
    maxs = [5,5,5]
    


    timeout =0.1
    sub_func = None
    channel = ""
    msgType = None

    connection_list = []

    ardPort = "/dev/ttyACM1"

    if part == 'left':
        channel = ROS_LEFT
        handToBaxter = partial(XYZRescale,scales, offsets, mins, maxs)
        l = 'left'
        left_limb = Limb(l)

        iksvc_l,ns_l = iksvcForLimb(l)
        
        IsValidPub = rospy.Publisher(ROS_LEFT_VALID_STATE, Bool, queue_size=10,latch=True)
        PosePub  = rospy.Publisher(ROS_LEFT_CURRENTPOS_STATE, Pose, queue_size=10,latch=True)
        ResPub   = rospy.Publisher(ROS_LEFT_CMD_STATE, JointState, queue_size=2,latch=True)



        sub_func = partial(ProcessHand,IsValidPub,ResPub, iksvc_l, ns_l, timeout, handToBaxter, l, left_limb)
        msgType = Pose
        connection_list.append((channel,msgType,sub_func)) 



        channel = ROS_LEFT_CMD_STATE
        sub_func = partial(ProcessLimbCommands,PosePub,left_limb)
        msgType = JointState
        connection_list.append((channel,msgType,sub_func)) 

        l_cam = CameraController('left_hand_camera')
        l_cam.resolution=(320,200)#(1280, 800)
        l_cam.open()  
         
    elif part == 'right':
        channel = ROS_RIGHT
        handToBaxter = partial(XYZRescale,scales, offsets, mins, maxs)
        r = 'right'
        right_limb = Limb(r)

        iksvc_r,ns_r = iksvcForLimb(r)
        
        IsValidPub = rospy.Publisher(ROS_RIGHT_VALID_STATE, Bool, queue_size=10,latch=True)
        PosePub  = rospy.Publisher(ROS_RIGHT_CURRENTPOS_STATE, Pose, queue_size=10,latch=True)
        ResPub   = rospy.Publisher(ROS_RIGHT_CMD_STATE, JointState, queue_size=2,latch=True)

        sub_func = partial(ProcessHand,IsValidPub,ResPub, iksvc_r,ns_r,timeout,handToBaxter, r,right_limb)
        msgType = Pose   
        connection_list.append((channel,msgType,sub_func))  

        channel = ROS_RIGHT_CMD_STATE
        sub_func = partial(ProcessLimbCommands,PosePub,right_limb)
        msgType = JointState
        connection_list.append((channel,msgType,sub_func)) 


        r_cam = CameraController('right_hand_camera')
        r_cam.resolution=(320,200)#(1280, 800)
        r_cam.open()
         
    elif part == 'head':
        theta_max = pi
        theta_min = -pi
    
        OculusToAngle = partial(YawFromQuat,theta_min,theta_max)

        head = Head()
        channel = ROS_HEAD
        sub_func = partial(ProcessHead,head,OculusToAngle)
        msgType = Pose  
        connection_list.append((channel,msgType,sub_func))

    elif part == 'right_trigger':
        channel = ROS_R_TRIGGER
        arduino = ArduinoInterface(ardPort)
        sub_func = partial(ProcessTrigger,arduino)
        msgType = Bool
        connection_list.append((channel,msgType,sub_func))

    elif part == 'right_trigger_gripper':
        gripper = Gripper('right', CHECK_VERSION)
        gripper.calibrate()

        channel = ROS_R_TRIGGER
        msgType = Bool
        sub_func = partial(ProcessTriggerCMDAsGripper,gripper)
        connection_list.append((channel,msgType,sub_func))

    elif part == 'right_gripper':
        gripper = Gripper('right', CHECK_VERSION)
        gripper.calibrate()
        channel = ROS_R_CMD
        msgType = UInt16
        sub_func = partial(ProcessGripperCMD,gripper)
        connection_list.append((channel,msgType,sub_func))

        channel = ROS_R_VEL
        msgType = Float64
        sub_func = partial(ProcessGripperVel,gripper)
        connection_list.append((channel,msgType,sub_func))

    elif part == 'left_gripper':
        gripper = Gripper('left', CHECK_VERSION)
        gripper.calibrate()
        channel = ROS_L_CMD
        msgType = UInt16
        sub_func = partial(ProcessGripperCMD,gripper)
        connection_list.append((channel,msgType,sub_func))

        channel = ROS_L_VEL
        msgType = Float64
        sub_func = partial(ProcessGripperVel,gripper)
        connection_list.append((channel,msgType,sub_func))
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



