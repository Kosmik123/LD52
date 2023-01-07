using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.ChunkSystem.TerrainChunks
{
    public class ChunkTerrainManager : MonoBehaviour
    {
        private readonly Dictionary<Vector3Int, ChunkTerrain> chunkTerrainsDictionary = new Dictionary<Vector3Int, ChunkTerrain>();

        private void OnEnable()
        {
            ChunkTerrain.OnTerrainCreated += AddChunkTerrainToList;
        }

        private void AddChunkTerrainToList(ChunkTerrain chunkTerrain)
        {
            var index = chunkTerrain.Chunk.Index;
            chunkTerrainsDictionary.Add(index, chunkTerrain);
        }

        private void OnDisable()
        {
            ChunkTerrain.OnTerrainCreated -= AddChunkTerrainToList;
        }
    }
}
