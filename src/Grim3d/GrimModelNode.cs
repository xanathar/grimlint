using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GrimrockModelToolkit.Grim3d
{
    public class GrimModelNode : GrimObject
    {
        public string Name;
        public GrimMat4x3 LocalToParent;
        public Int32 Parent;
        public Int32 Type;
        public GrimModelMeshEntity MeshEntity;

        public GrimModelNode() : base() { }
        public GrimModelNode(BinaryReader reader) : base(reader) { }


        public GrimModelNode(string name, GrimMat4x3 localToParent, Int32 parent, Int32 type, GrimModelMeshEntity meshEntity)
        {
            Name = name;
            LocalToParent = localToParent;
            Parent = parent;
            Type = type;
            MeshEntity = meshEntity;
        }


        public override void ResetToDefaultParameters()
        {
            Name = "";
            LocalToParent = new GrimMat4x3();
            Parent = 0;
            Type = 0;
            MeshEntity = null;
        }


        protected override void DoRead(System.IO.BinaryReader reader)
        {
            Name = ReadString(reader);
            LocalToParent = new GrimMat4x3(reader);
            Parent = reader.ReadInt32();
            Type = reader.ReadInt32();

            if (Type == 0)
            {
                MeshEntity = new GrimModelMeshEntity(reader);
            }
        }


        protected override void DoWrite(System.IO.BinaryWriter writer)
        {
            WriteString(writer, Name);
            LocalToParent.Write(writer);
            writer.Write(Parent);
            writer.Write(Type);

            if (MeshEntity != null)
            {
                MeshEntity.Write(writer);
            }
        }


        public override string ToString()
        {
            return String.Format("GrimModelNode(Name: {0}, Parent:{1}, MeshEntity: {2})", Name, Parent, MeshEntity);
        }


        public override string GetFriendlyDescription()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("Name: {0} \nLocalToParent: {1} \nParent: {2} \nType: {3}\n", Name, LocalToParent.ToString(), Parent, Type);

            if (MeshEntity != null)
            {
                builder.AppendFormat("\nMesh Entity\n{0}", MeshEntity.GetFriendlyDescription());
            }

            return builder.ToString();
        }
    }
}
