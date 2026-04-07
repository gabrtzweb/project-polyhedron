using ProjectPolyhedron.Planet.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace ProjectPolyhedron.Planet.Rendering
{
    internal static class IcosahedronMeshUtility
    {
        public static Mesh CreateMesh(NativeArray<float3> vertices, NativeArray<int3> triangles)
        {
            var mesh = new Mesh
            {
                name = "Icosahedron"
            };

            var meshVertices = new Vector3[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                float3 vertex = vertices[i];
                meshVertices[i] = new Vector3(vertex.x, vertex.y, vertex.z);
            }

            var triangleIndices = new int[triangles.Length * 3];
            for (int i = 0; i < triangles.Length; i++)
            {
                int3 triangle = triangles[i];
                int baseIndex = i * 3;
                triangleIndices[baseIndex] = triangle.x;
                triangleIndices[baseIndex + 1] = triangle.y;
                triangleIndices[baseIndex + 2] = triangle.z;
            }

            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt16;
            mesh.vertices = meshVertices;
            mesh.triangles = triangleIndices;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }
    }
}
