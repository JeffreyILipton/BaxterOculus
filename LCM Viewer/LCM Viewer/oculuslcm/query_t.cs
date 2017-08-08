

using System;
using System.Collections.Generic;
using System.IO;
using LCM.LCM;
 
namespace oculuslcm
{
    public sealed class query_t : LCM.LCM.LCMEncodable
    {
        public int userID;
 
        public query_t()
        {
        }
 
        public static readonly ulong LCM_FINGERPRINT;
        public static readonly ulong LCM_FINGERPRINT_BASE = 0x324da988abdbae17L;
 
        static query_t()
        {
            LCM_FINGERPRINT = _hashRecursive(new List<String>());
        }
 
        public static ulong _hashRecursive(List<String> classes)
        {
            if (classes.Contains("oculuslcm.query_t"))
                return 0L;
 
            classes.Add("oculuslcm.query_t");
            ulong hash = LCM_FINGERPRINT_BASE
                ;
            classes.RemoveAt(classes.Count - 1);
            return (hash<<1) + ((hash>>63)&1);
        }
 
        public void Encode(LCMDataOutputStream outs)
        {
            outs.Write((long) LCM_FINGERPRINT);
            _encodeRecursive(outs);
        }
 
        public void _encodeRecursive(LCMDataOutputStream outs)
        {
            outs.Write(this.userID); 
 
        }
 
        public query_t(byte[] data) : this(new LCMDataInputStream(data))
        {
        }
 
        public query_t(LCMDataInputStream ins)
        {
            if ((ulong) ins.ReadInt64() != LCM_FINGERPRINT)
                throw new System.IO.IOException("LCM Decode error: bad fingerprint");
 
            _decodeRecursive(ins);
        }
 
        public static oculuslcm.query_t _decodeRecursiveFactory(LCMDataInputStream ins)
        {
            oculuslcm.query_t o = new oculuslcm.query_t();
            o._decodeRecursive(ins);
            return o;
        }
 
        public void _decodeRecursive(LCMDataInputStream ins)
        {
            this.userID = ins.ReadInt32();
 
        }
 
        public oculuslcm.query_t Copy()
        {
            oculuslcm.query_t outobj = new oculuslcm.query_t();
            outobj.userID = this.userID;
 
            return outobj;
        }
    }
}
