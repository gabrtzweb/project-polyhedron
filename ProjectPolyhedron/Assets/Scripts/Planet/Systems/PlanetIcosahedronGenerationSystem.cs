using ProjectPolyhedron.Planet.Components;
using ProjectPolyhedron.Planet.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace ProjectPolyhedron.Planet.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct PlanetIcosahedronGenerationSystem : ISystem
    {
        private NativeArray<float3> _vertices;
        private EntityQuery _planetConfigQuery;
        private float _lastRadius;

        public NativeArray<float3> Vertices => _vertices;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _planetConfigQuery = SystemAPI.QueryBuilder().WithAll<PlanetConfig>().Build();
            state.RequireForUpdate(_planetConfigQuery);

            // Persistent allocation keeps geometry available for downstream meshing systems.
            _vertices = new NativeArray<float3>(12, Allocator.Persistent);
            _lastRadius = float.MinValue;
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            if (_vertices.IsCreated)
            {
                _vertices.Dispose();
            }
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            PlanetConfig config = SystemAPI.GetSingleton<PlanetConfig>();
            float radius = config.Radius > 0f ? config.Radius : PlanetConfig.DefaultRadius;

            if (math.abs(radius - _lastRadius) < 0.0001f)
            {
                return;
            }

            var job = new GenerateIcosahedronVerticesJob
            {
                Vertices = _vertices
            };

            state.Dependency = job.Schedule(state.Dependency);
            _lastRadius = radius;
        }
    }
}
