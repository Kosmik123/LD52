using UnityEngine;
using System.Collections.Generic;

namespace Bipolar.ChunkSystem.Generation
{
    [CreateAssetMenu(menuName = "Chunk System/Generation/Stacked Map Generator")]
    public class StackedMapGenerator : ScriptableObject//, IMapProvider
    {
        private struct MapProviderWithStrength
        {
            [SerializeField]
            private Object mapProvider;
            public IMapProvider<float> MapProvider => mapProvider as IMapProvider<float>;

            [SerializeField]
            private float strength;
            public float Strength => strength;

            public void Validate()
            {
                mapProvider = (Object)MapProvider;
            }
        }

        [SerializeField]
        private List<MapProviderWithStrength> providersStrengths = new List<MapProviderWithStrength>();

        public bool Equals(IMapProvider other)
        {
            return false;
        }

        public float[,] GetMap(int width, int height, float xScale = 1, float yScale = 1, float xOffset = 0, float yOffset = 0, float maxValue = 1, float minValue = 0)
        {
            int count = providersStrengths.Count;
            float total = 0;
            for (int i = 0; i < count; i++)
                total += providersStrengths[i].Strength;

            float oneOverTotal = 1f / total;

            float[,] temp = new float[width, height];
            float[,] result = new float[width, height];
            for (int i = 0; i < count; i++)
            {
                var providerData = providersStrengths[i];
                float relativeStrength = providerData.Strength * oneOverTotal;
                providerData.MapProvider.GetMapNonAlloc(ref temp, width, height, xScale, yScale, xOffset, yOffset);








            }
            return result;
        }

        private void OnValidate()
        {
            foreach (var provider in providersStrengths)
            {
                provider.Validate();
            }
        }
    }
}
