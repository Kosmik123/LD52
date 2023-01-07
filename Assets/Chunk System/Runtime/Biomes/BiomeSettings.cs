using Bipolar.ChunkSystem.Generation;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Bipolar.ChunkSystem.Biomes
{
    [CreateAssetMenu(menuName = "Chunk System/Biomes/Biome Settings")]
    public class BiomeSettings : ScriptableObject
    {
        [SerializeField]
        private Biome[] biomes;
        public Biome[] Biomes => biomes;

        [SerializeField, Range(0, 1f)]
        private float transitionLength;
        public float TransitionLength
        {
            get => transitionLength;
            set => transitionLength = value;
        }
    }

    public abstract class Biome : ScriptableObject
    {
        [SerializeField, Tooltip("IMapProvider<float>")]
        private Object heightmapGenerator;
        public IMapProvider HeightmapGenerator
        {
            get => heightmapGenerator as IMapProvider;
            set
            {
                heightmapGenerator = value as Object;
            }
        }

        protected virtual void OnValidate()
        {
            HeightmapGenerator = HeightmapGenerator;
        }
    }

    

}
