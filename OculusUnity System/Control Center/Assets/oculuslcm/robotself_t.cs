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
    public sealed class robotself_t : LCM.LCM.LCMEncodable
    {
        public short id;
        public String type;
        public String ability;
        public String queryChannel;
        public short channelCount;
        public String[] channels;
        public String leftNDIChannel;
        public String rightNDIChannel;
        public bool enabled;
 
        public robotself_t()
        {
        }
 
        public static readonly ulong LCM_FINGERPRINT;
        public static readonly ulong LCM_FINGERPRINT_BASE = 0x552e367cce2ca341L;
 
        static robotself_t()
        {
            LCM_FINGERPRINT = _hashRecursive(new List<String>());
        }
 
        public static ulong _hashRecursive(List<String> classes)
        {
            if (classes.Contains("oculuslcm.robotself_t"))
                return 0L;
 
            classes.Add("oculuslcm.robotself_t");
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
            byte[] __strbuf = null;
            outs.Write(this.id); 
 
            __strbuf = System.Text.Encoding.GetEncoding("US-ASCII").GetBytes(this.type); outs.Write(__strbuf.Length+1); outs.Write(__strbuf, 0, __strbuf.Length); outs.Write((byte) 0); 
 
            __strbuf = System.Text.Encoding.GetEncoding("US-ASCII").GetBytes(this.ability); outs.Write(__strbuf.Length+1); outs.Write(__strbuf, 0, __strbuf.Length); outs.Write((byte) 0); 
 
            __strbuf = System.Text.Encoding.GetEncoding("US-ASCII").GetBytes(this.queryChannel); outs.Write(__strbuf.Length+1); outs.Write(__strbuf, 0, __strbuf.Length); outs.Write((byte) 0); 
 
            outs.Write(this.channelCount); 
 
            for (int a = 0; a < this.channelCount; a++) {
                __strbuf = System.Text.Encoding.GetEncoding("US-ASCII").GetBytes(this.channels[a]); outs.Write(__strbuf.Length+1); outs.Write(__strbuf, 0, __strbuf.Length); outs.Write((byte) 0); 
            }
 
            __strbuf = System.Text.Encoding.GetEncoding("US-ASCII").GetBytes(this.leftNDIChannel); outs.Write(__strbuf.Length+1); outs.Write(__strbuf, 0, __strbuf.Length); outs.Write((byte) 0); 
 
            __strbuf = System.Text.Encoding.GetEncoding("US-ASCII").GetBytes(this.rightNDIChannel); outs.Write(__strbuf.Length+1); outs.Write(__strbuf, 0, __strbuf.Length); outs.Write((byte) 0); 
 
            outs.Write(this.enabled); 
 
        }
 
        public robotself_t(byte[] data) : this(new LCMDataInputStream(data))
        {
        }
 
        public robotself_t(LCMDataInputStream ins)
        {
            if ((ulong) ins.ReadInt64() != LCM_FINGERPRINT)
                throw new System.IO.IOException("LCM Decode error: bad fingerprint");
 
            _decodeRecursive(ins);
        }
 
        public static oculuslcm.robotself_t _decodeRecursiveFactory(LCMDataInputStream ins)
        {
            oculuslcm.robotself_t o = new oculuslcm.robotself_t();
            o._decodeRecursive(ins);
            return o;
        }
 
        public void _decodeRecursive(LCMDataInputStream ins)
        {
            byte[] __strbuf = null;
            this.id = ins.ReadInt16();
 
            __strbuf = new byte[ins.ReadInt32()-1]; ins.ReadFully(__strbuf); ins.ReadByte(); this.type = System.Text.Encoding.GetEncoding("US-ASCII").GetString(__strbuf);
 
            __strbuf = new byte[ins.ReadInt32()-1]; ins.ReadFully(__strbuf); ins.ReadByte(); this.ability = System.Text.Encoding.GetEncoding("US-ASCII").GetString(__strbuf);
 
            __strbuf = new byte[ins.ReadInt32()-1]; ins.ReadFully(__strbuf); ins.ReadByte(); this.queryChannel = System.Text.Encoding.GetEncoding("US-ASCII").GetString(__strbuf);
 
            this.channelCount = ins.ReadInt16();
 
            this.channels = new String[(int) channelCount];
            for (int a = 0; a < this.channelCount; a++) {
                __strbuf = new byte[ins.ReadInt32()-1]; ins.ReadFully(__strbuf); ins.ReadByte(); this.channels[a] = System.Text.Encoding.GetEncoding("US-ASCII").GetString(__strbuf);
            }
 
            __strbuf = new byte[ins.ReadInt32()-1]; ins.ReadFully(__strbuf); ins.ReadByte(); this.leftNDIChannel = System.Text.Encoding.GetEncoding("US-ASCII").GetString(__strbuf);
 
            __strbuf = new byte[ins.ReadInt32()-1]; ins.ReadFully(__strbuf); ins.ReadByte(); this.rightNDIChannel = System.Text.Encoding.GetEncoding("US-ASCII").GetString(__strbuf);
 
            this.enabled = ins.ReadBoolean();
 
        }
 
        public oculuslcm.robotself_t Copy()
        {
            oculuslcm.robotself_t outobj = new oculuslcm.robotself_t();
            outobj.id = this.id;
 
            outobj.type = this.type;
 
            outobj.ability = this.ability;
 
            outobj.queryChannel = this.queryChannel;
 
            outobj.channelCount = this.channelCount;
 
            outobj.channels = new String[(int) channelCount];
            for (int a = 0; a < this.channelCount; a++) {
                outobj.channels[a] = this.channels[a];
            }
 
            outobj.leftNDIChannel = this.leftNDIChannel;
 
            outobj.rightNDIChannel = this.rightNDIChannel;
 
            outobj.enabled = this.enabled;
 
            return outobj;
        }
    }
}
