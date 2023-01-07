using Bipolar.ChunkSystem.Generation;
using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.ChunkSystem.TerrainChunks
{
    public class ChunkTerrainSmoothing : Smoothing
    {
        [SerializeField]
        private ChunkTerrain chunkTerrain;

        private ChunksData data;
        private Vector2Int index;

        private readonly Dictionary<Vector2Int, Chunk> neighbourChunks = new Dictionary<Vector2Int, Chunk>(8);
        private readonly Dictionary<Vector2Int, ChunkTerrain> neighbourChunkTerrains = new Dictionary<Vector2Int, ChunkTerrain>(8);

        private readonly Dictionary<SmoothDirection, float[,]> heightmapsByDirection = new Dictionary<SmoothDirection, float[,]>();

        public override void ModifyMap(ref float[,] map)
        {
            if (isInitialized == false)
                Init();

            int mapWidth = map.GetLength(0);
            int mapHeight = map.GetLength(1);

            int xSeamVertsCount = Mathf.RoundToInt(mapWidth * settings.SeamLength);
            int ySeamVertsCount = Mathf.RoundToInt(mapHeight * settings.SeamLength);
            var smoothingSides = SmoothDirection.None;
            for (int j = -1; j <= 1; j++)
            {
                for (int i = -1; i <= 1; i++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    
                    var direction = i == -1 ? SmoothDirection.AnyLeft : i == 1 ? SmoothDirection.AnyRight : SmoothDirection.AnyCenter;
                    direction &= j == -1 ? SmoothDirection.AnyBottom : j == 1 ? SmoothDirection.AnyTop : SmoothDirection.AnyCenter;

                    if (neighbourChunks.TryGetValue(new Vector2Int(i, j), out var neighbour))
                    {
                        if (neighbour == null)
                            continue;

                        var neighbourChunkTerrain = neighbourChunkTerrains[new Vector2Int(i, j)];
                        if (neighbourChunkTerrain != null)
                            if (neighbourChunkTerrain.MapProvider != null && chunkTerrain.MapProvider.Equals(neighbourChunkTerrain.MapProvider))
                                continue;

                        smoothingSides |= direction;

                        int resolution = neighbourChunkTerrain.TerrainData.heightmapResolution;
                        int lastIndex = resolution - 1;
                        float[,] heights = neighbourChunkTerrain.TerrainData.GetHeights(
                            (direction & SmoothDirection.AnyLeft) > 0 ? lastIndex : 0,
                            (direction & SmoothDirection.AnyBottom) > 0 ? lastIndex : 0,
                            (direction & SmoothDirection.Vertical) > 0 ? resolution : 1,
                            (direction & SmoothDirection.Horizontal) > 0 ? resolution : 1);
                        heightmapsByDirection.Add(direction, heights);
                    }
                }
            }

            if (smoothingSides != SmoothDirection.None)
                Smooth(ref map, mapWidth, mapHeight, smoothingSides, xSeamVertsCount, ySeamVertsCount);
        }

        private void Smooth(ref float[,] map, int mapWidth, int mapHeight, SmoothDirection direction, int smoothingWidth, int smoothingHeight)
        {
            int rightSmoothEnd = mapWidth - smoothingWidth - 1;
            int bottomSmoothEnd = mapHeight - smoothingHeight - 1;

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    float value = map[y, x];

                    if ((direction & SmoothDirection.Top) > 0)
                    {
                        float targetValue = heightmapsByDirection[SmoothDirection.Top][0, x];
                        float lerp = Mathf.InverseLerp(mapHeight - 1, bottomSmoothEnd, y);
                        if (settings.UseCurve)
                            lerp = settings.Curve.Evaluate(lerp);
                        value = Mathf.Lerp(targetValue, value, lerp);
                    }
                    if ((direction & SmoothDirection.Left) > 0)
                    {
                        float targetValue = heightmapsByDirection[SmoothDirection.Left][y, 0];
                        float lerp = Mathf.InverseLerp(0, smoothingWidth, x);
                        if (settings.UseCurve)
                            lerp = settings.Curve.Evaluate(lerp);
                        value = Mathf.Lerp(targetValue, value, lerp);
                    }
                    if ((direction & SmoothDirection.Right) > 0)
                    {
                        float targetValue = heightmapsByDirection[SmoothDirection.Right][y, 0];
                        float lerp = Mathf.InverseLerp(mapWidth - 1, rightSmoothEnd, x);
                        if (settings.UseCurve)
                            lerp = settings.Curve.Evaluate(lerp);
                        value = Mathf.Lerp(targetValue, value, lerp);
                    }
                    if ((direction & SmoothDirection.Bottom) > 0)
                    {
                        float targetValue = heightmapsByDirection[SmoothDirection.Bottom][0, x];
                        float lerp = Mathf.InverseLerp(0, smoothingHeight, y);
                        if (settings.UseCurve)
                            lerp = settings.Curve.Evaluate(lerp);
                        value = Mathf.Lerp(targetValue, value, lerp);
                    }

                    if ((direction & SmoothDirection.TopLeft) > 0 && (direction & SmoothDirection.Top) == 0 && (direction & SmoothDirection.Left) == 0)
                    {
                        float targetValue = heightmapsByDirection[SmoothDirection.TopLeft][0, 0];
                        float lerp = Mathf.InverseLerp(mapHeight - 1, bottomSmoothEnd, y) + Mathf.InverseLerp(0, smoothingWidth, x);
                        if (settings.UseCurve)
                            lerp = settings.Curve.Evaluate(lerp);
                        value = Mathf.Lerp(targetValue, value, lerp);
                    }
                    if ((direction & SmoothDirection.TopRight) > 0 && (direction & SmoothDirection.Top) == 0 && (direction & SmoothDirection.Right) == 0)
                    {
                        float targetValue = heightmapsByDirection[SmoothDirection.TopRight][0, 0];
                        float lerp = Mathf.InverseLerp(mapHeight - 1, bottomSmoothEnd, y) + Mathf.InverseLerp(mapWidth - 1, rightSmoothEnd, x);
                        if (settings.UseCurve)
                            lerp = settings.Curve.Evaluate(lerp);
                        value = Mathf.Lerp(targetValue, value, lerp);
                    }
                    if ((direction & SmoothDirection.BottomLeft) > 0 && (direction & SmoothDirection.Bottom) == 0 && (direction & SmoothDirection.Left) == 0)
                    {
                        float targetValue = heightmapsByDirection[SmoothDirection.BottomLeft][0, 0];
                        float lerp = Mathf.InverseLerp(0, smoothingHeight, y) + Mathf.InverseLerp(0, smoothingWidth, x);
                        if (settings.UseCurve)
                            lerp = settings.Curve.Evaluate(lerp);
                        value = Mathf.Lerp(targetValue, value, lerp);
                    }
                    if ((direction & SmoothDirection.BottomRight) > 0 && (direction & SmoothDirection.Bottom) == 0 && (direction & SmoothDirection.Right) == 0)
                    {
                        float targetValue = heightmapsByDirection[SmoothDirection.BottomRight][0, 0];
                        float lerp = Mathf.InverseLerp(0, smoothingHeight, y) + Mathf.InverseLerp(mapWidth - 1, rightSmoothEnd, x);
                        if (settings.UseCurve)
                            lerp = settings.Curve.Evaluate(lerp);
                        value = Mathf.Lerp(targetValue, value, lerp);
                    }
                    map[y, x] = value;
                }
            }
        }

        private void Awake()
        {
            chunkTerrain.Chunk.OnChunkInitialized += Init;
        }

        private bool isInitialized = false;
        private void Init()
        {    
            data = FindObjectOfType<ChunksData>();
            if (chunkTerrain == null)
                chunkTerrain = GetComponent<ChunkTerrain>();

            index = new Vector2Int(chunkTerrain.Chunk.Index.x, chunkTerrain.Chunk.Index.z);
            PopulateNeighbours();
            isInitialized = true;
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
                    if (neighbour != null && neighbour.TryGetComponent<ChunkTerrain>(out var neighbourChunkTerrain))
                        neighbourChunkTerrains[neighbourRelativeIndex] = neighbourChunkTerrain;
                    else
                        neighbourChunkTerrains[neighbourRelativeIndex] = null;
                }
            }
        }
    }
}
