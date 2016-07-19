#!/usr/bin/env python
from enum import IntEnum
import serial
import struct
import time

DEBUG = True
NO_SERIAL = False

class ArduinoInterface(object):
    """Serial Interface to Arduino"""
    def __init__(self,port):
        if NO_SERIAL:
            self.port = serial.Serial()
        else:
            self.port = serial.Serial(port,9600,parity = serial.PARITY_NONE, stopbits = serial.STOPBITS_ONE,xonxoff=False,timeout=10.0)

    def _writeCommand(self,cmd):
        '''the input type should be a string, int, or Create_ value
           int is converted to a char string,
           strings are passed through'''
        #cmd = str(chr(cmd))
        nb = len(cmd)
        if self.port.is_open:
            nb_written = self.port.write(cmd)
            if (nb != nb_written):print "Error only wrote %i not %i bytes"%(nb_written,nb)
        if DEBUG:
            int_form = []
            for i in range(0,nb):
                int_form.append(int(ord(cmd[i])))
            print "#######Trigger cmd:", cmd
            print "int form:", int_form
            print "read: ", self.port.read()

    def trigger(self,cmd=False):
        if cmd:
            self._writeCommand('T')
        else:
            self._writeCommand('R');

if __name__ == '__main__':
    AI = ArduinoInterface("/dev/ttyACM0")
    print AI.port.is_open
    val = 0
    while val <2 and val>=0:
        val = input("val 1/0")
        AI.trigger(val)