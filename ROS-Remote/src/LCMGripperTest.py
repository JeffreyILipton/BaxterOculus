#!/usr/bin/env python
import os
import sys
if os.name!='nt':
    import lcm

from oculuslcm import *
from Comms import *
open_cmd_msg = cmd_t()
open_cmd_msg.command = 1

close_cmd_msg = cmd_t()
close_cmd_msg.command = 2

stop_cmd_msg = cmd_t()
stop_cmd_msg.command = 0


channel = LCM_L_CMD
try:
    lc = lcm.LCM()
    notdone = True
    while notdone:
        val = input('Val: ')
        if val >3: 
            notdone = False
            lc.publish(channel,stop_cmd_msg.encode())
        elif val==2: 
            lc.publish(channel,close_cmd_msg.encode())
        elif val==1: 
            lc.publish(channel,open_cmd_msg.encode())
        else:
            lc.publish(channel,stop_cmd_msg.encode())
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
