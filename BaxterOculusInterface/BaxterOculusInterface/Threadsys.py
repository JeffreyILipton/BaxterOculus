from threading import Thread, Lock
from functools import *
import time
from math import *

class MessageHolder():
    def __init__(self,lock,message):
        self.msg = message
        self.lock = lock
        self.read = True
    def setMsg(self,message):
        self.lock.acquire()
        self.msg = message
        self.read = False
        self.lock.release()
    def getMsg(self):
        read =self.read
        self.read = True
        return self.msg,read

def lcm_handler(state_holder,msg):
    state_holder.setMsg(msg)

class LCMInterface(Thread):
    def __init__(self,lc,channel,state_holder):
        self.channel = channel
        Thread.__init__(self)
        self.lc = lc
        self.subscription = self.lc.subscribe(channel,state_holder.setMsg)

    def run(self):
        try:
            while True:
                 self.lc.handle()
        except KeyboardInterrupt:
            pass    

class BaxterPartInterface(Thread):
    def __init__(self,dt,message_holder,processor):
        Thread.__init__(self)
        self.holder = message_holder
        self.processor = processor
        self.dt = dt

    def run(self):
        try:
            while True:
                tic = time.time()
                msg, isread = self.holder.getMsg()
                if not isread:
                    self.processor(msg)
                toc = time.time()
                time_taken = toc-tic
                waittime = self.dt-time_taken
                waittime = max(waittime,0)
                time.sleep(waittime/self.speedup)
        except KeyboardInterrupt:
            pass    

