using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GrimrockModelToolkit.Grim3d
{
    public class GrimModel : GrimObject
    {
        public GrimFourCC Magic;
        public Int32 Version;
        public List<GrimModelNode> Nodes = new List<GrimModelNode>();


        public static GrimModel LoadFromPath(String filePath)
        {
            FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(stream);
            GrimModel model = new GrimModel(reader);
            reader.Close();
            stream.Close();

            return model;
        }


        public GrimModel() : base() { }
        public GrimModel(BinaryReader reader) : base(reader) { }


        public override void ResetToDefaultParameters()
        {
            Magic = new GrimFourCC("MDL1");
            Version = 2;
            Nodes.Clear();

            // Add RootNode
            Nodes.Add(new GrimModelNode("RootNode", GrimMat4x3.IdentityMatrix(), -1, -1, null));
        }


        protected override void DoRead(System.IO.BinaryReader reader)
        {
            Magic = new GrimFourCC(reader);

            if (!Magic.Value.Equals("MDL1"))
            {
                throw new Exception("Invalid FourCC. Expected MDL1");
            }

            Version = reader.ReadInt32();
            int NumNodes = reader.ReadInt32();
            Nodes.Clear();

            for (int i = 0; i < NumNodes; i++)
            {
                Nodes.Add(new GrimModelNode(reader));
            }
        }


        protected override void DoWrite(System.IO.BinaryWriter writer)
        {
            Magic.Write(writer);
            writer.Write(Version);
            writer.Write((Int32)Nodes.Count);

            foreach (GrimModelNode node in Nodes)
            {
                node.Write(writer);
            }
        }


        public override string ToString()
        {
            return String.Format("GrimModel(Nodes: {0})", Nodes.Count);
        }


        public void WriteToPath(String filePath)
        {
            FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            BinaryWriter writer = new BinaryWriter(stream);
            Write(writer);
            writer.Close();
            stream.Close();
        }
    }
}
