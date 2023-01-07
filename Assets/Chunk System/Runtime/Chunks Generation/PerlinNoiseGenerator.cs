using Bipolar.ChunkSystem.Generation;
using System;
using UnityEditor;
using UnityEngine;

namespace Bipolar.ChunkSystem.Generation
{
    [CreateAssetMenu(menuName = "Chunk System/Generation/Perlin Noise Generator")]
    public class PerlinNoiseGenerator : ScriptableObject, IMapProvider
    {
        public event Action OnChanged;

        [SerializeField]
        private int seed;
        public int Seed
        {
            get => seed;
            set
            {
                seed = value;
                random = new System.Random(seed);
                octaveOffsets = new Vector2[octavesCount];
                for (int i = 0; i < octavesCount; i++)
                {
                    float offsetX = random.Next(-10000, 10000);
                    float offsetY = random.Next(-10000, 10000);
                    octaveOffsets[i] = new Vector2(offsetX, offsetY);
                    OnChanged?.Invoke();
                }
            }
        }
        private System.Random random = new System.Random();

        private Vector2[] octaveOffsets;

        [SerializeField, Range(1, 20)]
        private int octavesCount = 1;
        public int OctavesCount => octavesCount;

        [SerializeField]
        private float scale = 1;
        public float Scale
        {
            get => scale;
            set
            {
                scale = Mathf.Max(0.0001f, value);
                oneOverScale = 1f / scale;
                OnChanged?.Invoke();
            }
        }
        private float oneOverScale;

        [SerializeField, Range(0, 1)]
        private float persistance;
        public float Persistance
        {
            get => persistance;
            set 
            {
                if (persistance == value)
                    return;
                persistance = Mathf.Clamp01(value);
                OnChanged?.Invoke();
            }
        }

        [SerializeField]
        private float lacunarity;

        [SerializeField]
        private float minNoiseValue = -2;
        [SerializeField]
        private float maxNoiseValue = 2;

        private void Awake()
        {
            Seed = seed;
            Scale = scale;
        }

        public float GetValue(float x, float y, float xScale = 1, float yScale = 1)
        {
            float amplitude = 1;
            float frequency = 1;

            float oneOverScaleX = oneOverScale / xScale;
            float oneOverScaleY = oneOverScale / yScale;

            float noiseHeight = 0;
            for (int i = 0; i < octavesCount; i++)
            {
                float sampleX = x * oneOverScaleX * frequency + octaveOffsets[i].x;
                float sampleY = y * oneOverScaleY * frequency + octaveOffsets[i].y;

                float perlinValue = 2 * Mathf.PerlinNoise(sampleX, sampleY) - 1;
                noiseHeight += perlinValue * amplitude;

                amplitude *= persistance;
                frequency *= lacunarity;
            }
            return noiseHeight;
        }

        public float[,] GetMap(int xResolution, int yResolution, float xScale = 1, float yScale = 1, float xOffset = 0, float yOffset = 0)
        {
            float[,] noiseMap = new float[xResolution, yResolution];
            GetMapNonAlloc(ref noiseMap, xResolution, yResolution, xScale, yScale, xOffset, yOffset);
            return noiseMap;
        }

        public void GetMapNonAlloc(ref float[,] map, int xResolution, int yResolution, float xScale = 1, float yScale = 1, float xOffset = 0, float yOffset = 0)
        {
            float maxNoiseValue = float.MinValue;
            float minNoiseValue = float.MaxValue;

            float deltaX = xScale / (xResolution - 1);
            float deltaY = yScale / (yResolution - 1);

            float halfXScale = 0.5f * xScale;
            float halfYScale = 0.5f * yScale;

            for (int j = 0; j < yResolution; j++)
            {
                for (int i = 0; i < xResolution; i++)
                {
                    float noiseValue = GetValue(
                        i * deltaX + xOffset - halfXScale,
                        j * deltaY + yOffset - halfYScale,
                        1, 1);
                    map[i, j] = noiseValue;
                    if (noiseValue > maxNoiseValue)
                        maxNoiseValue = noiseValue;
                    else if (noiseValue < minNoiseValue)
                        minNoiseValue = noiseValue;
                }
            }

            for (int j = 0; j < yResolution; j++)
            {
                for (int i = 0; i < xResolution; i++)
                {
                    map[i, j] = Mathf.InverseLerp(this.minNoiseValue, this.maxNoiseValue, map[i, j]);
                }
            }
        }

        private void OnValidate()
        {
            Seed = seed;
            Scale = Scale;
        }

        public bool Equals(IMapProvider other)
        { 
            if (other.GetType() != GetType())
                return false;

            var otherPerlin = other as PerlinNoiseGenerator;
            if (otherPerlin.Seed != seed)
                return false;
            if (otherPerlin.OctavesCount != octavesCount)
                return false;
            if (otherPerlin.lacunarity != lacunarity)
                return false; 
            if (otherPerlin.Persistance != persistance)
                return false; 
            if (otherPerlin.Scale != scale)
                return false;

            return true;
        }
    }
}

namespace ChunkSystem
{
#if UNITY_EDITOR
    //[CustomEditor(typeof(NoiseGenerator))]
    public class NoiseGeneratorEditor : UnityEditor.Editor
    {
        private const int previewResolution = 64;
        private Texture texture = new Texture2D(previewResolution, previewResolution);

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var noiseGenerator = target as PerlinNoiseGenerator;
            if (texture == null)
                texture = new Texture2D(previewResolution, previewResolution);

            EditorGUI.DrawPreviewTexture(new Rect(120, 120, 100, 100), texture);

        }
    }
#endif
}
