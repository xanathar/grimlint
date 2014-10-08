using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GrimrockModelToolkit.Grim3d
{
    public class GrimModelVertexArray : GrimObject
    {
        public const int DataTypeByte = 0;
        public const int DataTypeInt16 = 1;
        public const int DataTypeInt32 = 2;
        public const int DataTypeFloat = 3;


        public GrimModelMeshData ParentMeshData;
        public Int32 DataType;                      // 0=byte, 1=int16, 2=int32, 3=float32
        public Int32 Dim;                           // Dimension of each datatype (2-4)
        public Int32 Stride;                        // Byte offset from vertex to vertex
        public byte[] RawVertexData;


        public GrimModelVertexArray(GrimModelMeshData parentMeshData) : base() 
        {
            ParentMeshData = parentMeshData;
        }

        public GrimModelVertexArray(GrimModelMeshData parentMeshData, BinaryReader reader) : base()
        {
            ResetToDefaultParameters();
            ParentMeshData = parentMeshData;
            Read(reader);
        }


        public override void ResetToDefaultParameters()
        {
            DataType = 0;
            Dim = 0;
            Stride = 0;
            RawVertexData = null;
        }


        protected override void DoRead(System.IO.BinaryReader reader)
        {
            DataType = reader.ReadInt32();
            Dim = reader.ReadInt32();
            Stride = reader.ReadInt32();
            RawVertexData = null;

            if ((ParentMeshData.NumVertices > 0) && (Stride > 0))
            {
                RawVertexData = reader.ReadBytes(ParentMeshData.NumVertices * Stride);
            }
        }


        protected override void DoWrite(System.IO.BinaryWriter writer)
        {
            writer.Write(DataType);
            writer.Write(Dim);
            writer.Write(Stride);

            if (RawVertexData != null)
            {
                writer.Write(RawVertexData);
            }
        }


        public override string ToString()
        {
            return String.Format("GrimModelVertexArray(DataType: {0}, Dim: {1}, Stride: {2}, Vertices: {3})", DataType, Dim, Stride, ParentMeshData.NumVertices);
        }


        public byte[][] getVertexDataAsByteArray()
        {
            if (DataType != DataTypeByte)
                return null;

            MemoryStream memoryStream = new MemoryStream(RawVertexData);
            BinaryReader binaryReader = new BinaryReader(memoryStream);
            byte[][] result = new byte[ParentMeshData.NumVertices][];

            for (int i = 0; i < ParentMeshData.NumVertices; i++)
            {
                result[i] = new byte[Dim];

                for (int j = 0; j < Dim; j++)
                {
                    result[i][j] = binaryReader.ReadByte();
                }
            }

            return result;
        }


        public Int16[][] getVertexDataAsInt16Array()
        {
            if (DataType != DataTypeInt16)
                return null;

            MemoryStream memoryStream = new MemoryStream(RawVertexData);
            BinaryReader binaryReader = new BinaryReader(memoryStream);
            Int16[][] result = new Int16[ParentMeshData.NumVertices][];

            for (int i = 0; i < ParentMeshData.NumVertices; i++)
            {
                result[i] = new Int16[Dim];

                for (int j = 0; j < Dim; j++)
                {
                    result[i][j] = binaryReader.ReadInt16();
                }
            }

            return result;
        }


        public Int32[][] getVertexDataAsInt32Array()
        {
            if (DataType != DataTypeInt32)
                return null;

            MemoryStream memoryStream = new MemoryStream(RawVertexData);
            BinaryReader binaryReader = new BinaryReader(memoryStream);
            Int32[][] result = new Int32[ParentMeshData.NumVertices][];

            for (int i = 0; i < ParentMeshData.NumVertices; i++)
            {
                result[i] = new Int32[Dim];

                for (int j = 0; j < Dim; j++)
                {
                    result[i][j] = binaryReader.ReadInt32();
                }
            }

            return result;
        }

        public float[][] getVertexDataAsFloatArray()
        {
            if (DataType != DataTypeFloat)
                return null;

            MemoryStream memoryStream = new MemoryStream(RawVertexData);
            BinaryReader binaryReader = new BinaryReader(memoryStream);
            float[][] result = new float[ParentMeshData.NumVertices][];

            for (int i = 0; i < ParentMeshData.NumVertices; i++)
            {
                result[i] = new float[Dim];

                for (int j = 0; j < Dim; j++)
                {
                    result[i][j] = binaryReader.ReadSingle();
                }
            }

            return result;
        }

        public GrimVec3[] getVertexDataAsVec3Array()
        {
            if (DataType != DataTypeFloat || Dim != 3)
                return null;

            MemoryStream memoryStream = new MemoryStream(RawVertexData);
            BinaryReader binaryReader = new BinaryReader(memoryStream);
            GrimVec3[] result = new GrimVec3[ParentMeshData.NumVertices];

            for (int i = 0; i < ParentMeshData.NumVertices; i++)
            {
                result[i] = new GrimVec3(binaryReader);
            }

            return result;
        }


        public void putVertexDataAsByteArray(Byte[][] data)
        {
            if (data == null)
                return;

            DataType = DataTypeByte;
            Dim = data[0].Length;
            Stride = Dim;
            RawVertexData = new byte[Stride * data.Length];

            MemoryStream memoryStream = new MemoryStream(RawVertexData);
            BinaryWriter binaryWriter = new BinaryWriter(memoryStream);

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == null)
                {
                    throw new Exception("GrimModelVertexArray: Provided float array contains a null value.");
                }

                for (int j = 0; j < Dim; j++)
                {
                    binaryWriter.Write(data[i][j]);
                }
            }
        }

        public void putVertexDataAsInt16Array(Int16[][] data)
        {
            if (data == null)
                return;

            DataType = DataTypeInt16;
            Dim = data[0].Length;
            Stride = 2 * Dim;
            RawVertexData = new byte[Stride * data.Length];

            MemoryStream memoryStream = new MemoryStream(RawVertexData);
            BinaryWriter binaryWriter = new BinaryWriter(memoryStream);

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == null)
                {
                    throw new Exception("GrimModelVertexArray: Provided float array contains a null value.");
                }

                for (int j = 0; j < Dim; j++)
                {
                    binaryWriter.Write(data[i][j]);
                }
            }
        }

        public void putVertexDataAsInt32Array(Int32[][] data)
        {
            if (data == null)
                return;

            DataType = DataTypeInt32;
            Dim = data[0].Length;
            Stride = 4 * Dim;
            RawVertexData = new byte[Stride * data.Length];

            MemoryStream memoryStream = new MemoryStream(RawVertexData);
            BinaryWriter binaryWriter = new BinaryWriter(memoryStream);

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == null)
                {
                    throw new Exception("GrimModelVertexArray: Provided float array contains a null value.");
                }

                for (int j = 0; j < Dim; j++)
                {
                    binaryWriter.Write(data[i][j]);
                }
            }
        }

        public void putVertexDataAsFloatArray(float[][] data)
        {
            if ( data == null ) 
                return;

            DataType = DataTypeFloat;
            Dim = data[0].Length;
            Stride = 4 * Dim;
            RawVertexData = new byte[Stride * data.Length];

            MemoryStream memoryStream = new MemoryStream(RawVertexData);
            BinaryWriter binaryWriter = new BinaryWriter(memoryStream);

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == null)
                {
                    throw new Exception("GrimModelVertexArray: Provided float array contains a null value.");
                }

                for( int j = 0; j < Dim; j++ )
                {
                    binaryWriter.Write(data[i][j]);
                }
            }
        }

        public void putVertexDataAsVec3Array(GrimVec3[] data)
        {
            if (data == null)
                return;

            DataType = DataTypeFloat;
            Dim = 3;
            Stride = 4 * Dim;
            RawVertexData = new byte[Stride * data.Length];

            MemoryStream memoryStream = new MemoryStream(RawVertexData);
            BinaryWriter binaryWriter = new BinaryWriter(memoryStream);

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == null)
                {
                    data[i] = new GrimVec3();
                    //throw new Exception("GrimModelVertexArray: Provided Vec3 array contains a null value.");
                }

                data[i].Write(binaryWriter);
            }
        }
    }
}
