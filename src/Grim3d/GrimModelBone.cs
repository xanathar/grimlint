using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GrimrockModelToolkit.Grim3d
{
    public class GrimModelBone : GrimObject
    {
        public Int32 BoneNodeIndex;
        public GrimMat4x3 InvRestMatrix;


        public GrimModelBone() : base() { }
        public GrimModelBone(BinaryReader reader) : base(reader) { }


        public GrimModelBone(Int32 boneNodeIndex, GrimMat4x3 invRestMatrix)
        {
            BoneNodeIndex = boneNodeIndex;
            InvRestMatrix = invRestMatrix;
        }


        public override void ResetToDefaultParameters()
        {
            BoneNodeIndex = 0;
            InvRestMatrix = new GrimMat4x3();
        }


        protected override void DoRead(System.IO.BinaryReader reader)
        {
            BoneNodeIndex = reader.ReadInt32();
            InvRestMatrix = new GrimMat4x3(reader);
        }


        protected override void DoWrite(System.IO.BinaryWriter writer)
        {
            writer.Write(BoneNodeIndex);
            InvRestMatrix.Write(writer);
        }


        public override string ToString()
        {
            return String.Format("GrimModelBone({0}, {1})", BoneNodeIndex, InvRestMatrix);
        }
    }
}
