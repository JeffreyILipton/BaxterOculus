NAME = "Remote7/"
STATE = "_state"

ROS_LEFT_VALID = "left_ros_valid"
ROS_LEFT_VALID_STATE = ROS_LEFT_VALID + STATE

ROS_LEFT_CMD_STATE = "left_ros_pos_cmd"

ROS_LEFT_CURRENTPOS = "left_ros_currentpos"
ROS_LEFT_CURRENTPOS_STATE = ROS_LEFT_CURRENTPOS + STATE

ROS_RIGHT_VALID = "right_ros_valid"
ROS_RIGHT_VALID_STATE = ROS_RIGHT_VALID + STATE

ROS_RIGHT_CMD_STATE = "right_ros_pos_cmd"

ROS_RIGHT_CURRENTPOS = "left_ros_currentpos"
ROS_RIGHT_CURRENTPOS_STATE = ROS_RIGHT_CURRENTPOS + STATE



ROS_RIGHT_CAM = "/cameras/right_hand_camera/image"
ROS_RIGHT_CAM_ECHO = "/echo/cameras/right_hand_camera/image"

ROS_LEFT_CAM = "/cameras/left_hand_camera/image"
ROS_LEFT_CAM_ECHO = "/echo/cameras/left_hand_camera/image"

# listening
ROS_RIGHT   = NAME+"right_arm_cmd"
ROS_LEFT    = NAME+"left_arm_cmd"
ROS_HEAD    =  NAME+"head_cmd"

ROS_R_TRIGGER = NAME+"right_trigger_cmd"
ROS_L_TRIGGER = NAME+"left_trigger_cmd"

ROS_R_VEL   = NAME+"right_vel"
ROS_R_CMD   = NAME+"right_cmd"

ROS_L_VEL   = NAME+"left_vel"
ROS_L_CMD   = NAME+"left_cmd"

