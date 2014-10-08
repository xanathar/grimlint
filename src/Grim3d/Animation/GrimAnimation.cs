using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GrimrockModelToolkit.Grim3d
{
    public class GrimAnimation : GrimObject
    {
        public GrimFourCC Magic;
        public Int32 Version;
        public String AnimationName;
        public Single FramesPerSecond;
        public Int32 NumFrames;
        public GrimAnimationItem[] Items;

        // Not stored in the file, just keep here for reference
        public String OriginalFilePath;


        public static GrimAnimation LoadFromPath(String filePath)
        {
            FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(stream);
            GrimAnimation anim = new GrimAnimation(reader);
            reader.Close();
            stream.Close();

            if (anim != null)
            {
                anim.OriginalFilePath = filePath;
            }

            return anim;
        }        
        

        public GrimAnimation() : base() { }
        public GrimAnimation(BinaryReader reader) : base(reader) { }


        public GrimAnimation(String name, Single fps, Int32 numFrames, GrimAnimationItem[] items)
        {
            Magic = new GrimFourCC("ANIM");
            Version = 1;
            AnimationName = name;
            FramesPerSecond = fps;
            NumFrames = numFrames;
            Items = items;
        }


        public override void ResetToDefaultParameters()
        {
            Magic = new GrimFourCC("ANIM");
            Version = 1;
            AnimationName = "";
            FramesPerSecond = 30;
            NumFrames = 0;
            Items = null;
        }


        protected override void DoRead(System.IO.BinaryReader reader)
        {
            Magic = new GrimFourCC(reader);
            Version = reader.ReadInt32();
            AnimationName = ReadString(reader);
            FramesPerSecond = reader.ReadSingle();
            NumFrames = reader.ReadInt32();

            Int32 NumItems = reader.ReadInt32();

            if (NumItems > 0)
            {
                Items = new GrimAnimationItem[NumItems];

                for (int i = 0; i < NumItems; i++)
                {
                    Items[i] = new GrimAnimationItem(reader);
                }
            }
        }


        protected override void DoWrite(System.IO.BinaryWriter writer)
        {
            Magic.Write(writer);
            writer.Write(Version);
            WriteString(writer, AnimationName);
            writer.Write(FramesPerSecond);
            writer.Write(NumFrames);

            if (Items == null)
            {
                writer.Write((Int32)0);
            }
            else
            {
                writer.Write((Int32)Items.Length);

                for (int i = 0; i < Items.Length; i++)
                {
                    Items[i].Write(writer);
                }
            }
        }


        public override string ToString()
        {
            return String.Format("Animation: {0}, FPS:  {1}, Frames: {2}, Items: {3}", AnimationName, FramesPerSecond, NumFrames, (Items == null) ? 0 : Items.Length);
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
