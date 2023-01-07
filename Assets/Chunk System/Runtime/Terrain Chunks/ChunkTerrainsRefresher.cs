using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.ChunkSystem.TerrainChunks
{
    // do testowania biomów
    public class ChunkTerrainsRefresher : MonoBehaviour
    {
        private ChunksData data;

        public ChunkTerrainSettings chunkTerrainSettings;

        private readonly Dictionary<Vector3Int, ChunkTerrain> chunkTerrainsDictionary = new Dictionary<Vector3Int, ChunkTerrain>();

        private void Awake()
        {
            data = GetComponent<ChunksData>();
        }

        private void OnEnable()
        {
            chunkTerrainSettings.OnChanged += Refresh;
            data.OnChunkAdded += AddTerrain;
        }

        private void AddTerrain(Chunk chunk)
        {
            if (chunk.TryGetComponent<ChunkTerrain>(out var chunkTerrain))
            {
                chunkTerrainsDictionary.Add(chunk.Index, chunkTerrain);
            }
        }

        [ContextMenu("Refresh Colors")]
        public void Refresh()
        {
            foreach(var chunkTerrain in chunkTerrainsDictionary)
            {
                chunkTerrain.Value.CreateTerrainColors();
            }
        }


        private void OnDisable()
        {
            chunkTerrainSettings.OnChanged -= Refresh;
            data.OnChunkAdded -= AddTerrain;
        }
    }
}
