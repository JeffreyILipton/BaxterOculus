"""LCM type definitions
This file automatically generated by lcm.
DO NOT MODIFY BY HAND!!!!
"""

try:
    import cStringIO.StringIO as BytesIO
except ImportError:
    from io import BytesIO
import struct

class velocity_t(object):
    __slots__ = ["velocity"]

    def __init__(self):
        self.velocity = 0.0

    def encode(self):
        buf = BytesIO()
        buf.write(velocity_t._get_packed_fingerprint())
        self._encode_one(buf)
        return buf.getvalue()

    def _encode_one(self, buf):
        buf.write(struct.pack(">d", self.velocity))

    def decode(data):
        if hasattr(data, 'read'):
            buf = data
        else:
            buf = BytesIO(data)
        if buf.read(8) != velocity_t._get_packed_fingerprint():
            raise ValueError("Decode error")
        return velocity_t._decode_one(buf)
    decode = staticmethod(decode)

    def _decode_one(buf):
        self = velocity_t()
        self.velocity = struct.unpack(">d", buf.read(8))[0]
        return self
    _decode_one = staticmethod(_decode_one)

    _hash = None
    def _get_hash_recursive(parents):
        if velocity_t in parents: return 0
        tmphash = (0xcd0b79cc8bfd113) & 0xffffffffffffffff
        tmphash  = (((tmphash<<1)&0xffffffffffffffff)  + (tmphash>>63)) & 0xffffffffffffffff
        return tmphash
    _get_hash_recursive = staticmethod(_get_hash_recursive)
    _packed_fingerprint = None

    def _get_packed_fingerprint():
        if velocity_t._packed_fingerprint is None:
            velocity_t._packed_fingerprint = struct.pack(">Q", velocity_t._get_hash_recursive([]))
        return velocity_t._packed_fingerprint
    _get_packed_fingerprint = staticmethod(_get_packed_fingerprint)
