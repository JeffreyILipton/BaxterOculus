"""LCM type definitions
This file automatically generated by lcm.
DO NOT MODIFY BY HAND!!!!
"""

try:
    import cStringIO.StringIO as BytesIO
except ImportError:
    from io import BytesIO
import struct

class trigger_t(object):
    __slots__ = ["trigger"]

    def __init__(self):
        self.trigger = False

    def encode(self):
        buf = BytesIO()
        buf.write(trigger_t._get_packed_fingerprint())
        self._encode_one(buf)
        return buf.getvalue()

    def _encode_one(self, buf):
        buf.write(struct.pack(">b", self.trigger))

    def decode(data):
        if hasattr(data, 'read'):
            buf = data
        else:
            buf = BytesIO(data)
        if buf.read(8) != trigger_t._get_packed_fingerprint():
            raise ValueError("Decode error")
        return trigger_t._decode_one(buf)
    decode = staticmethod(decode)

    def _decode_one(buf):
        self = trigger_t()
        self.trigger = bool(struct.unpack('b', buf.read(1))[0])
        return self
    _decode_one = staticmethod(_decode_one)

    _hash = None
    def _get_hash_recursive(parents):
        if trigger_t in parents: return 0
        tmphash = (0x4b54427c3c3d322c) & 0xffffffffffffffff
        tmphash  = (((tmphash<<1)&0xffffffffffffffff)  + (tmphash>>63)) & 0xffffffffffffffff
        return tmphash
    _get_hash_recursive = staticmethod(_get_hash_recursive)
    _packed_fingerprint = None

    def _get_packed_fingerprint():
        if trigger_t._packed_fingerprint is None:
            trigger_t._packed_fingerprint = struct.pack(">Q", trigger_t._get_hash_recursive([]))
        return trigger_t._packed_fingerprint
    _get_packed_fingerprint = staticmethod(_get_packed_fingerprint)

