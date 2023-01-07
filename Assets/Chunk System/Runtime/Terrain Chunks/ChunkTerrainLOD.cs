using UnityEngine;

namespace Bipolar.ChunkSystem.TerrainChunks
{
    public class ChunkTerrainLOD : MonoBehaviour
    {
        [Header("To Link")]
        [SerializeField]
        private ChunkTerrain chunkTerrain;
        [SerializeField]
        private ChunkLOD chunkLOD;

        [Header("Settings")]
        [SerializeField]
        private ChunkTerrainLODSettings settings;

        private void OnEnable()
        {
            chunkLOD.OnChunkLODRefreshed += Refresh;
        }

        private void Refresh(float distance)
        {
            chunkTerrain.Terrain.heightmapPixelError = settings.GetPixelError(distance);
        }

        private void OnDisable()
        {
            chunkLOD.OnChunkLODRefreshed -= Refresh;
        }

#if UNITY_EDITOR
        private void Reset()
        {
            chunkTerrain = GetComponent<ChunkTerrain>();
            chunkLOD = GetComponent<ChunkLOD>();
        }
#endif
    }
}
