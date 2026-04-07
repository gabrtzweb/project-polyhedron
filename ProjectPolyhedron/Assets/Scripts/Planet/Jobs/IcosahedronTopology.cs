using Unity.Collections;
using Unity.Mathematics;

namespace ProjectPolyhedron.Planet.Jobs
{
    internal static class IcosahedronTopology
    {
        public static readonly int3[] TriangleFaces =
        {
            new int3(0, 11, 5),
            new int3(0, 5, 1),
            new int3(0, 1, 7),
            new int3(0, 7, 10),
            new int3(0, 10, 11),
            new int3(1, 5, 9),
            new int3(5, 11, 4),
            new int3(11, 10, 2),
            new int3(10, 7, 6),
            new int3(7, 1, 8),
            new int3(3, 9, 4),
            new int3(3, 4, 2),
            new int3(3, 2, 6),
            new int3(3, 6, 8),
            new int3(3, 8, 9),
            new int3(4, 9, 5),
            new int3(2, 4, 11),
            new int3(6, 2, 10),
            new int3(8, 6, 7),
            new int3(9, 8, 1)
        };

        public static NativeArray<float3> CreateVertices(float radius, Allocator allocator)
        {
            var vertices = new NativeArray<float3>(12, allocator, NativeArrayOptions.UninitializedMemory);
            WriteVertices(vertices, radius);
            return vertices;
        }

        public static NativeArray<int3> CreateTriangles(Allocator allocator)
        {
            var triangles = new NativeArray<int3>(TriangleFaces.Length, allocator, NativeArrayOptions.UninitializedMemory);
            for (int i = 0; i < TriangleFaces.Length; i++)
            {
                triangles[i] = TriangleFaces[i];
            }

            return triangles;
        }

        public static void WriteVertices(NativeArray<float3> vertices, float radius)
        {
            float safeRadius = radius > 0f ? radius : 50f;
            const float goldenRatio = 1.6180339887498948482f;
            float scale = safeRadius / math.sqrt(1f + (goldenRatio * goldenRatio));
            float phiScaled = goldenRatio * scale;

            vertices[0] = new float3(-scale, phiScaled, 0f);
            vertices[1] = new float3(scale, phiScaled, 0f);
            vertices[2] = new float3(-scale, -phiScaled, 0f);
            vertices[3] = new float3(scale, -phiScaled, 0f);

            vertices[4] = new float3(0f, -scale, phiScaled);
            vertices[5] = new float3(0f, scale, phiScaled);
            vertices[6] = new float3(0f, -scale, -phiScaled);
            vertices[7] = new float3(0f, scale, -phiScaled);

            vertices[8] = new float3(phiScaled, 0f, -scale);
            vertices[9] = new float3(phiScaled, 0f, scale);
            vertices[10] = new float3(-phiScaled, 0f, -scale);
            vertices[11] = new float3(-phiScaled, 0f, scale);
        }
    }
}
