using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace GrimrockModelToolkit.Grim3d
{
    public class GrimVec3 : GrimObject
    {
        public float X;
        public float Y;
        public float Z;


        public GrimVec3() : base() { }
        public GrimVec3(BinaryReader reader) : base(reader) { }


        public GrimVec3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }


        public GrimVec3(float[] val) : base()
        {
            if (val == null || val.Length < 3)
                return;

            X = val[0];
            Y = val[1];
            Z = val[2];
        }


        public override void ResetToDefaultParameters()
        {
            X = Y = Z = 0.0f;
        }


        protected override void DoRead(System.IO.BinaryReader reader)
        {
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Z = reader.ReadSingle();
        }


        protected override void DoWrite(System.IO.BinaryWriter writer)
        {
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Z);
        }


        public override string ToString()
        {
            return String.Format("({0}, {1}, {2})", X, Y, Z);
        }


        public static GrimVec3 operator +(GrimVec3 op1, GrimVec3 op2)
        {
            return new GrimVec3(op1.X + op2.X, op1.Y + op2.Y, op1.Z + op2.Z);
        }

        public static GrimVec3 operator -(GrimVec3 op1, GrimVec3 op2)
        {
            return new GrimVec3(op1.X - op2.X, op1.Y - op2.Y, op1.Z - op2.Z);
        }

        public static GrimVec3 operator *(GrimVec3 op1, float op2)
        {
            return new GrimVec3(op1.X * op2, op1.Y * op2, op1.Z * op2);
        }

        public static GrimVec3 operator *(float op1, GrimVec3 op2)
        {
            return op2 * op1;
        }

        public static GrimVec3 operator /(GrimVec3 op1, float op2)
        {
            return new GrimVec3(op1.X / op2, op1.Y / op2, op1.Z / op2);
        }

        public static GrimVec3 Cross(GrimVec3 left, GrimVec3 right)
        {
            GrimVec3 returnValue = new GrimVec3();
            returnValue.X = left.Y * right.Z - left.Z * right.Y;
            returnValue.Y = left.Z * right.X - left.X * right.Z;
            returnValue.Z = left.X * right.Y - left.Y * right.X;
            return returnValue;
        }

        public static float Dot(GrimVec3 left, GrimVec3 right)
        {
            return (left.X * right.X) + (left.Y * right.Y) + (left.Z * right.Z);
        }

        public float[] ToFloatArray()
        {
            return new float[3] { X, Y, Z };
        }

        public float LengthSquared()
        {
            return (X * X) + (Y * Y) + (Z * Z);
        }

        public float Length()
        {
            return (float)Math.Sqrt(LengthSquared());
        }

        public void Normalize()
        {
            float len = Length();
            X = X / len;
            Y = Y / len;
            Z = Z / len;
        }
    }
}
