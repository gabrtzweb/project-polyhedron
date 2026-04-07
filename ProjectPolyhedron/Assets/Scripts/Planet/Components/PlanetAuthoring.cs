using Unity.Entities;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Collections;
using Unity.Mathematics;
using ProjectPolyhedron.Planet.Jobs;
using ProjectPolyhedron.Planet.Rendering;

namespace ProjectPolyhedron.Planet.Components
{
    public class PlanetAuthoring : MonoBehaviour
    {
        public float Radius = 50f;
        public Material Material;

        public class Baker : Baker<PlanetAuthoring>
        {
            public override void Bake(PlanetAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable);

                if (authoring.Material == null)
                {
                    Debug.LogError("PlanetAuthoring requires a material before baking.");
                    return;
                }

                float radius = authoring.Radius > 0f ? authoring.Radius : PlanetConfig.DefaultRadius;

                using var vertices = IcosahedronTopology.CreateVertices(radius, Allocator.Temp);
                using var triangles = IcosahedronTopology.CreateTriangles(Allocator.Temp);
                Mesh mesh = IcosahedronMeshUtility.CreateMesh(vertices, triangles);

                var renderMeshArray = new RenderMeshArray(new[] { authoring.Material }, new[] { mesh });
                var renderMeshDescription = new RenderMeshDescription(
                    ShadowCastingMode.On,
                    receiveShadows: true,
                    motionVectorGenerationMode: MotionVectorGenerationMode.Camera,
                    layer: authoring.gameObject.layer,
                    renderingLayerMask: 0xffffffff,
                    lightProbeUsage: LightProbeUsage.Off,
                    staticShadowCaster: false);

                AddComponent(entity, new WorldRenderBounds { Value = mesh.bounds.ToAABB() });
                AddComponent(entity, new RenderBounds { Value = mesh.bounds.ToAABB() });
                AddComponent(entity, new ChunkWorldRenderBounds { Value = mesh.bounds.ToAABB() });
                AddComponent(entity, new EntitiesGraphicsChunkInfo());
                AddComponent(entity, new WorldToLocal_Tag());
                AddComponent(entity, new PerInstanceCullingTag());
                AddComponent(entity, MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0));
                AddSharedComponent(entity, renderMeshDescription.FilterSettings);
                AddSharedComponentManaged(entity, renderMeshArray);
                AddComponent(entity, new PlanetConfig
                {
                    Radius = authoring.Radius
                });
            }
        }
    }
}