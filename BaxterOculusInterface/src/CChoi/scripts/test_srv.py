#!/usr/bin/env python
import rospy
from cnn3d_grasping.srv import Start, StartRequest, Stop, StopRequest, \
    GetThreshold, GetThresholdRequest, \
    SetThreshold, SetThresholdRequest, \
    NeedHelp, NeedHelpResponse
from geometry_msgs.msg import Pose, Quaternion
import sys

class Test():
    def __init__(self):
        rospy.wait_for_service('demo_grasping/start')
        self._srv_start = rospy.ServiceProxy('demo_grasping/start', Start)

        rospy.wait_for_service('demo_grasping/stop')
        self._srv_stop = rospy.ServiceProxy('demo_grasping/stop', Stop)

        rospy.wait_for_service('demo_grasping/get_th')
        self._srv_get_th = rospy.ServiceProxy('demo_grasping/get_th', GetThreshold)

        rospy.wait_for_service('demo_grasping/set_th')
        self._srv_set_th = rospy.ServiceProxy('demo_grasping/set_th', SetThreshold)

        s = rospy.Service('~grasp_help', NeedHelp, self.srv_grasp_help)

    def srv_grasp_help(self, req):
        rospy.loginfo('srv_grasp_help')

        # a human gives a good grasp pose
        # ...

        pose = Pose(orientation=Quaternion(w=1.0))
        resp = NeedHelpResponse()
        resp.pose = pose
        return resp

    def _test_start(self):
        req = StartRequest()
        resp = self._srv_start(req)
        print('done _test_start')

    def _test_stop(self):
        req = StopRequest()
        resp = self._srv_stop(req)
        print('done _test_stop')

    def _test_get_th(self):
        req = GetThresholdRequest()
        resp = self._srv_get_th(req)
        print('threshold value: {}'.format(resp.th))
        print('done _test_get_th')

    def _test_set_th(self):
        req = SetThresholdRequest()
        req.th = 0.2
        resp = self._srv_set_th(req)
        print('done _test_set_th')

    def test(self):
        try:
            self._test_start()
            self._test_stop()
            self._test_get_th()
            self._test_set_th()
        except rospy.ServiceException, e:
            print("Service call failed: {}".format(e))

def main(args):
    rospy.init_node('test_srv')
    t = Test()

    # quick service call test
    t.test()

    # waiting for help service call
    t._test_start()
    r = rospy.Rate(10)
    while not rospy.is_shutdown():
        r.sleep()

if __name__ == '__main__':
    main(sys.argv)


