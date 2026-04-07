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

            const float goldenRatio = 1.6180339887498948482f;
            float scale = radius / math.sqrt(1f + (goldenRatio * goldenRatio));
            float phiScaled = goldenRatio * scale;

            Vertices[0] = new float3(-scale, phiScaled, 0f);
            Vertices[1] = new float3(scale, phiScaled, 0f);
            Vertices[2] = new float3(-scale, -phiScaled, 0f);
            Vertices[3] = new float3(scale, -phiScaled, 0f);

            Vertices[4] = new float3(0f, -scale, phiScaled);
            Vertices[5] = new float3(0f, scale, phiScaled);
            Vertices[6] = new float3(0f, -scale, -phiScaled);
            Vertices[7] = new float3(0f, scale, -phiScaled);

            Vertices[8] = new float3(phiScaled, 0f, -scale);
            Vertices[9] = new float3(phiScaled, 0f, scale);
            Vertices[10] = new float3(-phiScaled, 0f, -scale);
            Vertices[11] = new float3(-phiScaled, 0f, scale);
        }
    }
}
