"""LCM type definitions
This file automatically generated by lcm.
DO NOT MODIFY BY HAND!!!!
"""

try:
    import cStringIO.StringIO as BytesIO
except ImportError:
    from io import BytesIO
import struct

class robotself_t(object):
    __slots__ = ["id", "type", "ability", "queryChannel", "channelCount", "channels", "leftNDIChannel", "rightNDIChannel", "enabled"]

    def __init__(self):
        self.id = 0
        self.type = ""
        self.ability = ""
        self.queryChannel = ""
        self.channelCount = 0
        self.channels = []
        self.leftNDIChannel = ""
        self.rightNDIChannel = ""
        self.enabled = False

    def encode(self):
        buf = BytesIO()
        buf.write(robotself_t._get_packed_fingerprint())
        self._encode_one(buf)
        return buf.getvalue()

    def _encode_one(self, buf):
        buf.write(struct.pack(">h", self.id))
        __type_encoded = self.type.encode('utf-8')
        buf.write(struct.pack('>I', len(__type_encoded)+1))
        buf.write(__type_encoded)
        buf.write(b"\0")
        __ability_encoded = self.ability.encode('utf-8')
        buf.write(struct.pack('>I', len(__ability_encoded)+1))
        buf.write(__ability_encoded)
        buf.write(b"\0")
        __queryChannel_encoded = self.queryChannel.encode('utf-8')
        buf.write(struct.pack('>I', len(__queryChannel_encoded)+1))
        buf.write(__queryChannel_encoded)
        buf.write(b"\0")
        buf.write(struct.pack(">h", self.channelCount))
        for i0 in range(self.channelCount):
            __channels_encoded = self.channels[i0].encode('utf-8')
            buf.write(struct.pack('>I', len(__channels_encoded)+1))
            buf.write(__channels_encoded)
            buf.write(b"\0")
        __leftNDIChannel_encoded = self.leftNDIChannel.encode('utf-8')
        buf.write(struct.pack('>I', len(__leftNDIChannel_encoded)+1))
        buf.write(__leftNDIChannel_encoded)
        buf.write(b"\0")
        __rightNDIChannel_encoded = self.rightNDIChannel.encode('utf-8')
        buf.write(struct.pack('>I', len(__rightNDIChannel_encoded)+1))
        buf.write(__rightNDIChannel_encoded)
        buf.write(b"\0")
        buf.write(struct.pack(">b", self.enabled))

    def decode(data):
        if hasattr(data, 'read'):
            buf = data
        else:
            buf = BytesIO(data)
        if buf.read(8) != robotself_t._get_packed_fingerprint():
            raise ValueError("Decode error")
        return robotself_t._decode_one(buf)
    decode = staticmethod(decode)

    def _decode_one(buf):
        self = robotself_t()
        self.id = struct.unpack(">h", buf.read(2))[0]
        __type_len = struct.unpack('>I', buf.read(4))[0]
        self.type = buf.read(__type_len)[:-1].decode('utf-8', 'replace')
        __ability_len = struct.unpack('>I', buf.read(4))[0]
        self.ability = buf.read(__ability_len)[:-1].decode('utf-8', 'replace')
        __queryChannel_len = struct.unpack('>I', buf.read(4))[0]
        self.queryChannel = buf.read(__queryChannel_len)[:-1].decode('utf-8', 'replace')
        self.channelCount = struct.unpack(">h", buf.read(2))[0]
        self.channels = []
        for i0 in range(self.channelCount):
            __channels_len = struct.unpack('>I', buf.read(4))[0]
            self.channels.append(buf.read(__channels_len)[:-1].decode('utf-8', 'replace'))
        __leftNDIChannel_len = struct.unpack('>I', buf.read(4))[0]
        self.leftNDIChannel = buf.read(__leftNDIChannel_len)[:-1].decode('utf-8', 'replace')
        __rightNDIChannel_len = struct.unpack('>I', buf.read(4))[0]
        self.rightNDIChannel = buf.read(__rightNDIChannel_len)[:-1].decode('utf-8', 'replace')
        self.enabled = bool(struct.unpack('b', buf.read(1))[0])
        return self
    _decode_one = staticmethod(_decode_one)

    _hash = None
    def _get_hash_recursive(parents):
        if robotself_t in parents: return 0
        tmphash = (0x552e367cce2ca341) & 0xffffffffffffffff
        tmphash  = (((tmphash<<1)&0xffffffffffffffff)  + (tmphash>>63)) & 0xffffffffffffffff
        return tmphash
    _get_hash_recursive = staticmethod(_get_hash_recursive)
    _packed_fingerprint = None

    def _get_packed_fingerprint():
        if robotself_t._packed_fingerprint is None:
            robotself_t._packed_fingerprint = struct.pack(">Q", robotself_t._get_hash_recursive([]))
        return robotself_t._packed_fingerprint
    _get_packed_fingerprint = staticmethod(_get_packed_fingerprint)
