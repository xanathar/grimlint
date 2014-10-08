using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GrimrockModelToolkit.Grim3d
{
    public class GrimAnimationKeyFrame : GrimObject
    {
        public GrimVec3 Position;
        public GrimQuaternion Rotation;
        public GrimVec3 Scale;

        public GrimAnimationKeyFrame() : base() { }
        public GrimAnimationKeyFrame(BinaryReader reader) : base(reader) { }


        public GrimAnimationKeyFrame(GrimVec3 pos, GrimQuaternion rot, GrimVec3 scale)
        {
            Position = pos;
            Rotation = rot;
            Scale = scale;
        }


        public override void ResetToDefaultParameters()
        {
            Position = new GrimVec3();
            Rotation = new GrimQuaternion();
            Scale = new GrimVec3();
        }


        protected override void DoRead(System.IO.BinaryReader reader)
        {
            Position = new GrimVec3(reader);
            Rotation = new GrimQuaternion(reader);
            Scale = new GrimVec3(reader);
        }


        protected override void DoWrite(System.IO.BinaryWriter writer)
        {
            Position.Write(writer);
            Rotation.Write(writer);
            Scale.Write(writer);
        }


        public override string ToString()
        {
            return String.Format("KeyFrame: {0}, {1}, {2}", Position, Rotation, Scale);
        }
    }
}
