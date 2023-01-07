using UnityEngine;
using Bipolar.ChunkSystem.Generation;
using static Bipolar.ChunkSystem.Generation.FastNoiseLite;

namespace Bipolar.ChunkSystem.Generation
{
    [CreateAssetMenu(menuName = "Chunk System/Generation/Cellular Noise Generator")]
    public class CellularNoiseGenerator : ScriptableMapProvider<CellularData>
    {
        [SerializeField]
        private int seed;
        public int Seed
        {
            get => seed;
            set
            {
                seed = value;
                InitFastNoise();
            }
        }

        [SerializeField]
        private float scale;
        public float Scale
        { 
            get => scale;
            set 
            {
                scale = value; 
                InitFastNoise();
                fastNoise.SetFrequency(1 / scale);
            }
        }

        [SerializeField]
        private CellularDistanceFunction distanceType;
        public CellularDistanceFunction DistanceType
        {
            get => distanceType;
            set
            {
                distanceType = value;
                InitFastNoise();
                fastNoise.SetCellularDistanceFunction(distanceType);
            }
        }

        [SerializeField, Range(0, 1)]
        private float jitter;
        public float Jitter
        {
            get => jitter;
            set
            {
                jitter = value;
                InitFastNoise();
                fastNoise.SetCellularJitter(jitter);
            }
        }

        private FastNoiseLite fastNoise;


        private void OnEnable()
        {
            Validate();
        }

        private void Validate()
        {
            InitFastNoise();
            fastNoise.SetNoiseType(NoiseType.Cellular);
            fastNoise.SetCellularDistanceFunction(distanceType);
            fastNoise.SetFrequency(1 / scale);
            fastNoise.SetCellularJitter(jitter);
        }

        private void InitFastNoise()
        {
            if (fastNoise == null)
                fastNoise = new FastNoiseLite(seed);
            else
                fastNoise.SetSeed(seed);
        }

        public override bool Equals(IMapProvider<CellularData> other)
        {
            return false;
        }

        public override CellularData[,] GetMap(int width, int height, float xScale = 1, float yScale = 1, float xOffset = 0, float yOffset = 0)
        {
            var map = new CellularData[width, height];
            GetMapNonAlloc(ref map, width, height, xScale, yScale, xOffset, yOffset);
            return map;
        }

        public override void GetMapNonAlloc(ref CellularData[,] map, int width, int height, float xScale = 1, float yScale = 1, float xOffset = 0, float yOffset = 0)
        {
            float oneOverWidth = 1f / width;
            float oneOverHeight = 1f / height;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    float x = (i * oneOverWidth - 0.5f) * xScale + xOffset;
                    float y = (j * oneOverHeight - 0.5f) * yScale + yOffset;
                    var value = fastNoise.GetCellularNoise(x, y);
                    map[i, j] = value;
                }
            }
        }

        private void OnValidate()
        {
            Validate();
        }

    }
}
