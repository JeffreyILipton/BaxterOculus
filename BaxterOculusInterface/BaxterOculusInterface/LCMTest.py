import os
import sys
if os.name!='nt':
    import lcm

from oculuslcm import *

message1 = hand_t()
message1.position = [ 0.0 for dim0 in range(0,3) ]
message1.orientation = [ 0.0 for dim0 in range(0,4) ]
message1.orientation[1]=1

message2 = hand_t()
message2.position = [ 100.0 for dim0 in range(0,3) ]
message2.orientation = [ 0.25 for dim0 in range(0,4) ]


channel = 'left'
try:
    lc = lcm.LCM()
    notdone = True
    while notdone:
        val = raw_input("A or B")
        if val == 'a' or val == 'A':
            lc.publish(channel,message1.encode())
        elif val == 'b' or val == 'B':
            lc.publish(channel,message2.encode())
        else:
            print "Not A or B, got : ",val
            notdone = False
except KeyboardInterrupt:
    pass    