using Unity.Entities;

namespace ProjectPolyhedron.Planet.Components
{
    public struct PlanetConfig : IComponentData
    {
        public const float DefaultRadius = 50f;
        public const int DefaultSubdivisions = 3;

        public float Radius;
        public int Subdivisions;
    }
}
