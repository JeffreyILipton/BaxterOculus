import os
import sys
if os.name!='nt':
    import lcm

from oculuslcm import *
from Comms import *
message1 = pose_t()
message1.position = [ 100.0,0,200.0 ]
message1.orientation = [ 0.25 for dim0 in range(0,4) ]


message2 = pose_t()
message2.position = [ 100.0,0,300.0 ]
message2.orientation = [0.0490787797075,-0.386342787948,0.920786840126,0.0219572000488]
'''    x: -0.386342787948
    y: 0.920786840126
    z: 0.0219572000488
    w: 0.0490787797075'''
message3 = pose_t()
message3.position = [ 100.0,0,400.0 ]
message3.orientation = [ 0.25 for dim0 in range(0,4) ]

channel = LCM_LEFT
try:
    lc = lcm.LCM()
    notdone = True
    while notdone:
	array = input('3 array: ')
        if len(array)!=3: notdone = False
        else:
            message2.position=array
            lc.publish(channel,message2.encode())
        #val = raw_input("A,B,C: ")
        #if val == 'a' or val == 'A':
        #    lc.publish(channel,message1.encode())
        #elif val == 'b' or val == 'B':
        #    lc.publish(channel,message2.encode())

        #elif val == 'c' or val == 'C':
        #    lc.publish(channel,message3.encode())
        #else:
        #    print "Not A or B or C, got : ",val
        #    notdone = False
except KeyboardInterrupt:
    pass    
