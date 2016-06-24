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


channel = LCM_L_RANGE

try:
    lc = lcm.LCM()
    notdone = True
    subscription = lc.subscribe(channel, range_handler)
    while notdone:
        lc.handle()

except KeyboardInterrupt:
    pass    
