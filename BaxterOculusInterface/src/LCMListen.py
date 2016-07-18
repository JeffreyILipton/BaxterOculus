#!/usr/bin/env python
import os
import sys
if os.name!='nt':
    import lcm

from oculuslcm import *
from Comms import *

def range_handler(channel,data):
    msg = range_t.decode(data)
    print msg.range

def image_handler(channel,data):
	msg = image_t.decode(data)
	print "len: ",len(msg.data)


channel = LCM_R_RANGE
#channel = LCM_L_CAMERA
try:
    lc = lcm.LCM()
    notdone = True
    subscription = lc.subscribe(channel, range_handler)
    while notdone:
        lc.handle()

except KeyboardInterrupt:
    pass    
