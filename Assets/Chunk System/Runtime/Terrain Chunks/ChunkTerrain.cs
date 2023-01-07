using Bipolar.ChunkSystem.Generation;
using System.Threading.Tasks;
using UnityEngine;

namespace Bipolar.ChunkSystem.TerrainChunks
{
    public class ChunkTerrain : MonoBehaviour
    {
        public static event System.Action<ChunkTerrain> OnTerrainCreated;

        public event System.Action OnTerrainHeightsCreated;

        [Header("To Link")]
        [SerializeField]
        private Chunk chunk;
        public Chunk Chunk => chunk;
        [SerializeField]
        private Terrain terrain;
        public Terrain Terrain => terrain;
        [SerializeField]
        private new TerrainCollider collider;
        public TerrainCollider Collider => collider;
        [SerializeField]
        private ChunkTerrainSettings settings;
        public ChunkTerrainSettings Settings => settings;

        private float[,] heightMap;
        private float[,,] alphaMap;

        [Header("Map")]
        [SerializeField]
        private Object mapProvider;
        public IMapProvider MapProvider
        {
            get => mapProvider as IMapProvider;
            set
            {
                mapProvider = value as Object;
            }
        }

        [SerializeField]
        private AnimationCurve heightCurve;

        [SerializeField]
        private Object smoothing;
        public IMapModifier Smoothing
        {
            get => smoothing as IMapModifier;
            set
            {
                smoothing = value as Object;
            }
        }

        [Header("Biomes")]
        [SerializeField]
        private Object biomeMapProvider;
        public IMapProvider<FastNoiseLite.CellularData> BiomeMapProvider
        {
            get => biomeMapProvider as IMapProvider<FastNoiseLite.CellularData>;
            set
            {
                biomeMapProvider = value as Object;
            }
        }

        [Header("Properties")]
        [SerializeField]
        private TerrainData terrainData;
        public TerrainData TerrainData => terrainData;

        [Header("States")]
        [SerializeField]
        private bool wasGeneratedProcedurally = false;
        public bool WasGeneratedProcedurally => wasGeneratedProcedurally;

        private Task<float[,]> heightmapTask;
        private Task<float[,,]> alphamapsTask;

        private void Reset()
        {
            if (Settings != null)
                settings = Settings;
            if (chunk == null)
                chunk = GetComponent<Chunk>();
            if (terrain == null)
                terrain = GetComponent<Terrain>();
            if (collider == null)
                collider = GetComponent<TerrainCollider>();
            if (terrainData == null && terrain != null)
                terrainData = terrain.terrainData;
            if (collider != null && collider.terrainData == null)
                collider.terrainData = terrainData;
        }

        private void Awake()
        {
            Reset();
            chunk.OnChunkInitialized += GenerateTerrain;
        }

        private void Start()
        {
            terrain.transform.localPosition = new Vector3(
                -0.5f * chunk.Settings.ChunkSize.x,
                0,
                -0.5f * chunk.Settings.ChunkSize.z);
        }

        private void GenerateTerrain()
        {
            chunk.OnChunkInitialized -= GenerateTerrain;
            if (terrain == null || terrainData != null)
                return;
            wasGeneratedProcedurally = true;

            // Request these things
            // mainly TerrainData with all of them contained

            CreateTerrainData();

            CreateTerrainShape();
            //CreateTerrainColors();


            collider.terrainData = terrain.terrainData = terrainData;
            gameObject.name = $"Terrain Chunk ({chunk.Index.x}, {chunk.Index.z})";
            OnTerrainCreated?.Invoke(this);
        }

        private void CreateTerrainData()
        {
            terrainData = settings.BaseTerrainData == null ? new TerrainData() : Instantiate(settings.BaseTerrainData);
            terrainData.name = $"TerrainData {System.Guid.NewGuid()}";
            // terrain size must be set after hightmap resolution that's why it's not here
            terrain.materialTemplate = settings.Material;
        }

        private void CreateBiomesData()
        {
        }

        private void CreateTerrainShape()
        {
            float xSize = chunk.Settings.ChunkSize.x;
            float zSize = chunk.Settings.ChunkSize.z;
            int resolution = settings.HeightmapResolution;             
            terrainData.heightmapResolution = resolution;
            terrainData.size = new Vector3(xSize, Settings.MaxHeight, zSize);
            heightMap = MapProvider.GetMap(resolution, resolution,
                zSize, xSize, chunk.Index.z * zSize, chunk.Index.x * xSize);

            for (int j = 0; j < resolution; j++)
            {
                for (int i = 0; i < resolution; i++)
                {
                    heightMap[j, i] = heightCurve.Evaluate(heightMap[j, i]);
                }
            }
            if(Smoothing != null)
                Smoothing.ModifyMap(ref heightMap);
            terrainData.SetHeights(0, 0, heightMap);
            OnTerrainHeightsCreated?.Invoke();
        }

        public void SetLayers(TerrainLayer[] layers, float[,,] alphaMap)
        {
            terrainData.alphamapResolution = alphaMap.GetLength(0);
            terrainData.terrainLayers = layers;
            terrainData.SetAlphamaps(0, 0, alphaMap);
        }

        public void CreateTerrainColors()
        {
            int layersCount = terrainData.alphamapLayers;
            int CellValueToLayer(float biome)
            {
                float layerAsFloat = Mathf.InverseLerp(1, -1, biome) * layersCount;
                return (int)layerAsFloat;
            }

            float xSize = chunk.Settings.ChunkSize.x;
            float zSize = chunk.Settings.ChunkSize.z;
            int resolution = settings.AlphamapResolution;
            terrainData.alphamapResolution = resolution;
            var biomeMap = BiomeMapProvider.GetMap(resolution, resolution,
                zSize * (1 + 1f / resolution), xSize * (1 + 1f / resolution), 
                chunk.Index.z * zSize, chunk.Index.x * xSize);
            //float oneOverResolution = 1f / (resolution - 1);
            alphaMap = new float[resolution, resolution, layersCount];
            float limitDifference = settings.BiomesTransitionLength * 1f;
            for (int j = 0; j < resolution; j++)
            {
                for (int i = 0; i < resolution; i++)
                {
                    var cellData = biomeMap[i, j];
                    int mainBiomeIndex = CellValueToLayer(cellData.cellValue0);
                    int secondBiomeIndex = CellValueToLayer(cellData.cellValue1);
                    int thirdBiomeIndex = CellValueToLayer(cellData.cellValue2);

                    float mainBiomeDistance = cellData.distance0;
                    float secondBiomeDistance = cellData.distance1;
                    float thirdBiomeDistance = cellData.distance2;

                    float difference21 = secondBiomeDistance - mainBiomeDistance;
                    float difference31 = thirdBiomeDistance - mainBiomeDistance;
                    float difference32 = thirdBiomeDistance - secondBiomeDistance;

                    float good = Mathf.Lerp(0.5f, 1, Mathf.InverseLerp(0, limitDifference, difference21));
                    float mainBiomeStrength, secondBiomeStrength, thirdBiomeStrength;
                    if (difference21 > limitDifference)
                    {
                        mainBiomeStrength = 1;
                        secondBiomeStrength = 0;
                        thirdBiomeStrength = 0;
                    }
                    else if (difference31 > limitDifference)
                    {
                        mainBiomeStrength = good;
                        secondBiomeStrength = 1 - mainBiomeStrength;
                        thirdBiomeStrength = 0;
                    }
                    else
                    {
                        mainBiomeStrength = Mathf.Lerp(0.333f, good, Mathf.InverseLerp(0, limitDifference, difference31));
                        float rest = 1 - mainBiomeStrength;
                        secondBiomeStrength = rest * Mathf.Lerp(0.5f, 1, Mathf.InverseLerp(0, limitDifference, difference32));
                        thirdBiomeStrength = rest - secondBiomeStrength;
                    }

                    //if (secondBiomeIndex == thirdBiomeIndex)
                    //    secondBiomeStrength += thirdBiomeStrength;
                    //if (mainBiomeIndex == thirdBiomeIndex)
                    //    mainBiomeStrength += thirdBiomeStrength;
                    //if (mainBiomeIndex == secondBiomeIndex)
                    //    mainBiomeStrength += secondBiomeStrength;

                    alphaMap[i, j, thirdBiomeIndex] += thirdBiomeStrength;
                    alphaMap[i, j, secondBiomeIndex] += secondBiomeStrength;
                    alphaMap[i, j, mainBiomeIndex] += mainBiomeStrength;
                }
            }
            terrainData.SetAlphamaps(0, 0, alphaMap);
        }

        private float GetHeight(float x, float y, int maxHeightmapIndex)
        {
            int xFloor = Mathf.FloorToInt(x * maxHeightmapIndex);
            int xCeil = Mathf.CeilToInt(x * maxHeightmapIndex);
            int yFloor = Mathf.FloorToInt(y * maxHeightmapIndex);
            int yCeil = Mathf.CeilToInt(y * maxHeightmapIndex);

            float xMantissa = x - xFloor;
            float yMantissa = y - yFloor;

            float bottomLeft = heightMap[yFloor, xFloor];
            float bottomRight = heightMap[yFloor, xCeil];
            float topLeft = heightMap[yCeil, xFloor];
            float topRight = heightMap[yCeil, xCeil];

            float leftHeight = Mathf.Lerp(bottomLeft, topLeft, yMantissa);
            float rightHeight = Mathf.Lerp(bottomRight, topRight, yMantissa);
            return Mathf.Lerp(leftHeight, rightHeight, xMantissa);
        }
    }
}


public static class ExtensionFloat
{
    public static float Squared(this float f)
    {
        return  f;
    }
}