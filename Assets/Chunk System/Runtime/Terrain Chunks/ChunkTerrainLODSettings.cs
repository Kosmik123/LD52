using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.ChunkSystem.TerrainChunks
{
    [CreateAssetMenu(menuName = "Chunk System/Terrain Chunks/Chunk Terrain LOD Settings")]
    public class ChunkTerrainLODSettings : ScriptableObject
    {
        [System.Serializable]
        private struct TerrainLOD
        {
            public float maxDistance;
            public float pixelError;
            [Tooltip("Not implemented yet")]
            public bool treeColliders;
        }

        [SerializeField]
        private List<TerrainLOD> TerrainLODs = new List<TerrainLOD>();
        [SerializeField]
        private float defaultPixelError = 10;

        private void OnEnable()
        {
            TerrainLODs.Sort((lhs, rhs) => lhs.maxDistance.CompareTo(rhs.maxDistance));
        }

        public float GetPixelError(float distance)
        {
            for (int i = 0; i < TerrainLODs.Count; i++)
            {
                var mapping = TerrainLODs[i];
                if (distance <= mapping.maxDistance)
                    return mapping.pixelError;
            }
            return defaultPixelError;
        }
    }
}
