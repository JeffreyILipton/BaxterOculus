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
    public sealed class pose_t : LCM.LCM.LCMEncodable
    {
        public double[] position;
        public double[] orientation;
        public bool enabled;
 
        public pose_t()
        {
            position = new double[3];
            orientation = new double[4];
        }
 
        public static readonly ulong LCM_FINGERPRINT;
        public static readonly ulong LCM_FINGERPRINT_BASE = 0xea8a3a46f83c6991L;
 
        static pose_t()
        {
            LCM_FINGERPRINT = _hashRecursive(new List<String>());
        }
 
        public static ulong _hashRecursive(List<String> classes)
        {
            if (classes.Contains("oculuslcm.pose_t"))
                return 0L;
 
            classes.Add("oculuslcm.pose_t");
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
            for (int a = 0; a < 3; a++) {
                outs.Write(this.position[a]); 
            }
 
            for (int a = 0; a < 4; a++) {
                outs.Write(this.orientation[a]); 
            }
 
            outs.Write(this.enabled); 
 
        }
 
        public pose_t(byte[] data) : this(new LCMDataInputStream(data))
        {
        }
 
        public pose_t(LCMDataInputStream ins)
        {
            if ((ulong) ins.ReadInt64() != LCM_FINGERPRINT)
                throw new System.IO.IOException("LCM Decode error: bad fingerprint");
 
            _decodeRecursive(ins);
        }
 
        public static oculuslcm.pose_t _decodeRecursiveFactory(LCMDataInputStream ins)
        {
            oculuslcm.pose_t o = new oculuslcm.pose_t();
            o._decodeRecursive(ins);
            return o;
        }
 
        public void _decodeRecursive(LCMDataInputStream ins)
        {
            this.position = new double[(int) 3];
            for (int a = 0; a < 3; a++) {
                this.position[a] = ins.ReadDouble();
            }
 
            this.orientation = new double[(int) 4];
            for (int a = 0; a < 4; a++) {
                this.orientation[a] = ins.ReadDouble();
            }
 
            this.enabled = ins.ReadBoolean();
 
        }
 
        public oculuslcm.pose_t Copy()
        {
            oculuslcm.pose_t outobj = new oculuslcm.pose_t();
            outobj.position = new double[(int) 3];
            for (int a = 0; a < 3; a++) {
                outobj.position[a] = this.position[a];
            }
 
            outobj.orientation = new double[(int) 4];
            for (int a = 0; a < 4; a++) {
                outobj.orientation[a] = this.orientation[a];
            }
 
            outobj.enabled = this.enabled;
 
            return outobj;
        }
    }
}

