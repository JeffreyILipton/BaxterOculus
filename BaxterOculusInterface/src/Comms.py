ROS_LEFT    = "left_arm_cmd"
LCM_LEFT    = "left_lcm"
LCM_LEFT_VALID = "left_lcm_valid"
LCM_LEFT_CURRNEPOS = "left_lcm_currentpos"

ROS_RIGHT   = "right_arm_cmd"
LCM_RIGHT   = "right_lcm"
LCM_RIGHT_VALID = "right_lcm_valid"
LCM_RIGHT_CURRNEPOS = "right_lcm_currentpos"

ROS_HEAD    = "head_cmd"
LCM_HEAD    = "head_lcm"

ROS_R_TRIGGER = "right_trigger_cmd"
LCM_R_TRIGGER = "right_trigger_lcm"

ROS_L_TRIGGER = "left_trigger_cmd"
LCM_L_TRIGGER = "left_trigger_lcm"

ROS_R_VEL   = "right_vel"
LCM_R_VEL   = "right_lcm_vel"
ROS_R_CMD   = "right_gripper_cmd"
LCM_R_CMD   = "right_lcm_cmd"

ROS_L_VEL   = "left_vel"
LCM_L_VEL   = "left_lcm_vel"
ROS_L_CMD   = "left_gripper__cmd"
LCM_L_CMD   = "left_lcm_cmd"

ROS_L_RANGE = "/robot/range/left_hand_range/state"
LCM_L_RANGE = "left_lcm_range"
ROS_R_RANGE = "/robot/range/right_hand_range/state"
LCM_R_RANGE = "right_lcm_range"

LCM_L_CAMERA = "left_lcm_camera"
ROS_L_CAMERA = "/cameras/left_hand_camera/image"
LCM_R_CAMERA = "right_lcm_camera"
ROS_R_CAMERA = "/cameras/right_hand_camera/image"

LCM_MONITOR_CAMERA = "monitor_lcm_camera"

ROS_QUERY      = "query"
ROS_CONFIDENCE = "confidence"
ROS_THRESHOLD  = "threshold"
LCM_CONFIDENCE_THRESHOLD = "confidence_threshold_lcm"

CC_PREFIX = "grasp_learning"

ROS_ORB = "orb_grab_release"
LCM_ORB = "orb_grab_release_lcm"

ROS_RIGHT_REQUEST = "right_request"
ROS_LEFT_REQUEST = "left_request"
ROS_RIGHT_ISVALID = "right_isvalid"
ROS_LEFT_ISVALID = "left_isvalid"

ROS_PROGRAM_START = "/human_factors/program_start"
ROS_TRIAL_START = "/human_factors/trial_start"
ROS_TRIAL_END = "/human_factors/trial_end_status"
ROS_GRASP_RESULT = "/human_factors/grasp_result"
ROS_HELP_REQUESTED = "/human_factors/help_request_start"
ROS_HELP_COMPLETED = "/human_factors/help_request_end"
ROS_EXP_NOTES = "/human_factors/exp_notes"


ROS_HELP = "help"
