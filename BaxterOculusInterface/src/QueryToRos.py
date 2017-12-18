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

def QueryRos(RosPub,LcmChannel,lcmData):
    #print "QuerryRosCalled"
    try:
        lcm_msg = query_t.decode(lcmData)
        #print "LCM Channel: "+LcmChannel+"\tRos: "+RosPub.name+"\tuser id: "+str(lcm_msg.userID)
        '''For some reason, different LCM Channels get the same call? i ahve no idea why
        print "LCM Channel: "+LcmChannel+"\tRos: "+RosPub.name+"\tuser id: "+str(lcm_msg.userID)

        looks like this 
        LCM Channel: control_query|101  Ros: /query_101 user id: -2
        done _test_stop
        LCM Channel: control_query|100  Ros: /query_101 user id: 12
        '''
        lcmparts = LcmChannel.split("|")
        rosparts = RosPub.name.split("_")
        if len(lcmparts) ==2 and len(rosparts)==2:
            if lcmparts[1] == rosparts[1]:
                print "publishing Querry for " + str(lcmparts[1])
                RosPub.publish(lcm_msg.userID)
    except:
        pass
        #print "errored"


class LCMInterface():
    def __init__(self):

        rospy.init_node('QueryRos',anonymous = True)


        full_param_name = rospy.search_param('queryChannel')
        param_value = rospy.get_param(full_param_name)
        queryChannel = param_value

        self.lc = lcm.LCM("udpm://239.255.76.67:7667:?ttl=1")
        self.subscriptions={}

        print "Querry Channel in LCM: ",queryChannel
        vals = queryChannel.split("|")
        print "Querry Channel in ROS: ",ROS_QUERY+'_'+vals[1]
        #vals = queryChannel.split("|")

        connections = [(ROS_QUERY+'_'+vals[1],queryChannel,Int16)]
        print connections

        ros_channnel,lcm_channel,ros_msg_type = connections[0]
        pub = rospy.Publisher(ros_channnel, ros_msg_type, queue_size=1)

        subscriber = partial(QueryRos,pub)

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

