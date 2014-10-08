using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GrimrockModelToolkit.Grim3d
{
    public class GrimFourCC : GrimObject
    {
        private string mValue;
        public string Value
        {
            get { return mValue; }
            set { mValue = value; }
        }

        public GrimFourCC() : base() { }
        public GrimFourCC(BinaryReader reader) : base(reader) { }


        public GrimFourCC(string value)
        {
            Value = value;
        }


        public override void ResetToDefaultParameters()
        {
            mValue = "    ";
        }


        protected override void DoRead(System.IO.BinaryReader reader)
        {
            char[] fourCC = reader.ReadChars(4);
            Value = new String(fourCC);
        }


        protected override void DoWrite(System.IO.BinaryWriter writer)
        {
            if (mValue.Length < 4)
            {
                mValue = mValue.PadLeft(4, ' ');
            }
            else if (mValue.Length > 4)
            {
                mValue = mValue.Substring(0, 4);
            }

            char[] fourCC = mValue.ToCharArray();
            writer.Write(fourCC);
        }
    }
}
