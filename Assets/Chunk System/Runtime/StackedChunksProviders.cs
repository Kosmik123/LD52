using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.ChunkSystem.Generation
{
    public class StackedChunksProviders : MonoChunkProvider
    {
        [SerializeField]
        private List<Object> chunkProviders = new List<Object>();
        public List<IChunkProvider> ChunkProviders { get; } = new List<IChunkProvider>();

        private void Awake()
        {
            ChunkProviders.Clear();
            for (int i = 0; i < chunkProviders.Count; i++)
            {
                var provider = chunkProviders[i] as IChunkProvider;
                ChunkProviders.Add(provider);
            }
        }

        public override Chunk GetChunk()
        {
            for (int i = 0; i < ChunkProviders.Count; i++)
            {
                var chunk = ChunkProviders[i].GetChunk();
                if (chunk != null)
                    return chunk;
            }
            return null;
        }

        private void OnValidate()
        {
            for (int i = 0; i < chunkProviders.Count; i++)
            {
                var provider = chunkProviders[i] as IChunkProvider;
                chunkProviders[i] = (Object)provider;
            }
        }
    }
}
