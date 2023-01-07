using System;
using UnityEngine;

namespace Bipolar.ChunkSystem.TerrainChunks
{
    [RequireComponent(typeof(ChunksData))]
    public class ChunkTerrainsNeighbourhoodManager : MonoBehaviour
    {
        private ChunksData data;

        private void Awake()
        {
            data = GetComponent<ChunksData>();
        }

        private void OnEnable()
        {
            ChunkTerrain.OnTerrainCreated += ConnectToNeighbours;
        }

        private void ConnectToNeighbours(ChunkTerrain chunkTerrain)
        {
            var index = chunkTerrain.Chunk.Index;
            var terrain = chunkTerrain.Terrain;
            Terrain topNeighbour = terrain.topNeighbor, 
                bottomNeighbour = terrain.bottomNeighbor, 
                leftNeighbour = terrain.leftNeighbor, 
                rightNeighbour = terrain.rightNeighbor;
            var topChunk = data.GetChunk(index.x, index.z + 1);
            TryAssignNeighbourFromChunk(topChunk, ref topNeighbour);

            var bottomChunk = data.GetChunk(index.x, index.z - 1);
            TryAssignNeighbourFromChunk(bottomChunk, ref bottomNeighbour);

            var leftChunk = data.GetChunk(index.x - 1, index.z);
            TryAssignNeighbourFromChunk(leftChunk, ref leftNeighbour);

            var rightChunk = data.GetChunk(index.x + 1, index.z);
            TryAssignNeighbourFromChunk(rightChunk, ref rightNeighbour);

            terrain.SetNeighbors(leftNeighbour, topNeighbour, rightNeighbour, bottomNeighbour);

            SetNeighboursToTerrain(leftNeighbour, right: terrain);
            SetNeighboursToTerrain(topNeighbour, bottom: terrain);
            SetNeighboursToTerrain(rightNeighbour, left: terrain);
            SetNeighboursToTerrain(bottomNeighbour, top: terrain);
        }

        private bool TryAssignNeighbourFromChunk(Chunk sourceChunk, ref Terrain targetTerrain)
        {
            if (sourceChunk == null)
                return false;

            if (!sourceChunk.TryGetComponent<ChunkTerrain>(out var neighbourChunkTerrain))
                return false;
            
            targetTerrain = neighbourChunkTerrain.Terrain;
            return true;
        }

        public void SetNeighboursToTerrain(Terrain terrain, Terrain left = null, Terrain top = null, Terrain right = null, Terrain bottom = null)
        {
            if (terrain == null)
                return;

            if (left == null)
                left = terrain.leftNeighbor;
            if (top == null)
                top = terrain.topNeighbor;
            if (right == null)
                right = terrain.rightNeighbor;
            if (bottom == null)
                bottom = terrain.bottomNeighbor;
            
            terrain.SetNeighbors(left, top, right, bottom);
        }

        private void OnDisable()
        {
            ChunkTerrain.OnTerrainCreated -= ConnectToNeighbours;
        }
    }
}
