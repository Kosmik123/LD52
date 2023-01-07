using UnityEngine;

namespace Bipolar.ChunkSystem.Generation
{
    public abstract class MonoChunkProvider : MonoBehaviour, IChunkProvider
    {
        public abstract Chunk GetChunk();
    }
}
