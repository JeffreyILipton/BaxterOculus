#!/usr/bin/env python
import os
import sys
if os.name!='nt':
    import lcm

from oculuslcm import *
from Comms import *
from math import *

message2 = pose_t()
message2.position = [ 0.254,0,0.25 ]
message2.orientation = [0.0490787797075,-0.386342787948,0.920786840126,0.0219572000488]


channel = LCM_RIGHT
try:
    lc = lcm.LCM()
    notdone = True
    while notdone:
        ang = input('ang in degrees: ')
        ang = pi/180.0*ang

        array3 = [0.3,0,0.2]#input('3 array: ')
        n = input('n: ')
        mag = sqrt(n[0]*n[0]+n[1]*n[1]+n[2]*n[2])
        n = [x*sin(ang/2)/mag for x in n]
        array4 = [cos(ang/2)]+n
        print array4
        if len(array4)!=4 or len(array3)!=3: 
            notdone = False
        else:
            message2.position=array3
            message2.orientation = array4
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