"""LCM type definitions
This file automatically generated by lcm.
DO NOT MODIFY BY HAND!!!!
"""

try:
    import cStringIO.StringIO as BytesIO
except ImportError:
    from io import BytesIO
import struct

class range_t(object):
    __slots__ = ["range"]

    def __init__(self):
        self.range = 0.0

    def encode(self):
        buf = BytesIO()
        buf.write(range_t._get_packed_fingerprint())
        self._encode_one(buf)
        return buf.getvalue()

    def _encode_one(self, buf):
        buf.write(struct.pack(">f", self.range))

    def decode(data):
        if hasattr(data, 'read'):
            buf = data
        else:
            buf = BytesIO(data)
        if buf.read(8) != range_t._get_packed_fingerprint():
            raise ValueError("Decode error")
        return range_t._decode_one(buf)
    decode = staticmethod(decode)

    def _decode_one(buf):
        self = range_t()
        self.range = struct.unpack(">f", buf.read(4))[0]
        return self
    _decode_one = staticmethod(_decode_one)

    _hash = None
    def _get_hash_recursive(parents):
        if range_t in parents: return 0
        tmphash = (0x324da988abdbae17) & 0xffffffffffffffff
        tmphash  = (((tmphash<<1)&0xffffffffffffffff)  + (tmphash>>63)) & 0xffffffffffffffff
        return tmphash
    _get_hash_recursive = staticmethod(_get_hash_recursive)
    _packed_fingerprint = None

    def _get_packed_fingerprint():
        if range_t._packed_fingerprint is None:
            range_t._packed_fingerprint = struct.pack(">Q", range_t._get_hash_recursive([]))
        return range_t._packed_fingerprint
    _get_packed_fingerprint = staticmethod(_get_packed_fingerprint)
