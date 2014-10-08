using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GrimrockModelToolkit.Grim3d
{
    public class GrimQuaternion : GrimObject
    {
        public float X;
        public float Y;
        public float Z;
        public float W;


        public GrimQuaternion() : base() { }
        public GrimQuaternion(BinaryReader reader) : base(reader) { }


        public GrimQuaternion(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }


        public override void ResetToDefaultParameters()
        {
            X = Y = Z = W = 0.0f;
        }


        protected override void DoRead(System.IO.BinaryReader reader)
        {
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Z = reader.ReadSingle();
            W = reader.ReadSingle();
        }


        protected override void DoWrite(System.IO.BinaryWriter writer)
        {
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Z);
            writer.Write(W);
        }


        public override string ToString()
        {
            return String.Format("({0}, {1}, {2}, {3})", X, Y, Z, W);
        }
    }
}
