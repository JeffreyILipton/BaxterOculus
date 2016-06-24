import os
import sys

if os.name!='nt':
    import rospy
    from baxter_interface import *
    from geometry_msgs.msg import (
        PoseStamped,
        Pose,
        Point,
        Quaternion,
    )
    from std_msgs.msg import Header
    from std_msgs.msg import Bool
    from baxter_core_msgs.srv import (
        SolvePositionIK,
        SolvePositionIKRequest,
    )

import time
import threading

class ServiceTimeouter(object):
    """ Ros services cannot be timed out. Occasionally the IK solver would take
        up to 5 seconds to respond. This is a workaround class. """
    def __init__(self,timeout, srv, param):
        self.srv = srv
        self.param = param
        self.timeout = timeout
        print "timeout:", timeout
        self.retval = None
        self.returned = False
        self.lock = threading.Lock()
        self.thread = threading.Thread(target=self._call_thread)
    def _call_thread(self):
        try:
            self.retval = self.srv(self.param)
            self.returned = True
        except rospy.ServiceException, e:
            rospy.loginfo("Service call failed: %s" % (e,))
        except AttributeError:
            rospy.loginfo("Socket.close() exception. Socket has become 'None'")
    def call(self):
        self.thread.start()
        timeout = time.time() + self.timeout
        while time.time() < timeout and self.thread.isAlive():
            time.sleep(0.001)
            #print (timeout-time.time())/self.timeout
        if not self.returned:
            print "timed out"
            return None
        return self.retval
