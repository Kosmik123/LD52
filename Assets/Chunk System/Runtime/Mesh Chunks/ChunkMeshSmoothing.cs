using Bipolar.ChunkSystem.Generation;
using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.ChunkSystem.MeshChunks
{


    public class ChunkMeshSmoothing : Smoothing
    {
        [SerializeField]
        private ChunkMesh chunkMesh;

        private ChunksData data;

        private readonly Dictionary<Vector2Int, Chunk> neighbourChunks = new Dictionary<Vector2Int, Chunk>(8);
        private readonly Dictionary<Vector2Int, ChunkMesh> neighbourChunkMeshes = new Dictionary<Vector2Int, ChunkMesh>(8);

        private Vector2Int index;

        #region PUBLIC
        public override void ModifyMap(ref float[,] map)
        {
            SmoothMap(ref map);
        }

        private void SmoothMap(ref float[,] map)
        {
            int mapWidth = map.GetLength(0);
            int mapHeight = map.GetLength(1);

            int xSeamVertsCount = Mathf.RoundToInt(mapWidth * settings.SeamLength);
            int ySeamVertsCount = Mathf.RoundToInt(mapHeight * settings.SeamLength);

            var smoothingSides = SmoothDirection.None;
            for (int j = -1; j <= 1; j++)
            {
                for (int i = -1; i <= 1; i++)
                {
                    var direction = i == -1 ? SmoothDirection.AnyLeft : i == 1 ? SmoothDirection.AnyRight : SmoothDirection.AnyCenter;
                    direction &= j == -1 ? SmoothDirection.AnyBottom : j == 1 ? SmoothDirection.AnyTop : SmoothDirection.AnyCenter;

                    if (i == 0 && j == 0)
                        continue;

                    if (neighbourChunks.TryGetValue(new Vector2Int(i, j), out var neighbour))
                    {
                        if (neighbour == null)
                            continue;

                        var neighbourChunkMesh = neighbourChunkMeshes[new Vector2Int(i, j)];
                        if (neighbourChunkMesh != null)
                            if (chunkMesh.Algorithm.Equals(neighbourChunkMesh.Algorithm))
                                continue;

                        smoothingSides |= direction;
                    }
                }
            }

            if (smoothingSides != SmoothDirection.None)
                Smooth(ref map, smoothingSides, xSeamVertsCount, ySeamVertsCount);
        }
        #endregion

        #region PRIVATE
        private void Awake()
        {
            data = FindObjectOfType<ChunksData>();
            if (chunkMesh == null)
                chunkMesh = GetComponent<ChunkMesh>();

            index = new Vector2Int(chunkMesh.Chunk.Index.x, chunkMesh.Chunk.Index.z);
            PopulateNeighbours();
        }

        private void PopulateNeighbours()
        {
            for (int j = -1; j <= 1; j++)
            {
                for (int i = -1; i <= 1; i++)
                {
                    if (i == 0 && j == 0)
                        continue;

                    Vector2Int neighbourRelativeIndex = new Vector2Int(i, j);

                    var neighbour = data.GetChunk(index.x + i, index.y + j);
                    neighbourChunks[neighbourRelativeIndex] = neighbour;
                    if (neighbour != null && neighbour.TryGetComponent<ChunkMesh>(out var neighbourChunkMesh))
                        neighbourChunkMeshes[neighbourRelativeIndex] = neighbourChunkMesh;
                    else
                        neighbourChunkMeshes[neighbourRelativeIndex] = null;
                }
            }
        }



        private void Smooth(ref float[,] map, SmoothDirection direction, int smoothingWidth, int smoothingHeight)
        {
            int mapWidth = map.GetLength(0);
            int mapHeight = map.GetLength(1);

            int rightSmoothEnd = mapWidth - smoothingWidth - 1;
            int bottomSmoothEnd = mapHeight - smoothingHeight - 1;

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    float value = map[x, y];

                    if ((direction & SmoothDirection.Top) > 0)
                        value = Mathf.Lerp(0.5f, value, Mathf.InverseLerp(mapHeight - 1, bottomSmoothEnd, y));
                    if ((direction & SmoothDirection.Left) > 0)
                        value = Mathf.Lerp(0.5f, value, Mathf.InverseLerp(0, smoothingWidth, x));
                    if ((direction & SmoothDirection.Right) > 0)
                        value = Mathf.Lerp(0.5f, value, Mathf.InverseLerp(mapWidth - 1, rightSmoothEnd, x));
                    if ((direction & SmoothDirection.Bottom) > 0)
                        value = Mathf.Lerp(0.5f, value, Mathf.InverseLerp(0, smoothingHeight, y));

                    if ((direction & SmoothDirection.TopLeft) > 0 && (direction & SmoothDirection.Top) == 0 && (direction & SmoothDirection.Left) == 0)
                    {
                        float lerp = Mathf.InverseLerp(mapHeight - 1, bottomSmoothEnd, y) + Mathf.InverseLerp(0, smoothingWidth, x);
                        value = Mathf.Lerp(0.5f, value, lerp);
                    }
                    if ((direction & SmoothDirection.TopRight) > 0 && (direction & SmoothDirection.Top) == 0 && (direction & SmoothDirection.Right) == 0)
                    {
                        float lerp = Mathf.InverseLerp(mapHeight - 1, bottomSmoothEnd, y) + Mathf.InverseLerp(mapWidth - 1, rightSmoothEnd, x);
                        value = Mathf.Lerp(0.5f, value, lerp);
                    }
                    if ((direction & SmoothDirection.BottomLeft) > 0 && (direction & SmoothDirection.Bottom) == 0 && (direction & SmoothDirection.Left) == 0)
                    {
                        float lerp = Mathf.InverseLerp(0, smoothingHeight, y) + Mathf.InverseLerp(0, smoothingWidth, x);
                        value = Mathf.Lerp(0.5f, value, lerp);
                    }
                    if ((direction & SmoothDirection.BottomRight) > 0 && (direction & SmoothDirection.Bottom) == 0 && (direction & SmoothDirection.Right) == 0)
                    {
                        float lerp = Mathf.InverseLerp(0, smoothingHeight, y) + Mathf.InverseLerp(mapWidth - 1, rightSmoothEnd, x);
                        value = Mathf.Lerp(0.5f, value, lerp);
                    }

                    map[x, y] = value;
                }
            }
        }
        #endregion
    }
}

namespace Bipolar.ChunkSystem.Generation
{

    [System.Flags]
    public enum SmoothDirection
    {
        None = 0,

        Top = 1 << 0,
        TopRight = 1 << 1,
        Right = 1 << 2,
        BottomRight = 1 << 3,
        Bottom = 1 << 4,
        BottomLeft = 1 << 5,
        Left = 1 << 6,
        TopLeft = 1 << 7,

        AnyTop = TopLeft | TopRight | Top,
        AnyBottom = BottomLeft | BottomRight | Bottom,
        AnyLeft = TopLeft | BottomLeft | Left,
        AnyRight = TopRight | BottomRight | Right,

        Vertical = Top | Bottom,
        Horizontal = Left | Right,
        AnyCenter = Vertical | Horizontal,

        AnyHorizontal = AnyLeft | AnyRight,
        AnyVertical = AnyTop | AnyBottom,

        All = AnyHorizontal | AnyVertical
    }
}
