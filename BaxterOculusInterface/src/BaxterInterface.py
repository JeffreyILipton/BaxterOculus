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
import time
import numpy as np

curtime = time.time()
print "time:", curtime

def minMax(min_val,max_val,val):
    return max(min_val,min(val,max_val))

def XYZRescale(scales, offsets, mins, maxs, xyz):
    return [ minMax(mins[i],maxs[i], scales[i]*(xyz[i]+offsets[i])) for i in range(0,3)]

def YawFromQuat(theta_min,theta_max, quat):
    # https://en.wikipedia.org/wiki/Conversion_between_quaternions_and_Euler_angles
    q0,q1,q2,q3 = quat
    return atan2(2*(q0*q1+q2*q3),1-2*(q1*q1+q2*q2))
def magQ(q):
    return sqrt(q[0]*q[0] + q[1]*q[1] + q[2]*q[2] + q[3]*q[3])

def qFromT(m):
    tr = m[0,0]+m[1,1]+m[2,2]
    if(tr>0):
        print "c1"
        s = 2*sqrt(tr+1.0)
        print "s:",s
        w = 0.25*s
        x = (m[2,1] - m[1,2] )/s
        y = (m[0,2] - m[2,0] )/s
        z = (m[1,0] - m[0,1] )/s

    elif ( (m[0,0]>m[1,1]) and ( m[0,0]>m[2,2])):
        print "c2"
        s = sqrt(1+m[0,0]-m[1,1]-m[2,2])*2
        w = ( m[2,1] - m[1,2] )/s
        x = 0.25*s
        y = ( m[0,1] + m[1,0] )/s
        z = ( m[0,2] + m[2,0] )/s

    elif ( m[1,1]>m[2,2] ):
        print "c3"
        s = sqrt(1+m[1,1]-m[0,0]-m[2,2])*2
        w = ( m[0,2] - m[2,0] )/s
        x = ( m[0,1] + m[1,0] )/s
        y = 0.25*s
        z = ( m[1,2] + m[2,1] )/s

    else:
        print "c4"
        s = sqrt(1.0 + m[2,2] - m[0,0] - m[1,1])*2
        w = (m[1,0] - m[0,1])/s
        x = (m[0,2] + m[2,0])/s
        y = (m[1,2] + m[2,1])/s
        z = 0.25*s

    mag = magQ([w,x,y,z])
    if mag<0.9 or mag>1.1: print "Mag error:",mag
    #w=w/mag
    #x=x/mag
    #y=y/mag
    #z=z/mag
    return [w,x,y,z]
def tFromQ(q):
    t = np.mat(np.zeros((3,3)))
    w,x,y,z = q
    t[0,0] = 1 - 2*y*y - 2*z*z
    t[0,1] = 0 + 2*x*y - 2*w*z
    t[0,2] = 0 + 2*x*z + 2*w*y
    t[1,0] = 0 + 2*x*y + 2*w*z
    t[1,1] = 1 - 2*x*x - 2*z*z
    t[1,2] = 0 + 2*y*z - 2*w*x
    t[2,0] = 0 + 2*x*z - 2*w*y
    t[2,1] = 0 + 2*y*z + 2*w*x
    t[2,2] = 1 - 2*x*x - 2*y*y
    return t


def qInv(q):
    return [q[0],-q[1],-q[2],-q[3]]
def qMult(q,r):
    t=[0,0,0,0]
    t[0] = r[0]*q[0]-r[1]*q[1]-r[2]*q[2]-r[3]*q[3]
    t[1] = r[0]*q[1]+r[1]*q[0]-r[2]*q[3]+r[3]*q[2]
    t[2] = r[0]*q[2]+r[1]*q[3]+r[2]*q[0]-r[3]*q[1]
    t[3] = r[0]*q[3]-r[1]*q[2]+r[2]*q[1]+r[3]*q[0]
    return t

def QuatTransform(quat):
    tBU = np.mat([ [0,0,1], [-1,0,0], [0,1,0] ])
    #quat= [1,0,0,0]#[cos(pi/4.0),0,cos(pi/4.0),0]
    qUO = qInv(quat)
    tUO = tFromQ(qUO)
    tBO = tBU.dot(tUO)
    Ox = tBO[:,0]
    Oy = tBO[:,1]
    Oz = tBO[:,2]
    Hz = Oz
    Hx = -Oy
    Hy = -Ox#-Ox
    tHB = np.concatenate((Hx,Hy,Hz), axis=1)
    ang = pi/2
    w90 = cos(ang/2)
    z90 = sin(ang/2)
    #tHP = tFromQ([w90,0,0,z90])
    #tHPB = tHP.dot(tHB)
    Q3 = qFromT(tHB)
    print "qUO: ",qUO
    print "tUO: \n",tUO
    print "TBO: \n",tBO
    print "THB: \n",tHB
    #print "THPB \n",tHPB
    print "qHB: ",Q3

    #Trans = cos(pi/4)*Trans
    #Q = np.mat(quat).uT
    #Q2 = Trans.dot(Q)
    #Q3 =  Q2.T.tolist()[0]
    #qZ = [cos(pi/4),0,0,0]
    #qZInv = qInv(qZ)
    #qBU = [0.5,0.5,0.5,-0.5]
    #qBUInv = qInv(qBU)
    #Q3 = qMult(qBU, qMult(quat,qBUInv))
    return Q3 #[Q3[0],Q3[3],Q3[2],-Q3[1]]


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

def ProcessGripperCMD(gripper,data):
    #print "gripper:",data.data
    if data.data <1:
        gripper.stop()
    elif data.data<2:
        gripper.open()
    elif data.data<3:
        gripper.close()

def iksvcForLimb(limb):
    ns = "ExternalTools/" + limb + "/PositionKinematicsNode/IKService"
    iksvc = rospy.ServiceProxy(ns, SolvePositionIK)
    return iksvc,ns

def ProcessHand(iksvc,ns,timeout,handToBaxter, limb,limb_obj, data):
    #http://sdk.rethinkrobotics.com/wiki/IK_Service_-_Code_Walkthrough 
    print "\n\nhand called for : ",limb
    position = [data.position.x,data.position.y,data.position.z]
    orientation = [data.orientation.w,data.orientation.x,data.orientation.y,data.orientation.z] 
    print "  Old Q:",orientation
    orientation = QuatTransform(orientation)
    print "  New Q:",orientation


    baxter_pos = handToBaxter(position)
    ikreq = SolvePositionIKRequest()
    hdr = Header(stamp=rospy.Time.now(), frame_id='base')
    pose  = poseFromPosQuatLib(hdr,limb,baxter_pos,orientation)
    ikreq.pose_stamp.append(pose[limb])
    try:
        print "\ntarget:", baxter_pos,"\n\t",orientation
        #print ikreq
        #rospy.wait_for_service(ns, timeout)
        resp = ServiceTimeouter(timeout,iksvc, ikreq).call()
        if (resp is not None and resp.isValid[0]):
            print "SUCCESS - Valid Joint Solution Found:"
            # Format solution into Limb API-compatible dictionary
            limb_joints = dict(zip(resp.joints[0].name, resp.joints[0].position))
            #print limb_joints
            limb_obj.move_to_joint_positions(limb_joints)
        else:
            print "ERROR - No valid Joint Solution:",limb
            
            #print resp
        #if msg.position[0]>10: limb_obj.move_to_joint_positions(wave_1)
        #else: limb_obj.move_to_joint_positions(wave_2)

            

    except (rospy.ServiceException, rospy.ROSException), e:
        print "except"
        rospy.logerr("Service call failed: %s" % (e,))
    
    
def ProcessHead(Head,OculusToAngle,data):
    ang = OculusToAngle(data.orientation)
    Head.set_pan(ang)

def ProcessTrigger(arduino,data):
    if data.data: arduino.trigger(True)
    else: arduino.trigger(False)


def ProcessRange(lc,lcChannel,data):
    msg = range_t()
    msg.range = data.range
    lc.publish(lcChannel,msg.encode())

def ProcessImage(lc,lcChannel,rosmsg):
    global curtime
    #print "tick"
    if time.time()-curtime >0.05:
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
    offsets = [0.5,00,0.0]
    mins = [0.2,0,0]
    maxs = [200,200,200]
    


    timeout =0.5
    sub_func = None
    channel = ""
    msgType = None

    connection_list = []

    if part == 'left':
        channel = ROS_LEFT
        handToBaxter = partial(XYZRescale,scales, offsets, mins, maxs)
        l = 'left'
        left_limb = Limb(l)
        iksvc_l,ns_l = iksvcForLimb(l)
        sub_func = partial(ProcessHand,iksvc_l,ns_l,timeout,handToBaxter, l,left_limb)
        msgType = Pose
        connection_list.append((channel,msgType,sub_func))   
         
    elif part == 'right':
        channel = ROS_RIGHT
        handToBaxter = partial(XYZRescale,scales, offsets, mins, maxs)
        r = 'right'
        right_limb = Limb(r)
        iksvc_r,ns_r = iksvcForLimb(r)
        sub_func = partial(ProcessHand,iksvc_r,ns_r,timeout,handToBaxter, r,right_limb)
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

    elif part == 'right_trigger':
        channel = ROS_R_TRIGGER
        arduino = ArduinoInterface("/dev/ttyACM0")
        sub_func = partial(ProcessTrigger,arduino)
        msgType = Bool
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
        lc = lcm.LCM("udpm://239.255.76.67:7667:?ttl=1")
        channel = ROS_L_RANGE
        msgType = Range
        sub_func = partial(ProcessRange,lc,lcChannel)
        connection_list.append((channel,msgType,sub_func))

    elif part == 'right_range':
        lcChannel = LCM_R_RANGE
        lc = lcm.LCM("udpm://239.255.76.67:7667:?ttl=1")
        channel = ROS_R_RANGE
        msgType = Range
        sub_func = partial(ProcessRange,lc,lcChannel)
        connection_list.append((channel,msgType,sub_func))

    elif part == 'right_camera':
        lcChannel = LCM_R_CAMERA
        lc = lcm.LCM("udpm://239.255.76.67:7667:?ttl=1")
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
        lc = lcm.LCM("udpm://239.255.76.67:7667:?ttl=1")
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



