using Bipolar.ChunkSystem.Generation;
using Bipolar.ChunkSystem.TerrainChunks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.ChunkSystem.Biomes
{
    public class ChunkBiome : MonoBehaviour
    {
        private const int MaxBiomesCountInOnePoint = 3;

        [SerializeField]
        private Chunk chunk;

        [SerializeField]
        private ChunkTerrain chunkTerrain;

        [SerializeField]
        private BiomeSettings settings;

        [SerializeField]
        private Object cellularMapProvider;
        public IMapProvider<FastNoiseLite.CellularData> CellularMapProvider
        {
            get => cellularMapProvider as IMapProvider<FastNoiseLite.CellularData>;
            set
            {
                cellularMapProvider = value as Object;
            }
        }

        [SerializeField]
        private TextureResolutionEnum resolution;
        public int Resolution => (int)resolution;

        public struct BiomeValue
        {
            public int biomeIndex;
            public float value;

            public BiomeValue(int index, float value)
            {
                biomeIndex = index;
                this.value = value;
            }
        }

        private BiomeValue[,,] biomeStrengths;
        private TerrainLayer[] layers;
        private BitArray biomesBitArray;

        private void Awake()
        {
            chunkTerrain.OnTerrainHeightsCreated += PopulateBiomeValues;
        }

        public void PopulateBiomeValues()
        {
            biomeStrengths = new BiomeValue[Resolution, Resolution, MaxBiomesCountInOnePoint];
            var allPossibleBiomes = settings.Biomes;
            int biomesCount = allPossibleBiomes.Length;
            int CellValueToLayer(float cellValue)
            {
                float biomeIndex = Mathf.InverseLerp(1, -1, cellValue) * biomesCount;
                return (int)biomeIndex;
            }

            float xSize = chunk.Settings.ChunkSize.x;
            float zSize = chunk.Settings.ChunkSize.z;
            var cellularMap = CellularMapProvider.GetMap(Resolution, Resolution,
                zSize * (1 + 1f / Resolution), xSize * (1 + 1f / Resolution),
                chunk.Index.z * zSize, chunk.Index.x * xSize);

            biomesBitArray = new BitArray(biomesCount);
            float limitDifference = settings.TransitionLength * 1f;
            for (int j = 0; j < Resolution; j++)
            {
                for (int i = 0; i < Resolution; i++)
                {
                    var cellData = cellularMap[i, j];
                    int mainBiomeIndex = CellValueToLayer(cellData.cellValue0);
                    int secondBiomeIndex = CellValueToLayer(cellData.cellValue1);
                    int thirdBiomeIndex = CellValueToLayer(cellData.cellValue2);

                    float mainBiomeDistance = cellData.distance0;
                    float secondBiomeDistance = cellData.distance1;
                    float thirdBiomeDistance = cellData.distance2;

                    float difference21 = secondBiomeDistance - mainBiomeDistance;
                    float difference31 = thirdBiomeDistance - mainBiomeDistance;
                    float difference32 = thirdBiomeDistance - secondBiomeDistance;

                    float mainBiomeStrength, secondBiomeStrength, thirdBiomeStrength;
                    if (difference21 > limitDifference)
                    {
                        mainBiomeStrength = 1;
                        secondBiomeStrength = 0;
                        thirdBiomeStrength = 0;
                        biomesBitArray.Set(mainBiomeIndex, true);
                    }
                    else
                    {
                        float good = Mathf.Lerp(0.5f, 1, Mathf.InverseLerp(0, limitDifference, difference21));
                        if (difference31 > limitDifference)
                        {
                            mainBiomeStrength = good;
                            secondBiomeStrength = 1 - mainBiomeStrength;
                            thirdBiomeStrength = 0;
                            biomesBitArray.Set(mainBiomeIndex, true);
                            biomesBitArray.Set(secondBiomeIndex, true);
                        }
                        else
                        {
                            mainBiomeStrength = Mathf.Lerp(0.333f, good, Mathf.InverseLerp(0, limitDifference, difference31));
                            float rest = 1 - mainBiomeStrength;
                            secondBiomeStrength = rest * Mathf.Lerp(0.5f, 1, Mathf.InverseLerp(0, limitDifference, difference32));
                            thirdBiomeStrength = rest - secondBiomeStrength;
                            biomesBitArray.Set(mainBiomeIndex, true);
                            biomesBitArray.Set(secondBiomeIndex, true);
                            biomesBitArray.Set(thirdBiomeIndex, true);
                        }
                    }
                    biomeStrengths[i, j, 0] = new BiomeValue(mainBiomeIndex, mainBiomeStrength);
                    biomeStrengths[i, j, 1] = new BiomeValue(secondBiomeIndex, secondBiomeStrength);
                    biomeStrengths[i, j, 2] = new BiomeValue(thirdBiomeIndex, thirdBiomeStrength);
                }
            }
            SetLayersOnTerrain();
        }

        public static int CountTrue(BitArray array)
        {
            int count = 0;
            for (int i = 0; i < array.Count; i++)
                if (array.Get(i))
                    count++;
            return count;
        }

        private void SetLayersOnTerrain()
        {
            int biomesInChunkCount = CountTrue(biomesBitArray);
            layers = new TerrainLayer[biomesInChunkCount];
            var globalToLocalBiomeIndicesDictionary = new Dictionary<int, int>(biomesInChunkCount);
            int layerIndex = 0;
            for (int i = 0; i < settings.Biomes.Length; i++)
            {
                if (biomesBitArray.Get(i))
                {
                    globalToLocalBiomeIndicesDictionary.Add(i, layerIndex);
                    layers[layerIndex] = ((TerrainBiome)settings.Biomes[i]).DefaultLayer;
                    layerIndex++;
                }

            }
            int resolution = Resolution;
            float[,,] alphamaps = new float[resolution, resolution, biomesInChunkCount];
            for (int i = 0; i < resolution; i++)
            {
                for (int j = 0; j < resolution; j++)
                {
                    for (int k = 0; k < MaxBiomesCountInOnePoint; k++)
                    {
                        var biomeStrength = biomeStrengths[i, j, k];
                        if (globalToLocalBiomeIndicesDictionary.TryGetValue(biomeStrength.biomeIndex, out int localBiomeIndex))
                            alphamaps[i, j, localBiomeIndex] += biomeStrength.value;
                    }
                }
            }
            chunkTerrain.SetLayers(layers, alphamaps);
        }

        private void OnDisable()
        {
            // if (chunkTerrain != null)
            //      chunkTerrain.OnTerrainHeightsCreated -= PopulateBiomeValues;
        }

        private void OnValidate()
        {
            CellularMapProvider = CellularMapProvider;
        }
    }
}
