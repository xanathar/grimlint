using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GrimrockModelToolkit.Grim3d
{
    public class GrimMat4x3 : GrimObject
    {
        public GrimVec3 BaseX;
        public GrimVec3 BaseY;
        public GrimVec3 BaseZ;
        public GrimVec3 Translation;


        public static GrimMat4x3 IdentityMatrix()
        {
            return new GrimMat4x3(new GrimVec3(1, 0, 0), new GrimVec3(0, 1, 0), new GrimVec3(0, 0, 1), new GrimVec3(0, 0, 0));
        }

        public GrimMat4x3() : base() { }
        public GrimMat4x3(BinaryReader reader) : base(reader) { }


        public GrimMat4x3(GrimVec3 baseX, GrimVec3 baseY, GrimVec3 baseZ, GrimVec3 translation)
        {
            BaseX = baseX;
            BaseY = baseY;
            BaseZ = baseZ;
            Translation = translation;
        }


        public override void ResetToDefaultParameters()
        {
            BaseX = new GrimVec3();
            BaseY = new GrimVec3();
            BaseZ = new GrimVec3();
            Translation = new GrimVec3();
        }


        protected override void DoRead(System.IO.BinaryReader reader)
        {
            BaseX = new GrimVec3(reader);
            BaseY = new GrimVec3(reader);
            BaseZ = new GrimVec3(reader);
            Translation = new GrimVec3(reader);
        }


        protected override void DoWrite(System.IO.BinaryWriter writer)
        {
            BaseX.Write(writer);
            BaseY.Write(writer);
            BaseZ.Write(writer);
            Translation.Write(writer);
        }


        public override string ToString()
        {
            return String.Format("[{0}, {1}, {2}, {3}]", BaseX.ToString(), BaseY.ToString(), BaseZ.ToString(), Translation.ToString());
        }
    }
}
