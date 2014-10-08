using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GrimrockModelToolkit.Grim3d
{
    public class GrimModelMeshData : GrimObject
    {
        public const int VERTEX_ARRAY_POSITION = 0;
        public const int VERTEX_ARRAY_NORMAL = 1;
        public const int VERTEX_ARRAY_TANGENT = 2;
        public const int VERTEX_ARRAY_BITANGENT = 3;
        public const int VERTEX_ARRAY_COLOR = 4;
        public const int VERTEX_ARRAY_TEXCOORD0 = 5;
        public const int VERTEX_ARRAY_TEXCOORD1 = 6;
        public const int VERTEX_ARRAY_TEXCOORD2 = 7;
        public const int VERTEX_ARRAY_TEXCOORD3 = 8;
        public const int VERTEX_ARRAY_TEXCOORD4 = 9;
        public const int VERTEX_ARRAY_TEXCOORD5 = 10;
        public const int VERTEX_ARRAY_TEXCOORD6 = 11;
        public const int VERTEX_ARRAY_TEXCOORD7 = 12;
        public const int VERTEX_ARRAY_BONE_INDEX = 13;
        public const int VERTEX_ARRAY_BONE_WEIGHT = 14;

        public GrimFourCC Magic;
        public Int32 Version;
        public Int32 NumVertices;
        public GrimModelVertexArray[] VertexArrays = new GrimModelVertexArray[15];
        public UInt32[] Indices;
        public GrimModelMeshSegment[] Segments;
        public GrimVec3 BoundCenter;
        public float BoundRadius;
        public GrimVec3 BoundMin;
        public GrimVec3 BoundMax;


        public GrimModelMeshData() : base() { }
        public GrimModelMeshData(BinaryReader reader) : base(reader) { }


        public override void ResetToDefaultParameters()
        {
            Magic = new GrimFourCC("MESH");
            Version = 2;
            NumVertices = 0;
            VertexArrays = new GrimModelVertexArray[15];
            Indices = null;
            Segments = null;
            BoundCenter = new GrimVec3();
            BoundRadius = 0.0f;
            BoundMin = new GrimVec3();
            BoundMax = new GrimVec3();

            for (int i = 0; i < VertexArrays.Length; i++)
            {
                VertexArrays[i] = new GrimModelVertexArray(this);
            }
        }


        protected override void DoRead(System.IO.BinaryReader reader)
        {
            Magic = new GrimFourCC(reader);
            Version = reader.ReadInt32();
            NumVertices = reader.ReadInt32();

            if ( !Magic.Value.Equals("MESH") ) 
            {
                throw new Exception("GrimModelMeshData: Invalid FourCC");
            }

            if ( NumVertices > 0 )
            {
                for( int i = 0; i < VertexArrays.Length; i++ )
                {
                    VertexArrays[i] = new GrimModelVertexArray(this, reader);
                }
            }

            int NumIndices = reader.ReadInt32();
            Indices = new UInt32[NumIndices];

            for (int i = 0; i < NumIndices; i++)
            {
                Indices[i] = reader.ReadUInt32();
            }

            int NumSegments = reader.ReadInt32();
            Segments = new GrimModelMeshSegment[NumSegments];

            for (int i = 0; i < NumSegments; i++)
            {
                GrimModelMeshSegment segment = new GrimModelMeshSegment(reader);

                if ((segment.TriCount * 3) > NumIndices)
                {
                    Logger.Instance.Warning("GrimModelMeshData: Too many triangles in this segment, capping to the number of triangles in the mesh.");
                    segment.TriCount = (NumIndices - segment.FirstIndex) / 3;
                }

                Segments[i] = segment;
            }

            BoundCenter = new GrimVec3(reader);
            BoundRadius = reader.ReadSingle();
            BoundMin = new GrimVec3(reader);
            BoundMax = new GrimVec3(reader);
        }


        protected override void DoWrite(System.IO.BinaryWriter writer)
        {
            Magic.Write(writer);
            writer.Write(Version);
            writer.Write(NumVertices);

            for (int i = 0; i < VertexArrays.Length; i++)
            {
                VertexArrays[i].Write(writer);
            }

            writer.Write((Int32)Indices.Length);

            for (int i = 0; i < Indices.Length; i++)
            {
                writer.Write(Indices[i]);
            }

            writer.Write((Int32)Segments.Length);

            for (int i = 0; i < Segments.Length; i++)
            {
                Segments[i].Write(writer);
            }

            BoundCenter.Write(writer);
            writer.Write(BoundRadius);
            BoundMin.Write(writer);
            BoundMax.Write(writer);
        }


        public override string ToString()
        {
            return String.Format("GrimModelMeshData(NumVertices: {0}, NumIndices: {1}, NumSegments: {2}, Bounds: [{3}, {4}])", NumVertices, Indices.Length, Segments.Length, BoundMin, BoundMax);
        }


        public override string GetFriendlyDescription()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("Vertex Count: {0} \nIndex Count: {1} \nSegment Count: {2} \n", NumVertices, Indices.Length, Segments.Length);
            builder.AppendFormat("Bound Center: {0} \nBound Min: {1} \nBound Max: {2} \nBound Radius: {3}\n", BoundCenter, BoundMin, BoundMax, BoundRadius);

            builder.AppendFormat("Vertex Arrays: \n");
            
            for (int i = 0; i < 15; i++)
            {
                GrimModelVertexArray va = VertexArrays[i];
                builder.AppendFormat(" {0}: {1}, {2}, {3}, {4}\n", i, va.DataType, va.Dim, va.Stride, (va.RawVertexData != null) ? va.RawVertexData.Length : 0);
            }

            builder.AppendFormat("\n");

            int invalidIndicesCount = 0;

            for (int i = 0; i < Indices.Length; i++)
            {
                if (Indices[i] < 0 || Indices[i] > NumVertices)
                {
                    invalidIndicesCount++;
                }
            }

            builder.AppendFormat("{0} Invalid Indices", invalidIndicesCount);

            return builder.ToString();
        }


        /// <summary>
        /// Destroy the normal data for this mesh and recalculate them using a simple vertex averaging system.
        /// </summary>
        public void CalculateAverageVertexNormals(bool simpleWeighting = true)
        {
            GrimModelVertexArray vertexArray = VertexArrays[VERTEX_ARRAY_POSITION];
            float[][] vertexData = vertexArray.getVertexDataAsFloatArray();

            if ( vertexData == null ) 
            {
                Logger.Instance.Error("No position vertex data in existing mesh.");
                return;
            }

            List<GrimVec3>[] vertexValues = new List<GrimVec3>[vertexData.Length];
            float[][] normalData = new float[vertexData.Length][];

            // Initialise all of the normals incase we don't compute any (the data needs to be the right size)
            for (int i = 0; i < vertexData.Length; i++)
            {
                normalData[i] = new float[3];
            }

            // Calculate the normal for each triangle and store that value for each vertex of that face (some vertices will have multiple values in a list).
            // We set each Normal Pt index to the same index as the vertex, as we are making these normals from scratch - we might as well make them parallel.
            foreach (GrimModelMeshSegment segment in Segments)
            {
                // Reset all of the values for this segment
                for (int i = 0; i < vertexData.Length; i++)
                {
                    vertexValues[i] = new List<GrimVec3>();
                }

                // Calculate the range of indices we are working with
                int firstIndex = segment.FirstIndex;
                int lastIndex = firstIndex + segment.TriCount * 3;

                // Iterate over each vertex and calculate a normal at that point for each face it is used in
                for (int i = firstIndex; i < lastIndex; i += 3)
                {
                    uint[] triIndices = new uint[] { Indices[i], Indices[i+1], Indices[i+2] };
                    GrimVec3[] verts = new GrimVec3[] { new GrimVec3(vertexData[triIndices[0]]), new GrimVec3(vertexData[triIndices[1]]), new GrimVec3(vertexData[triIndices[2]]) };
                    GrimVec3 normal = GrimVec3.Cross((verts[1] - verts[0]), (verts[2] - verts[0]));

                    // Apply Weighting
                    if (simpleWeighting)
                    {
                        for( int j = 0; j < 3; j++ ) 
                        {
                            GrimVec3 a = verts[(j + 1) % 3] - verts[j];
                            GrimVec3 b = verts[(j + 2) % 3] - verts[j];
                            float weight = (float)Math.Acos(GrimVec3.Dot(a,b) / (a.Length() * b.Length()));
                            normal *= weight;
                        }
                    }

                    vertexValues[triIndices[0]].Add(normal);
                    vertexValues[triIndices[1]].Add(normal);
                    vertexValues[triIndices[2]].Add(normal);
                }

                for (int i = 0; i < vertexValues.Length; i++)
                {
                    List<GrimVec3> vertexNormals = vertexValues[i];

                    if (vertexNormals.Count == 0)
                        continue;

                    GrimVec3 normal = new GrimVec3();

                    for (int j = 0; j < vertexNormals.Count; j++)
                    {
                        normal = normal + vertexNormals[j];
                    }

                    normal.Normalize();
                    normalData[i] = normal.ToFloatArray();
                }
            }

            VertexArrays[VERTEX_ARRAY_NORMAL].putVertexDataAsFloatArray(normalData);
        }


        /// <summary>
        /// Calculate tangents and bitangents for each vertex in order using the method described at
        /// http://www.terathon.com/code/tangent.html
        /// </summary>
        public void CalculateTangentsAndBitangents()
        {
            GrimModelVertexArray vertexArray = VertexArrays[VERTEX_ARRAY_POSITION];
            GrimModelVertexArray texArray = VertexArrays[VERTEX_ARRAY_TEXCOORD0];
            GrimModelVertexArray normalArray = VertexArrays[VERTEX_ARRAY_NORMAL];
            float[][] vertexData = vertexArray.getVertexDataAsFloatArray();
            float[][] texData = texArray.getVertexDataAsFloatArray();
            GrimVec3[] normalData = normalArray.getVertexDataAsVec3Array();

            if ( vertexData == null || texData == null || normalData == null ) 
            {
                Logger.Instance.Error("Need position, normal and texture data in existing mesh to calculate tangents.");
                return;
            }

            List<GrimVec3>[] vertexTangents = new List<GrimVec3>[vertexData.Length];
            List<GrimVec3>[] vertexBitangents = new List<GrimVec3>[vertexData.Length];
            GrimVec3[] tangentData = new GrimVec3[vertexData.Length];
            GrimVec3[] bitangentData = new GrimVec3[vertexData.Length];

            foreach (GrimModelMeshSegment segment in Segments)
            {
                // Reset all of the values for this segment
                for (int i = 0; i < vertexData.Length; i++)
                {
                    vertexTangents[i] = new List<GrimVec3>();
                    vertexBitangents[i] = new List<GrimVec3>();
                }

                // Calculate the range of indices we are working with
                int firstIndex = segment.FirstIndex;
                int lastIndex = firstIndex + segment.TriCount * 3;

                // Go over each triangle in this segment and calculate tangent / bitangent data for the related vertices
                for (int i = firstIndex; i < lastIndex; i += 3)
                {
                    uint[] triIndices = new uint[] { Indices[i], Indices[i + 1], Indices[i + 2] };
                    GrimVec3[] triVerts = new GrimVec3[] { new GrimVec3(vertexData[triIndices[0]]), new GrimVec3(vertexData[triIndices[1]]), new GrimVec3(vertexData[triIndices[2]]) };
                    float[][] triUVs = new float[][] { texData[triIndices[0]], texData[triIndices[1]], texData[triIndices[2]] };

                    GrimVec3 side0 = triVerts[1] - triVerts[0];
                    GrimVec3 side1 = triVerts[2] - triVerts[0];

                    float dU0 = triUVs[1][0] - triUVs[0][0];
                    float dU1 = triUVs[2][0] - triUVs[0][0];
                    float dV0 = triUVs[1][1] - triUVs[0][1];
                    float dV1 = triUVs[2][1] - triUVs[0][1];

                    GrimVec3 tangent = dV1 * side0 - dV0 * side1;
                    GrimVec3 bitangent = dU1 * side0 - dU0 * side1;
                    GrimVec3 tangentCross = GrimVec3.Cross(tangent, bitangent);
                    
                    // Normalize at the end now
                    // tangent.Normalize();
                    // bitangent.Normalize();

                    //wind the tangent into the other direction if necassary; means: if the UVs are mirrored
                    //http://www.opentk.com/node/2520
                    /*
                    if (GrimVec3.Dot(surfaceNormal, tangentCross) < 0.0f)
                    {
                        biTangent *= -1.0f;
                        tangent *= -1.0f;
                    }
                    */

                    for (int j = 0; j < 3; j++)
                    {
                        vertexTangents[triIndices[j]].Add(tangent);
                        vertexBitangents[triIndices[j]].Add(bitangent);
                    }
                }

                // Go over each vertex, see if we have created 1+ tangents for that vertex and do some maths if yes.
                for (int i = 0; i < vertexData.Length; i++)
                {
                    List<GrimVec3> t = vertexTangents[i];
                    List<GrimVec3> b = vertexBitangents[i];

                    if (t.Count != b.Count || t.Count == 0)
                    {
                        continue;
                    }

                    tangentData[i] = new GrimVec3();
                    bitangentData[i] = new GrimVec3();

                    for (int j = 0; j < t.Count; j++)
                    {
                        tangentData[i] = tangentData[i] + t[j];
                        bitangentData[i] = bitangentData[i] + b[j];
                    }

                    tangentData[i] = (tangentData[i] - (normalData[i] * GrimVec3.Dot(normalData[i], tangentData[i])));
                    bitangentData[i] = (bitangentData[i] - (normalData[i] * GrimVec3.Dot(normalData[i], bitangentData[i])));

                    tangentData[i].Normalize();
                    bitangentData[i].Normalize();
                }

            }

            VertexArrays[VERTEX_ARRAY_TANGENT].putVertexDataAsVec3Array(tangentData);
            VertexArrays[VERTEX_ARRAY_BITANGENT].putVertexDataAsVec3Array(bitangentData);
        }
    }
}
