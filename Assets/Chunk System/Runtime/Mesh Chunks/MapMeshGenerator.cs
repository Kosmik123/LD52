using UnityEngine;

namespace Bipolar.ChunkSystem.MeshChunks
{
    [CreateAssetMenu(menuName = "Chunk System/Mesh Chunks/Map Mesh Generator")]
    public class MapMeshGenerator : ScriptableObject
    {
        [Header("Settings")]
        [SerializeField]
        private float heightMultiplier;

        public MapMeshData GenerateMeshData(float xScale, float zScale, float[,] map)
        {
            int zBorderedCount = map.GetLength(0);
            int xBorderedCount = map.GetLength(1);
            int xVertCount = xBorderedCount - 2;
            int zVertCount = zBorderedCount - 2;
            var meshData = new MapMeshData(xVertCount, zVertCount, true);
            float xDelta = xScale / (xVertCount - 1);
            float zDelta = zScale / (zVertCount - 1);
            float xStart = -0.5f * xScale - xDelta;
            float zStart = -0.5f * zScale - zDelta;

            meshData.map = map;
            var uvs = GetUVs(xVertCount, zVertCount);

            int[,] vertexIndicesMap = new int[xBorderedCount, zBorderedCount];
            int borderVertexIndex = -1;
            int meshVertexIndex = 0;

            for (int z = 0; z < zBorderedCount; z++)
            {
                for (int x = 0; x < xBorderedCount; x++)
                {
                    bool isBorderVertex = (x == 0 || x == (xBorderedCount - 1) || z == 0 || z == (zBorderedCount - 1));
                    if (isBorderVertex)
                    {
                        vertexIndicesMap[x, z] = borderVertexIndex;
                        borderVertexIndex--;
                    }
                    else
                    {
                        vertexIndicesMap[x, z] = meshVertexIndex;
                        meshVertexIndex++;
                    }
                }
            }

            for (int j = 0; j < zBorderedCount; j++)
            {
                for (int i = 0; i < xBorderedCount; i++)
                {
                    int vertexIndex = vertexIndicesMap[i, j];
                    float xOnMesh = xStart + i * xDelta;
                    float yOnMesh = zStart + j * zDelta;

                    float heigth = (2 * map[i, j] - 1) * heightMultiplier;

                    var vertexPosition = new Vector3(xOnMesh, heigth, yOnMesh);

                    int xUV = Mathf.Clamp(i - 1, 0, xVertCount - 1);
                    int yUV = Mathf.Clamp(j - 1, 0, zVertCount - 1);

                    meshData.AddVertex(vertexIndex, vertexPosition, uvs[xUV + xVertCount * yUV]);
                    if (i < xBorderedCount - 1 && j < zBorderedCount - 1)
                    {
                        int a = vertexIndicesMap[i, j];
                        int b = vertexIndicesMap[i, j + 1];
                        int c = vertexIndicesMap[i + 1, j + 1];
                        int d = vertexIndicesMap[i + 1, j];
                        meshData.AddQuad(a, b, c, d);
                    }
                    meshVertexIndex++;
                }
            }
            meshData.FinalizeGeneration();
            return meshData;
        }

        private static Vector2[] GetUVs(int xVertCount, int yVertCount)
        {
            float deltaX = 1f / (xVertCount - 1);
            float deltaY = 1f / (yVertCount - 1);
            Vector2[] uvs = new Vector2[xVertCount * yVertCount];
            int index = 0;
            for (int j = 0; j < yVertCount; j++)
            {
                for (int i = 0; i < xVertCount; i++)
                {
                    uvs[index] = new Vector2(i * deltaX, j * deltaY);
                    index++;
                }
            }
            return uvs;
        }
    }
}
