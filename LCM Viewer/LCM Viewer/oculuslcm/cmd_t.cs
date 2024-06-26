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
    public sealed class cmd_t : LCM.LCM.LCMEncodable
    {
        public short command;
 
        public cmd_t()
        {
        }
 
        public static readonly ulong LCM_FINGERPRINT;
        public static readonly ulong LCM_FINGERPRINT_BASE = 0x304d4fab5f291c2cL;
 
        static cmd_t()
        {
            LCM_FINGERPRINT = _hashRecursive(new List<String>());
        }
 
        public static ulong _hashRecursive(List<String> classes)
        {
            if (classes.Contains("oculuslcm.cmd_t"))
                return 0L;
 
            classes.Add("oculuslcm.cmd_t");
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
            outs.Write(this.command); 
 
        }
 
        public cmd_t(byte[] data) : this(new LCMDataInputStream(data))
        {
        }
 
        public cmd_t(LCMDataInputStream ins)
        {
            if ((ulong) ins.ReadInt64() != LCM_FINGERPRINT)
                throw new System.IO.IOException("LCM Decode error: bad fingerprint");
 
            _decodeRecursive(ins);
        }
 
        public static oculuslcm.cmd_t _decodeRecursiveFactory(LCMDataInputStream ins)
        {
            oculuslcm.cmd_t o = new oculuslcm.cmd_t();
            o._decodeRecursive(ins);
            return o;
        }
 
        public void _decodeRecursive(LCMDataInputStream ins)
        {
            this.command = ins.ReadInt16();
 
        }
 
        public oculuslcm.cmd_t Copy()
        {
            oculuslcm.cmd_t outobj = new oculuslcm.cmd_t();
            outobj.command = this.command;
 
            return outobj;
        }
    }
}

