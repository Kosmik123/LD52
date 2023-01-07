using UnityEngine;

namespace Bipolar.ChunkSystem.Biomes
{
    [System.Serializable]
    public class SerializedTerrainLayer 
    {
        [SerializeField]
        private Texture2D diffuseTexture;
        [SerializeField]
        private Texture2D normalMapTexture;
        [SerializeField]
        private Texture2D maskMapTexture;

        [SerializeField]
        private Color specular = Color.white;

        [Header("Tiling Settings")]
        [SerializeField]
        private Vector2 tileSize;
        [SerializeField]
        private Vector2 tileOffset;

        [SerializeField]
        private float metallic;
        [SerializeField]
        private float smoothness;
        [SerializeField]
        private float normalScale;
        [SerializeField]
        private Vector4 diffuseRemapMin;
        [SerializeField]
        private Vector4 diffuseRemapMax;
        [SerializeField]
        private Vector4 maskMapRemapMin;
        [SerializeField]
        private Vector4 maskMapRemapMax;

        public TerrainLayer ToTerrainLayer() => new TerrainLayer()
        {
            diffuseTexture = diffuseTexture,
            normalMapTexture = normalMapTexture,
            maskMapTexture = maskMapTexture,
            tileSize = tileSize,
            tileOffset = tileOffset,
            specular = specular,
            metallic = metallic,
            smoothness = smoothness,
            normalScale = normalScale,
            diffuseRemapMin = diffuseRemapMin,
            diffuseRemapMax = diffuseRemapMax,
            maskMapRemapMin = maskMapRemapMin,
            maskMapRemapMax = maskMapRemapMax
        };
        
    }

    

}
