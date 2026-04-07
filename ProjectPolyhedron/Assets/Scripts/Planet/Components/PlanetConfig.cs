using Unity.Entities;

namespace ProjectPolyhedron.Planet.Components
{
    public struct PlanetConfig : IComponentData
    {
        public const float DefaultRadius = 50f;
        public float Radius;
    }
}
