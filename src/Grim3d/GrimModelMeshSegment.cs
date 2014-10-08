using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GrimrockModelToolkit.Grim3d
{
    public class GrimModelMeshSegment : GrimObject
    {
        public String Material;
        public Int32 PrimitiveType;
        public Int32 FirstIndex;
        public Int32 TriCount;


        public GrimModelMeshSegment() : base() { }
        public GrimModelMeshSegment(BinaryReader reader) : base(reader) { }


        public override void ResetToDefaultParameters()
        {
            Material = "";
            PrimitiveType = 2;
            FirstIndex = 0;
            TriCount = 0;
        }


        protected override void DoRead(System.IO.BinaryReader reader)
        {
            Material = ReadString(reader);
            PrimitiveType = reader.ReadInt32();
            FirstIndex = reader.ReadInt32();
            TriCount = reader.ReadInt32();
        }


        protected override void DoWrite(System.IO.BinaryWriter writer)
        {
            WriteString(writer, Material);
            writer.Write(PrimitiveType);
            writer.Write(FirstIndex);
            writer.Write(TriCount);
        }


        public override string ToString()
        {
            return String.Format("GrimModelMeshSegment(Material: {0}, Type: {1}, FirstIndex: {2}, TriCount: {3})", Material, PrimitiveType, FirstIndex, TriCount);
        }


        public override string GetFriendlyDescription()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("Material: {0} \nPrimitive Type: {1} \nFirst Index: {2} \nTriCount: {3}", Material, PrimitiveType, FirstIndex, TriCount);
            return builder.ToString();
        }
    }
}
