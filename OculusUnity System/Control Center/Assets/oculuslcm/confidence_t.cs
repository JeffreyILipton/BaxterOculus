/* LCM type definition class file
 * This file was automatically generated by lcm-gen
 * DO NOT MODIFY BY HAND!!!!
 */

using System;
using System.Collections.Generic;
using System.IO;
using LCM.LCM;
 
namespace oculuslcm
{
    public sealed class confidence_t : LCM.LCM.LCMEncodable
    {
        public float confidence;
 
        public confidence_t()
        {
        }
 
        public static readonly ulong LCM_FINGERPRINT;
        public static readonly ulong LCM_FINGERPRINT_BASE = 0x89a6a50363fb9211L;
 
        static confidence_t()
        {
            LCM_FINGERPRINT = _hashRecursive(new List<String>());
        }
 
        public static ulong _hashRecursive(List<String> classes)
        {
            if (classes.Contains("oculuslcm.confidence_t"))
                return 0L;
 
            classes.Add("oculuslcm.confidence_t");
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
            outs.Write(this.confidence); 
 
        }
 
        public confidence_t(byte[] data) : this(new LCMDataInputStream(data))
        {
        }
 
        public confidence_t(LCMDataInputStream ins)
        {
            if ((ulong) ins.ReadInt64() != LCM_FINGERPRINT)
                throw new System.IO.IOException("LCM Decode error: bad fingerprint");
 
            _decodeRecursive(ins);
        }
 
        public static oculuslcm.confidence_t _decodeRecursiveFactory(LCMDataInputStream ins)
        {
            oculuslcm.confidence_t o = new oculuslcm.confidence_t();
            o._decodeRecursive(ins);
            return o;
        }
 
        public void _decodeRecursive(LCMDataInputStream ins)
        {
            this.confidence = ins.ReadSingle();
 
        }
 
        public oculuslcm.confidence_t Copy()
        {
            oculuslcm.confidence_t outobj = new oculuslcm.confidence_t();
            outobj.confidence = this.confidence;
 
            return outobj;
        }
    }
}

