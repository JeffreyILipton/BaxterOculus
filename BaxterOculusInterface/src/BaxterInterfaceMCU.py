
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



def minMax(min_val,max_val,val):
    return max(min_val,min(val,max_val))

def XYZRescale(scales, offsets, mins, maxs, xyz):
    return [ minMax(mins[i],maxs[i], scales[i]*(xyz[i]+offsets[i])) for i in range(0,3)]



def QuatForInverse(quat):
    tBU = np.mat([ [0,-1,0], [0,0,1], [1,0,0]])
    theta = pi
    A = np.mat([ [cos(theta),0,-sin(theta)], [0,1,0], [sin(theta),0,cos(theta)]])
    print "\nA:"
    print A
    qHB = quat
    tHB = tFromQ(qHB)
    tHU = tHB.dot(A).dot(tBU)
    Hx = tHU[:,0]
    Hy = tHU[:,1]
    Hz = tHU[:,2]
    Oz = Hz
    Oy = -Hy
    Ox = mCross(Oy,Oz)
    tOU = np.concatenate((Ox,Oy,Oz), axis=1)

    Q3 = qFromT(tOU)
    return Q3
    # tUB = np.mat([ [0,-1,0], [0,0,1], [1,0,0]])
    # qBH = quat
    # tBH = tFromQ(qBH)
    # tUH = tUB.dot(tBH)
    # Hx = tUH[:,0]
    # Hy = tUH[:,1]
    # Hz = tUH[:,2]
    # Oz = Hz
    # Oy = -Hy
    # Ox = mCross(Oy,Oz)
    # tOU = np.concatenate((Ox,Oy,Oz), axis=1)

    # Q3 = qFromT(tOU)
    # return Q3

def QuatTransform(quat):
    tBU = np.mat([ [0,0,1], [-1,0,0], [0,1,0] ])
    #quat= [1,0,0,0]#[cos(pi/4.0),0,cos(pi/4.0),0]
    qUO = quat#qInv(quat)
    tUO = tFromQ(qUO)
    tBO = tBU.dot(tUO)
    Ox = tBO[:,0]
    Oy = tBO[:,1]
    Oz = tBO[:,2]
    Hz = Oz
    Hy = -Oy
    Hx = mCross(Hy,Hz) #-Ox
    tHB = np.concatenate((Hx,Hy,Hz), axis=1)

    Q3 = qFromT(tHB)
    global DEBUG
    if DEBUG:
        print "qUO: ",qUO
        print "tUO: \n",tUO
        print "TBO: \n",tBO
        print "THB: \n",tHB
        print "Oy:\n",Oy
        print "Hy:\n",Hy
        print "qHB: ",Q3

    return Q3 


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


def ProcessGripperVel(gripper,data):
    gripper.set_velocity(data.data)


def ProcessTriggerCMDAsGripper(gripper,data):
    #print "gripper:",data.data
    if data.data == True:
        gripper.open()
    else:
        gripper.close()

def ProcessGripperCMD(gripper,data):
    print "gripper:",data.data
    if data.data <1:
        gripper.open()
    elif data.data<2:
        gripper.close()
    elif data.data<3:
        gripper.stop()

def iksvcForLimb(limb):
    ns = "ExternalTools/" + limb + "/PositionKinematicsNode/IKService"
    iksvc = rospy.ServiceProxy(ns, SolvePositionIK)
    return iksvc,ns

def ProcessHand(lc,lcChannel,lcPosChannel,iksvc,ns,timeout,handToBaxter, limb,limb_obj, data):
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
    lcm_msg = trigger_t()

    try:
        print "\ntarget:", baxter_pos,"\n\t",orientation
        #print ikreq
        #rospy.wait_for_service(ns, timeout)
        resp = ServiceTimeouter(timeout,iksvc, ikreq).call()
        if (resp is not None and resp.isValid[0]):
            print "SUCCESS - Valid Joint Solution Found:"
            # Format solution into Limb API-compatible dictionary
            limb_joints = dict(zip(resp.joints[0].name, resp.joints[0].position))
            lcm_msg.trigger = True
            lc.publish(lcChannel,lcm_msg.encode())
            #print limb_joints
            limb_obj.move_to_joint_positions(limb_joints)
        else:
            print "ERROR - No valid Joint Solution:",limb
            lcm_msg.trigger = False
            lc.publish(lcChannel,lcm_msg.encode())
            
            #print resp
        #if msg.position[0]>10: limb_obj.move_to_joint_positions(wave_1)
        #else: limb_obj.move_to_joint_positions(wave_2)

            

    except (rospy.ServiceException, rospy.ROSException), e:
        print "except"
        lcm_msg.trigger = False
        rospy.logerr("Service call failed: %s" % (e,))
        lc.publish(lcChannel,lcm_msg.encode())
    
    # return endpoint
    newpose = limb_obj.endpoint_pose()
    xyz = newpose['position']
    orient = newpose['orientation']
    quat = [orient[3],orient[0],orient[1],orient[2]]
    quat = QuatForInverse(quat)
    lcm_pos_msg = pose_t()
    lcm_pos_msg.position = list(xyz)
    lcm_pos_msg.orientation = quat
    lc.publish(lcPosChannel,lcm_pos_msg.encode())
    
    
def ProcessHead(Head,OculusToAngle,data):
    ang = OculusToAngle(data.orientation)
    Head.set_pan(ang)

def ProcessRange(lc,lcChannel,data):
    global curtime 
    global RANGE_TIME
    #print "tick"
    if time.time()-curtime  >RANGE_TIME:
        msg = range_t()
        msg.range = data.range
        lc.publish(lcChannel,msg.encode())
        curtime  = time.time()

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

    if part == 'left':
        channel = ROS_LEFT
        handToBaxter = partial(XYZRescale,scales, offsets, mins, maxs)
        l = 'left'
        left_limb = Limb(l)

        iksvc_l,ns_l = iksvcForLimb(l)

        lcChannel = LCM_LEFT_VALID
        

        sub_func = partial(ProcessHand,lc,lcChannel,LCM_LEFT_CURRNEPOS, iksvc_l,ns_l,timeout,handToBaxter, l,left_limb)
        msgType = Pose
        connection_list.append((channel,msgType,sub_func))   
         
    elif part == 'right':
        channel = ROS_RIGHT
        handToBaxter = partial(XYZRescale,scales, offsets, mins, maxs)
        r = 'right'
        right_limb = Limb(r)
        #right_limb.set_position_speed(1)
        iksvc_r,ns_r = iksvcForLimb(r)
        
        lcChannel = LCM_RIGHT_VALID

        sub_func = partial(ProcessHand,lc,lcChannel,LCM_RIGHT_CURRNEPOS,iksvc_r,ns_r,timeout,handToBaxter, r,right_limb)
        msgType = Pose   
        connection_list.append((channel,msgType,sub_func))  
         
    elif part == 'head':
        theta_max = pi
        theta_min = -pi
    
        OculusToAngle = partial(YawFromQuat,theta_min,theta_max)

        head = Head()
        channel = ROS_HEAD
        sub_func = partial(ProcessHead,head,OculusToAngle)
        msgType = Pose  
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

        r_cam = CameraController('right_hand_camera')
        r_cam.resolution=(320,200)#(1280, 800)
        r_cam.open()


    elif part == 'left_camera':

        lcChannel = LCM_L_CAMERA
        #lc = lcm.LCM("udpm://239.255.76.67:7667:?ttl=1")
        channel = ROS_L_CAMERA
        msgType = Image
        sub_func = lambda x : ProcessImage(lc,lcChannel,x)
        #sub_func = partial(ProcessImage,lc,lcChannel)
        connection_list.append((channel,msgType,sub_func))
        connection_list.append((channel,msgType,sub_func))

        l_cam = CameraController('left_hand_camera')
        l_cam.resolution=(320,200)#(1280, 800)
        l_cam.open()
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



