#!/usr/bin/env python
from enum import IntEnum
import serial
import struct

DEBUG = False#True
NO_SERIAL = False

class ArduinoInterface(object):
    """Serial Interface to Arduino"""
    def __init__(self,port):
        if NO_SERIAL:
            self.port = serial.Serial()
        else:
            self.port = serial.Serial(port,19200,parity = serial.PARITY_NONE, stopbits = serial.STOPBITS_ONE,xonxoff=False,timeout=10.0)

    def _writeCommand(self,cmd):
        '''the input type should be a string, int, or Create_ value
           int is converted to a char string,
           strings are passed through'''
        cmd = str(chr(cmd))
        nb = len(cmd)
        if self.port.is_open:
            nb_written = self.port.write(cmd)
            if (nb != nb_written):print "Error only wrote %i not %i bytes"%(nb_written,nb)
        if DEBUG:
            int_form = []
            for i in range(0,nb):
                int_form.append(int(ord(cmd[i])))
            print cmd
            print int_form

    def trigger(self):
        self._writeCommand("1")

if __name__ == '__main__':
    AI = ArduinoInterface("/dev/ttyACM1")
    print AI.port.is_open
    AI.trigger