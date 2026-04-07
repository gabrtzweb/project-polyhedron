using ProjectPolyhedron.Planet.Components;
using ProjectPolyhedron.Planet.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace ProjectPolyhedron.Planet.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct PlanetIcosahedronGenerationSystem : ISystem
    {
        private static readonly int3[] TriangleFaces =
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

        private NativeArray<float3> _vertices;
        private NativeArray<int3> _triangles;
        private EntityQuery _planetConfigQuery;
        private float _lastRadius;

        public NativeArray<float3> Vertices => _vertices;
        public NativeArray<int3> Triangles => _triangles;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _planetConfigQuery = SystemAPI.QueryBuilder().WithAll<PlanetConfig>().Build();
            state.RequireForUpdate(_planetConfigQuery);

            // Persistent allocation keeps geometry available for downstream meshing systems.
            _vertices = new NativeArray<float3>(12, Allocator.Persistent);
            _triangles = new NativeArray<int3>(TriangleFaces.Length, Allocator.Persistent);
            _lastRadius = float.MinValue;

            var triangleJob = new GenerateIcosahedronTrianglesJob
            {
                Triangles = _triangles
            };

            state.Dependency = triangleJob.Schedule(state.Dependency);
            state.Dependency.Complete();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            if (_vertices.IsCreated)
            {
                _vertices.Dispose();
            }

            if (_triangles.IsCreated)
            {
                _triangles.Dispose();
            }
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            PlanetConfig config = SystemAPI.GetSingleton<PlanetConfig>();
            float radius = config.Radius > 0f ? config.Radius : PlanetConfig.DefaultRadius;

            // Only run the job if the radius changes
            if (math.abs(radius - _lastRadius) >= 0.0001f)
            {
                var job = new GenerateIcosahedronVerticesJob
                {
                    Vertices = _vertices
                };

                state.Dependency = job.Schedule(state.Dependency);
                _lastRadius = radius;

                state.Dependency.Complete();
            }
        }

        [BurstCompile]
        private struct GenerateIcosahedronTrianglesJob : IJob
        {
            public NativeArray<int3> Triangles;

            public void Execute()
            {
                for (int i = 0; i < TriangleFaces.Length; i++)
                {
                    Triangles[i] = TriangleFaces[i];
                }
            }
        }
    }
}
