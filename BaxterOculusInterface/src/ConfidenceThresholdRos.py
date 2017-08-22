#!/usr/bin/env python
import os
import sys
if os.name!='nt':
    import lcm
    import rospy
    from baxter_interface import *
    from std_msgs.msg import *
    from geometry_msgs.msg import (
        Pose,
        Point,
        Quaternion,
    )

from oculuslcm import *
from functools import *
from Comms import *

def ConfidenceThresholdRos(RosPub,LcmChannel,lcmData):
    #print "test"
    try:
        lcm_msg = confidencethreshold_t.decode(lcmData)
        print "confidence:", lcm_msg.confidence
        RosPub.publish(lcm_msg.confidence )
    except:
        pass
        #print "errored"


class LCMInterface():
    def __init__(self):

        rospy.init_node('ConfidenceThresholdRos',anonymous = True)


        full_param_name = rospy.search_param('confidenceThresholdChannel')
        param_value = rospy.get_param(full_param_name)
        confidenceThresholdChannel = param_value

        self.lc = lcm.LCM("udpm://239.255.76.67:7667:?ttl=1")
        self.subscriptions={}

        connections = [(ROS_THRESHOLD,confidenceThresholdChannel,Float32)]
        

        ros_channnel,lcm_channel,ros_msg_type = connections[0]
        pub = rospy.Publisher(ros_channnel, ros_msg_type, queue_size=1)

        subscriber = partial(ConfidenceThresholdRos,pub)

        self.subscriptions[ros_channnel] = self.lc.subscribe(lcm_channel,subscriber)

    def run(self):
        try:
            while not rospy.is_shutdown():
                 self.lc.handle()
                 #print "."
        except KeyboardInterrupt:
            pass

def main():
    print "lcm start"
    LCMI = LCMInterface()

    try:
        LCMI.run()
    except rospy.ROSInterruptException:pass



if __name__ == "__main__":

    sys.exit(int(main() or 0))

