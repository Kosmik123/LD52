using System;
using UnityEngine;

namespace Bipolar.ChunkSystem.MeshChunks
{
    public class MapMeshData
    {
        public int xVertCount;
        public int zVertCount;

        private Vector3[] vertices;
        private readonly int[] triangles;
        public Vector2[] uvs;
        private Vector3[] normals;
        public float[,] map;
        public float[,] Map => map;

        private Vector3[] borderVertices;
        private int[] borderTriangles;

        private int triangleIndex;
        private int borderTriangleIndex;

        private bool flatShading;

        public MapMeshData(int xVertCount, int zVertCount, bool useFlatShading = false)
        {
            this.xVertCount = xVertCount;
            this.zVertCount = zVertCount;

            vertices = new Vector3[xVertCount * zVertCount];
            triangles = new int[(xVertCount - 1) * (zVertCount - 1) * 6];

            borderVertices = new Vector3[2 * (xVertCount + zVertCount) + 4];
            borderTriangles = new int[12 * (xVertCount + zVertCount) + 24];

            uvs = new Vector2[xVertCount * zVertCount];
            flatShading = useFlatShading;
            triangleIndex = 0;
        }

        public void AddVertex(int index, Vector3 position, Vector2 uv)
        {
            if (index < 0)
            {
                borderVertices[-index - 1] = position;
            }
            else
            {
                vertices[index] = position;
                uvs[index] = uv;
            }
        }

        public void AddTriangle(int a, int b, int c)
        {
            if (a < 0 || b < 0 || c < 0)
            {
                borderTriangles[borderTriangleIndex] = a;
                borderTriangles[borderTriangleIndex + 1] = b;
                borderTriangles[borderTriangleIndex + 2] = c;
                borderTriangleIndex += 3;
            }
            else
            {
                triangles[triangleIndex] = a;
                triangles[triangleIndex + 1] = b;
                triangles[triangleIndex + 2] = c;
                triangleIndex += 3;
            }
        }

        public void AddQuad(int a, int b, int c, int d)
        {
            AddTriangle(b, c, a);
            AddTriangle(a, c, d);
        }

        private Vector3[] CalculateNormals()
        {
            Vector3[] normals = new Vector3[vertices.Length];
            int triangleCount = triangles.Length / 3;
            for (int i = 0; i < triangleCount; i++)
            {
                int normalTriangleIndex = i * 3;
                int vertexIndexA = triangles[normalTriangleIndex];
                int vertexIndexB = triangles[normalTriangleIndex + 1];
                int vertexIndexC = triangles[normalTriangleIndex + 2];

                var triangleNormal = GetTriangleNormal(vertexIndexA, vertexIndexB, vertexIndexC);
                normals[vertexIndexA] += triangleNormal;
                normals[vertexIndexB] += triangleNormal;
                normals[vertexIndexC] += triangleNormal;
            }

            int borderTriangleCount = borderTriangles.Length / 3;
            for (int i = 0; i < borderTriangleCount; i++)
            {
                int normalTriangleIndex = i * 3;
                int vertexIndexA = borderTriangles[normalTriangleIndex];
                int vertexIndexB = borderTriangles[normalTriangleIndex + 1];
                int vertexIndexC = borderTriangles[normalTriangleIndex + 2];

                var triangleNormal = GetTriangleNormal(vertexIndexA, vertexIndexB, vertexIndexC);
                if (vertexIndexA >= 0)
                    normals[vertexIndexA] += triangleNormal;
                if (vertexIndexB >= 0)
                    normals[vertexIndexB] += triangleNormal;
                if (vertexIndexC >= 0) 
                    normals[vertexIndexC] += triangleNormal;
            }

            for (int i = 0; i < normals.Length; i++)
                normals[i].Normalize();

            return normals;
        }

        private Vector3 GetTriangleNormal(int a, int b, int c)
        {
            var pointA = a < 0 ?  borderVertices [-a-1] : vertices[a];
            var pointB = b < 0 ?  borderVertices [-b-1] : vertices[b];
            var pointC = c < 0 ?  borderVertices [-c-1] : vertices[c];
            
            var sideAB = pointB - pointA;
            var sideAC = pointC - pointA;
            return Vector3.Cross(sideAB, sideAC).normalized;
        }

        public void FinalizeGeneration()
        {
            if (flatShading)
                ApplyFlatShading();
            else
                normals = CalculateNormals();
        }

        private void ApplyFlatShading()
        {
            var flatVertices = new Vector3[triangles.Length];
            var flatUVs = new Vector2[triangles.Length];
            for (int i = 0; i < triangles.Length; i++)
            {
                flatVertices[i] = vertices[triangles[i]];
                flatUVs[i] = uvs[triangles[i]];
                triangles[i] = i;
            }
            vertices = flatVertices;
            uvs = flatUVs;
        }

        public Mesh ToMesh()
        {
            var mesh = new Mesh
            {
                vertices = vertices,
                triangles = triangles,
                uv = uvs,
            };
            if (flatShading)
                mesh.RecalculateNormals();
            else
                mesh.normals = normals;
            return mesh;
        }
    }
}
