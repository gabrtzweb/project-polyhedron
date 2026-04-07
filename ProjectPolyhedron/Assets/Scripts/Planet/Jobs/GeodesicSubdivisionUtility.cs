using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

namespace ProjectPolyhedron.Planet.Jobs
{
    [BurstCompile]
    internal static class GeodesicSubdivisionUtility
    {
        private const float QuantizeScale = 100000f;

        internal struct GeodesicMeshData : System.IDisposable
        {
            public NativeArray<float3> Vertices;
            public NativeArray<int3> Triangles;

            public void Dispose()
            {
                if (Vertices.IsCreated)
                {
                    Vertices.Dispose();
                }

                if (Triangles.IsCreated)
                {
                    Triangles.Dispose();
                }
            }
        }

        [BurstCompile]
        public static GeodesicMeshData SubdivideToSphere(
            NativeArray<float3> baseVertices,
            NativeArray<int3> baseTriangles,
            float radius,
            int subdivisions,
            Allocator outputAllocator)
        {
            float safeRadius = radius > 0f ? radius : 50f;
            int frequency = math.max(1, subdivisions + 1);

            int estimatedVertices = 10 * frequency * frequency + 2;
            int estimatedTriangles = 20 * frequency * frequency;

            var vertices = new NativeList<float3>(estimatedVertices, Allocator.Temp);
            var triangles = new NativeList<int3>(estimatedTriangles, Allocator.Temp);
            var lookup = new NativeParallelHashMap<int3, int>(estimatedVertices, Allocator.Temp);
            var faceVertexIndices = new NativeList<int>((frequency + 1) * (frequency + 2) / 2, Allocator.Temp);

            for (int faceIndex = 0; faceIndex < baseTriangles.Length; faceIndex++)
            {
                faceVertexIndices.Clear();
                int3 face = baseTriangles[faceIndex];

                float3 a = baseVertices[face.x];
                float3 b = baseVertices[face.y];
                float3 c = baseVertices[face.z];

                for (int row = 0; row <= frequency; row++)
                {
                    float rowT = row / (float)frequency;
                    float3 start = math.lerp(a, c, rowT);
                    float3 end = math.lerp(b, c, rowT);

                    int rowCount = frequency - row + 1;
                    for (int col = 0; col < rowCount; col++)
                    {
                        float colT = rowCount == 1 ? 0f : col / (float)(rowCount - 1);
                        float3 point = math.lerp(start, end, colT);
                        float3 onSphere = math.normalize(point) * safeRadius;

                        int vertexIndex = GetOrAddVertex(onSphere, ref lookup, ref vertices);
                        faceVertexIndices.Add(vertexIndex);
                    }
                }

                int rowStart = 0;
                for (int row = 0; row < frequency; row++)
                {
                    int currentRowCount = frequency - row + 1;
                    int nextRowStart = rowStart + currentRowCount;
                    int nextRowCount = currentRowCount - 1;

                    for (int col = 0; col < nextRowCount; col++)
                    {
                        int v0 = faceVertexIndices[rowStart + col];
                        int v1 = faceVertexIndices[rowStart + col + 1];
                        int v2 = faceVertexIndices[nextRowStart + col];
                        triangles.Add(new int3(v0, v1, v2));

                        if (col < nextRowCount - 1)
                        {
                            int v3 = faceVertexIndices[nextRowStart + col + 1];
                            triangles.Add(new int3(v1, v3, v2));
                        }
                    }

                    rowStart = nextRowStart;
                }
            }

            var outputVertices = new NativeArray<float3>(vertices.Length, outputAllocator, NativeArrayOptions.UninitializedMemory);
            for (int i = 0; i < vertices.Length; i++)
            {
                outputVertices[i] = vertices[i];
            }

            var outputTriangles = new NativeArray<int3>(triangles.Length, outputAllocator, NativeArrayOptions.UninitializedMemory);
            for (int i = 0; i < triangles.Length; i++)
            {
                outputTriangles[i] = triangles[i];
            }

            faceVertexIndices.Dispose();
            lookup.Dispose();
            triangles.Dispose();
            vertices.Dispose();

            return new GeodesicMeshData
            {
                Vertices = outputVertices,
                Triangles = outputTriangles
            };
        }

        [BurstCompile]
        private static int GetOrAddVertex(
            float3 vertex,
            ref NativeParallelHashMap<int3, int> lookup,
            ref NativeList<float3> vertices)
        {
            int3 key = Quantize(vertex);
            if (lookup.TryGetValue(key, out int existingIndex))
            {
                return existingIndex;
            }

            int index = vertices.Length;
            vertices.Add(vertex);
            lookup.TryAdd(key, index);
            return index;
        }

        [BurstCompile]
        private static int3 Quantize(float3 point)
        {
            return (int3)math.round(point * QuantizeScale);
        }
    }
}
