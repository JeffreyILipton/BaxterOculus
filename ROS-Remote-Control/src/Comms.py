
ROS_HEAD    = "head_cmd"

NAME = "Remote4/"
STATE = "_state"


ROS_R   = "right_arm_cmd"
ROS_R_VALID = "right_ros_valid"
ROS_R_CMD_STATE = "right_ros_pos_cmd"
ROS_R_CURRENTPOS = "left_ros_currentpos"

ROS_R_TRIGGER = "right_trigger_cmd"
ROS_R_VEL   = "right_vel"
ROS_R_CMD   = "right_cmd"
ROS_R_CAM = "/cameras/right_hand_camera/image"


ROS_L    = "left_arm_cmd"
ROS_L_VALID = "left_ros_valid"
ROS_L_CMD_STATE = "left_ros_pos_cmd"
ROS_L_CURRENTPOS = "left_ros_currentpos"

ROS_L_TRIGGER = "left_trigger_cmd"
ROS_L_VEL   = "left_vel"
ROS_L_CMD   = "left_cmd"
ROS_L_CAM = "/cameras/left_hand_camera/image"




# LCM
LCM_L    = "left_lcm"
LCM_L_VALID =        "left_lcm_valid"
LCM_L_CURRENTPOS =   "left_lcm_currentpos"

LCM_R   = "right_lcm"
LCM_R_VALID =      "right_lcm_valid"
LCM_R_CURRENTPOS = "right_lcm_currentpos"

LCM_HEAD    = "head_lcm"

LCM_R_TRIGGER = "right_trigger_lcm"
LCM_L_TRIGGER = "left_trigger_lcm"

LCM_R_VEL   = "right_lcm_vel"
LCM_R_CMD   = "right_lcm_cmd"

LCM_L_VEL   = "left_lcm_vel"
LCM_L_CMD   = "left_lcm_cmd"

LCM_L_RANGE = "left_lcm_range"
LCM_R_RANGE = "right_lcm_range"

LCM_L_CAMERA = "left_lcm_camera"
LCM_R_CAMERA = "right_lcm_camera"





#Modifiers
ECHO = "/echo/"

#NAME = "Remote/"
#STATE = "_state"

#ROBOT = "Baxter/"

#State responses
#ROS_R_CURRENTPOS_STATE = ECHO + ROS_R_CURRENTPOS
#ROS_R_VALID_STATE = ECHO + ROS_R_VALID

#ROS_L_CURRENTPOS_STATE = ECHO + ROS_L_CURRENTPOS
#ROS_L_VALID_STATE = ECHO + ROS_L_VALID

##Echoing
#ROS_R_CAM_ECHO = ECHO + ROS_R_CAM
#ROS_L_CAM_ECHO = ECHO + ROS_L_CAM



#LCM Local listening
#ROS_RIGHT   = NAME+"right_arm_cmd"
#ROS_LEFT    = NAME+"left_arm_cmd"
#ROS_HEAD    =  NAME+"head_cmd"

#ROS_R_TRIGGER = NAME+"right_trigger_cmd"
#ROS_L_TRIGGER = NAME+"left_trigger_cmd"

#ROS_R_VEL   = NAME+"right_vel"
#ROS_R_CMD   = NAME+"right_cmd"

#ROS_L_VEL   = NAME+"left_vel"
#ROS_L_CMD   = NAME+"left_cmd"


#remote listening
#ROS_RIGHT_CURRENTPOS =  ROBOT + "left_ros_currentpos"
#ROS_RIGHT_VALID =       ROBOT + "right_ros_valid"
#ROS_L_RANGE =           ROBOT + "robot/range/left_hand_range/state"
#ROS_R_RANGE =           ROBOT + "robot/range/right_hand_range/state"
#ROS_LEFT_VALID =        ROBOT + "left_ros_valid"
#ROS_LEFT_CURRENTPOS =   ROBOT + "left_ros_currentpos"
#ROS_L_CAMERA =          ROBOT + "echo/cameras/left_hand_camera/image"
#ROS_R_CAMERA =          ROBOT + "echo/cameras/right_hand_camera/image"



