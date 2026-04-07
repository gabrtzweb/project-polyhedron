using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace ProjectPolyhedron.Planet.Jobs
{
    [BurstCompile]
    public struct GenerateIcosahedronTrianglesJob : IJob
    {
        public NativeArray<int3> Triangles;

        public void Execute()
        {
            for (int i = 0; i < IcosahedronTopology.TriangleFaces.Length; i++)
            {
                Triangles[i] = IcosahedronTopology.TriangleFaces[i];
            }
        }
    }
}
