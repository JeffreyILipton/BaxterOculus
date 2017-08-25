#!/usr/bin/env python

import sys
import copy
import rospy
import moveit_commander
import moveit_msgs.msg
from geometry_msgs.msg import *
from std_msgs.msg import *
from itertools import *
from functools import *

from std_msgs.msg import String

def moveArm(pub,group, pose_target):

  print "Received Pose"

  #pose_target = geometry_msgs.msg.Pose()

  group.set_pose_target(pose_target)

  plan1 = group.plan()
  #print plan1

  a = group.go(wait=True)
  print a
  pub.publish(a)
  #b = group.execute(plan1)
  #print b


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
    #print "PART IS:",par

    moveit_commander.roscpp_initialize(sys.argv)

    ## Instantiate a RobotCommander object.  This object is an interface to
    ## the robot as a whole.
    robot = moveit_commander.RobotCommander()

    ## Instantiate a PlanningSceneInterface object.  This object is an interface
    ## to the world surrounding the robot.
    scene = moveit_commander.PlanningSceneInterface()

    ## Instantiate a MoveGroupCommander object.  This object is an interface
    ## to one group of joints.  In this case the group is the joints in the left
    ## arm.  This interface can be used to plan and execute motions on the left
    ## arm.
    groupL = moveit_commander.MoveGroupCommander("left_arm")
    groupR = moveit_commander.MoveGroupCommander("right_arm")


    ## We create this DisplayTrajectory publisher which is used below to publish
    ## trajectories for RVIZ to visualize.
    display_trajectory_publisher = rospy.Publisher(
                                      '/move_group/display_planned_path',
                                      moveit_msgs.msg.DisplayTrajectory,
                                      queue_size=20)

    ## Wait for RVIZ to initialize. This sleep is ONLY to allow Rviz to come up.
    print "============ Waiting for RVIZ..."
    rospy.sleep(10)
    print "============ Starting tutorial "

    ## Getting Basic Information
    ## ^^^^^^^^^^^^^^^^^^^^^^^^^
    ##
    ## We can get the name of the reference frame for this robot
    print "============ Reference frame L: %s" % groupL.get_planning_frame()
    print "============ Reference frame R: %s" % groupR.get_planning_frame()

    ## We can also print the name of the end-effector link for this group
    print "============ End effector L: %s" % groupL.get_end_effector_link()
    print "============ End effector R: %s" % groupR.get_end_effector_link()

    ## We can get a list of all the groups in the robot
    print "============ Robot Groups:"
    print robot.get_group_names()

    ## Sometimes for debugging it is useful to print the entire state of the
    ## robot.
    print "============ Printing robot state"
    print robot.get_current_state()
    print "============"


    sub_func = None
    channel = ""
    msgType = None
    connection_list = []

    if  part == 'left':
        channel = "left_isvalid"
        pub = rospy.Publisher(channel,Bool,queue_size=1)
        sub_func = partial(moveArm,pub,groupL)
        msgType = Pose   
        channel = "left_request"
        connection_list.append((channel,msgType,sub_func))  
         
    elif part == 'right':
        channel = "right_isvalid"
        pub = rospy.Publisher(channel,Bool,queue_size=1)
        sub_func = partial(moveArm,pub,groupR)
        msgType = Pose   
        channel = "right_request"
        connection_list.append((channel,msgType,sub_func))  

    else :
        print "unknown part:", part
        moveit_commander.roscpp_shutdown()
        return 0
     

    
    #Start movement
    print "starting part: ",part
    for connection in connection_list:
        channel,msgType,sub_func = connection
        rospy.Subscriber(channel, msgType, sub_func)
        


    rospy.spin()


    ## When finished shut down moveit_commander.
    moveit_commander.roscpp_shutdown()

    return 0
        


if __name__ == "__main__":
    sys.exit(int(main() or 0))





