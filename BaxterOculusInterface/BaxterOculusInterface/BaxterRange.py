#!/usr/bin/env python
# -*- coding: iso-8859-1 -*-

import sys
import time
import rospy
import baxter_interface
from baxter_interface import CHECK_VERSION


from sensor_msgs.msg import Range



class BaxterRangeSensor():
    """
        Range Sensors
    """
    def __init__(self):
        self.distance = {}
        root_name = "/robot/range/"
        sensor_name = ["left_hand_range/state","right_hand_range/state"]
        self.__left_sensor  = rospy.Subscriber(root_name + sensor_name[0],Range, callback=self.__sensorCallback, callback_args="left",queue_size=1)
        self.__right_sensor = rospy.Subscriber(root_name + sensor_name[1],Range, callback=self.__sensorCallback, callback_args="right",queue_size=1)
      
    def __sensorCallback(self,msg,side):
       self.distance[side] = msg.range
       print "%s: %f"%(side,msg.range)


def main():
    rospy.init_node('range_state', anonymous=True)
    rs = baxter_interface.RobotEnable(CHECK_VERSION)
    init_state = rs.state().enabled

    RangeSensor = BaxterRangeSensor()
    rospy.spin()
    sys.exit()


if __name__ == "__main__":
    sys.exit(int(main() or 0))