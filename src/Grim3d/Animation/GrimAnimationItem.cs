using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GrimrockModelToolkit.Grim3d
{
    public class GrimAnimationItem : GrimObject
    {
        public string NodeName;
        public List<GrimAnimationKeyFrame> KeyFrames;

        public GrimAnimationItem() : base() { }
        public GrimAnimationItem(BinaryReader reader) : base(reader) { }


        public GrimAnimationItem(string nodeName, List<GrimAnimationKeyFrame> keyFrames)
        {
            NodeName = nodeName;
            KeyFrames = keyFrames;
        }


        public override void ResetToDefaultParameters()
        {
            NodeName = "";
            KeyFrames = null;
        }


        protected override void DoRead(System.IO.BinaryReader reader)
        {
            NodeName = ReadString(reader);
            Int32 NumKeys = reader.ReadInt32();

            if (NumKeys > 0)
            {
                KeyFrames = new List<GrimAnimationKeyFrame>(NumKeys);

                for (int i = 0; i < NumKeys; i++)
                {
                    KeyFrames.Add(new GrimAnimationKeyFrame(reader));
                }
            }
        }


        protected override void DoWrite(System.IO.BinaryWriter writer)
        {
            WriteString(writer, NodeName);

            if (KeyFrames == null)
            {
                writer.Write((Int32)0);
            }
            else
            {
                writer.Write((Int32)KeyFrames.Count);

                for (int i = 0; i < KeyFrames.Count; i++)
                {
                    KeyFrames[i].Write(writer);
                }
            }
        }


        public override string ToString()
        {
            return String.Format("AnimationItem - Bone:{0}, KeyFrames:{1}", NodeName, (KeyFrames == null) ? 0 : KeyFrames.Count);
        }
    }
}
