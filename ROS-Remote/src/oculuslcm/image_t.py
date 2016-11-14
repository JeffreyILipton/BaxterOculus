"""LCM type definitions
This file automatically generated by lcm.
DO NOT MODIFY BY HAND!!!!
"""

try:
    import cStringIO.StringIO as BytesIO
except ImportError:
    from io import BytesIO
import struct

class image_t(object):
    __slots__ = ["width", "height", "row_stride", "pixelformat", "size", "data"]

    PIXEL_FORMAT_UYVY = 1498831189
    PIXEL_FORMAT_YUYV = 1448695129
    PIXEL_FORMAT_IYU1 = 827677001
    PIXEL_FORMAT_IYU2 = 844454217
    PIXEL_FORMAT_YUV420 = 842093913
    PIXEL_FORMAT_YUV411P = 1345401140
    PIXEL_FORMAT_I420 = 808596553
    PIXEL_FORMAT_NV12 = 842094158
    PIXEL_FORMAT_GRAY = 1497715271
    PIXEL_FORMAT_RGB = 859981650
    PIXEL_FORMAT_BGR = 861030210
    PIXEL_FORMAT_RGBA = 876758866
    PIXEL_FORMAT_BGRA = 877807426
    PIXEL_FORMAT_BAYER_BGGR = 825770306
    PIXEL_FORMAT_BAYER_GBRG = 844650584
    PIXEL_FORMAT_BAYER_GRBG = 861427800
    PIXEL_FORMAT_BAYER_RGGB = 878205016
    PIXEL_FORMAT_BE_BAYER16_BGGR = 826360386
    PIXEL_FORMAT_BE_BAYER16_GBRG = 843137602
    PIXEL_FORMAT_BE_BAYER16_GRBG = 859914818
    PIXEL_FORMAT_BE_BAYER16_RGGB = 876692034
    PIXEL_FORMAT_LE_BAYER16_BGGR = 826360396
    PIXEL_FORMAT_LE_BAYER16_GBRG = 843137612
    PIXEL_FORMAT_LE_BAYER16_GRBG = 859914828
    PIXEL_FORMAT_LE_BAYER16_RGGB = 876692044
    PIXEL_FORMAT_MJPEG = 1196444237
    PIXEL_FORMAT_BE_GRAY16 = 357
    PIXEL_FORMAT_LE_GRAY16 = 909199180
    PIXEL_FORMAT_BE_RGB16 = 358
    PIXEL_FORMAT_LE_RGB16 = 1279412050
    PIXEL_FORMAT_BE_SIGNED_GRAY16 = 359
    PIXEL_FORMAT_BE_SIGNED_RGB16 = 360
    PIXEL_FORMAT_FLOAT_GRAY32 = 842221382
    PIXEL_FORMAT_INVALID = -2
    PIXEL_FORMAT_ANY = -1

    def __init__(self):
        self.width = 0
        self.height = 0
        self.row_stride = 0
        self.pixelformat = 0
        self.size = 0
        self.data = ""

    def encode(self):
        buf = BytesIO()
        buf.write(image_t._get_packed_fingerprint())
        self._encode_one(buf)
        return buf.getvalue()

    def _encode_one(self, buf):
        buf.write(struct.pack(">iiiii", self.width, self.height, self.row_stride, self.pixelformat, self.size))
        buf.write(bytearray(self.data[:self.size]))

    def decode(data):
        if hasattr(data, 'read'):
            buf = data
        else:
            buf = BytesIO(data)
        if buf.read(8) != image_t._get_packed_fingerprint():
            raise ValueError("Decode error")
        return image_t._decode_one(buf)
    decode = staticmethod(decode)

    def _decode_one(buf):
        self = image_t()
        self.width, self.height, self.row_stride, self.pixelformat, self.size = struct.unpack(">iiiii", buf.read(20))
        self.data = buf.read(self.size)
        return self
    _decode_one = staticmethod(_decode_one)

    _hash = None
    def _get_hash_recursive(parents):
        if image_t in parents: return 0
        tmphash = (0x59b1bb180d65656a) & 0xffffffffffffffff
        tmphash  = (((tmphash<<1)&0xffffffffffffffff)  + (tmphash>>63)) & 0xffffffffffffffff
        return tmphash
    _get_hash_recursive = staticmethod(_get_hash_recursive)
    _packed_fingerprint = None

    def _get_packed_fingerprint():
        if image_t._packed_fingerprint is None:
            image_t._packed_fingerprint = struct.pack(">Q", image_t._get_hash_recursive([]))
        return image_t._packed_fingerprint
    _get_packed_fingerprint = staticmethod(_get_packed_fingerprint)

