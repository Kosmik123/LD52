using UnityEngine;

namespace Bipolar.ChunkSystem.TerrainChunks
{
    [CreateAssetMenu(menuName = "Chunk System/Terrain/Chunk Terrain Settings")]
    public class ChunkTerrainSettings : ScriptableObject
    {
        [System.Serializable]
        public struct SerializedTreeInstance
        {
            public Vector3 position;
            public float widthScale;
            public float heightScale;
            public float rotation;
            public Color32 color;
            public Color32 lightmapColor;
            public int prototypeIndex;        
        }

        [System.Serializable]
        public class SerializedTreePrototype
        { 
            public GameObject prefab { get; set; }
            public float bendFactor { get; set; }
            public int navMeshLod { get; set; }
        }

        [Header("General")]
        [SerializeField]
        private TerrainData baseTerrainData;
        public TerrainData BaseTerrainData => baseTerrainData;
        private TerrainData previousTerrainData;
        [SerializeField]
        private int groupingID;
        [SerializeField]
        private float maxHeight;
        public float MaxHeight => maxHeight;
        [SerializeField]
        private float defaultLocalHeight;
        public float DefaultLocalHeight => defaultLocalHeight;
        [SerializeField]
        private Material material;
        public Material Material => material;

        [Header("Resolutions")]
        [SerializeField]
        private HeightmapResolutionEnum heightmapResolution = HeightmapResolutionEnum._257;
        public int HeightmapResolution => (int)heightmapResolution;

        [SerializeField]
        private TextureResolutionEnum alphamapResolution = TextureResolutionEnum._256;
        public int AlphamapResolution => (int)alphamapResolution;

        [Header("Biomes")]
        [SerializeField, Range(0, 1f)]
        private float biomesTransitionLength;
        public float BiomesTransitionLength
        {
            get => biomesTransitionLength;
            set => biomesTransitionLength = value;
        }

        private void PopulateFields()
        {   
        }


        public event System.Action OnChanged;
        private float previousBiomesTransitionLength;
        private void OnValidate()
        {
            if (previousBiomesTransitionLength != biomesTransitionLength)
            {
                previousBiomesTransitionLength = biomesTransitionLength;
                OnChanged?.Invoke();
            }

            if (baseTerrainData != previousTerrainData)
            {
                PopulateFields();
                previousTerrainData = baseTerrainData;
            }
            defaultLocalHeight = Mathf.Clamp(defaultLocalHeight, 0, maxHeight);

        }
    }
}