using UnityEngine;

namespace Bipolar.ChunkSystem.Biomes
{
    [CreateAssetMenu(menuName = "Chunk System/Biomes/Terrain Biome")]
    public class TerrainBiome : Biome
    {
        [System.Serializable]
        private struct LayerPropertiesMapping
        {
            [System.Serializable]
            public struct RangeLimitations
            {
                public bool enabled;
                [Range(0, 1)]
                public float min;
                [Range(0, 1)]
                public float max;

                public bool IsFullfilled(float value) => value < min && value > max;
            }

            public RangeLimitations heightRange;
            [Tooltip("0 is flat, 1 is vertical slope")]
            public RangeLimitations slopeRange;

            public TerrainLayer terrainLayer;

            public bool IsFullfilled(float height, float slope)
            {
                return (heightRange.enabled == false || heightRange.IsFullfilled(height))
                    && (slopeRange.enabled == false || slopeRange.IsFullfilled(slope));
            }
        }

        [SerializeField]
        private LayerPropertiesMapping[] layers;

        [SerializeField]
        private TerrainLayer defaultLayer;
        public TerrainLayer DefaultLayer => defaultLayer;

    }
}







