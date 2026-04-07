using ProjectPolyhedron.Planet.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace ProjectPolyhedron.Planet.Jobs
{
    [BurstCompile]
    public partial struct GenerateIcosahedronVerticesJob : IJobEntity
    {
        public NativeArray<float3> Vertices;

        public void Execute(in PlanetConfig config, [EntityIndexInQuery] int entityIndexInQuery)
        {
            if (entityIndexInQuery != 0)
            {
                return;
            }

            float radius = config.Radius > 0f ? config.Radius : PlanetConfig.DefaultRadius;
            IcosahedronTopology.WriteVertices(Vertices, radius);
        }
    }
}
