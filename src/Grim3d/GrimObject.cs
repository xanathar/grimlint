using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GrimrockModelToolkit.Grim3d
{
    public class GrimObject
    {
        #if DEBUG
        protected int mDebugLevel = 0;
        #else
        protected int mDebugLevel = 0;
        #endif

        public int DebugLevel
        {
            get { return mDebugLevel; }
            set { mDebugLevel = value; }
        }

        
        public GrimObject()
        {
            ResetToDefaultParameters();
        }


        public GrimObject(BinaryReader reader)
        {
            ResetToDefaultParameters();
            Read(reader);
        }


        public virtual void ResetToDefaultParameters()
        {

        }


        protected void DebugLog(string message)
        {
            //if (mDebugLevel > 0)
            //{
                Logger.Instance.Verbose(this.GetType().Name + " [Log]: " + message);
            //}
        }


        protected void DebugError(string message)
        {
            Logger.Instance.Error(this.GetType().Name + " [Error]: " + message);
            /*
            if (mDebugLevel > 0)
            {
                throw new Exception(message);
            }
             */
        }


        public void Read(BinaryReader reader)
        {
            if (reader == null)
            {
                DebugError("Binary Reader is null");
                return;
            }

            try
            {
                DoRead(reader);
            }
            catch (Exception ex)
            {
                DebugError("Failed Read Operation: " + ex.Message);
                return;
            }

            DebugLog(String.Format("Read [{0:X8}]: {1}", reader.BaseStream.Position, this.ToString()));
        }


        protected virtual void DoRead(BinaryReader reader)
        {
            throw new Exception("DoRead Not Defined");
        }


        public void Write(BinaryWriter writer)
        {
            if (writer == null)
            {
                DebugError("Binary Writer is null");
                return;
            }

            try
            {
                DoWrite(writer);
            }
            catch (Exception ex)
            {
                DebugError("Failed Write Operation: " + ex.Message);
                return;
            }

            //DebugLog(String.Format("Wrote [{0:X8}]: {1}", writer.BaseStream.Position, this.ToString()));
        }


        protected virtual void DoWrite(BinaryWriter writer)
        {
            throw new Exception("DoWrite Not Defined");
        }


        protected String ReadString(BinaryReader reader)
        {
            int length = reader.ReadInt32();
            char[] chars = reader.ReadChars(length);
            return new String(chars);
        }


        protected void WriteString(BinaryWriter writer, String str)
        {
            int length = str.Length;
            char[] chars = str.ToCharArray();
            writer.Write(length);
            writer.Write(chars);
        }


        public virtual string GetFriendlyDescription()
        {
            return "No Description Available";
        }

    }
}
