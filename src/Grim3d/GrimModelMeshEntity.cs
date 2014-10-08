using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GrimrockModelToolkit.Grim3d
{
    public class GrimModelMeshEntity : GrimObject
    {
        public GrimModelMeshData MeshData;
        public GrimModelBone[] Bones;
        public GrimVec3 EmissiveColor;
        public bool CastShadow;


        public GrimModelMeshEntity() : base() { }
        public GrimModelMeshEntity(BinaryReader reader) : base(reader) { }


        public GrimModelMeshEntity(GrimModelMeshData meshData, GrimModelBone[] bones, GrimVec3 emissiveColor, bool castShadow)
        {
            MeshData = meshData;
            Bones = bones;
            EmissiveColor = emissiveColor;
            CastShadow = castShadow;
        }


        public override void ResetToDefaultParameters()
        {
            MeshData = null;
            Bones = null;
            EmissiveColor = new GrimVec3();
            CastShadow = false;
        }


        protected override void DoRead(System.IO.BinaryReader reader)
        {
            MeshData = new GrimModelMeshData(reader);
            int NumBones = reader.ReadInt32();
            Bones = new GrimModelBone[NumBones];

            for (int i = 0; i < NumBones; i++)
            {
                Bones[i] = new GrimModelBone(reader);
            }

            EmissiveColor = new GrimVec3(reader);
            CastShadow = (reader.ReadByte() > 0);
        }


        protected override void DoWrite(System.IO.BinaryWriter writer)
        {
            MeshData.Write(writer);

            if (Bones == null)
            {
                writer.Write((Int32)0);
            }
            else
            {
                writer.Write((Int32)Bones.Length);

                for (int i = 0; i < Bones.Length; i++)
                {
                    Bones[i].Write(writer);
                }
            }

            EmissiveColor.Write(writer);
            writer.Write((CastShadow) ? (Byte)1 : (Byte)0);
        }


        public override string ToString()
        {
            int bonesLength = (Bones == null) ? 0 : Bones.Length;
            return String.Format("GrimModelMeshEntity(Bones: {0}, MeshData: {1})", bonesLength, MeshData);
        }


        public override string GetFriendlyDescription()
        {
            int boneCount = (Bones == null) ? 0 : Bones.Length;
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("Bone Count: {0} \nEmissive Color: {1} \nCast Shadow: {2} \n", boneCount, EmissiveColor, CastShadow);

            if (MeshData != null)
            {
                builder.AppendFormat("\nMesh Data\n{0}", MeshData.GetFriendlyDescription());
            }

            if (boneCount > 0)
            {
                builder.Append("\nBones\n");
                int i = 0;

                foreach( GrimModelBone bone in Bones )
                {
                    builder.AppendFormat("{0}: {1}\n", i++, bone.BoneNodeIndex);
                }
            }

            return builder.ToString();            
        }
    }
}
