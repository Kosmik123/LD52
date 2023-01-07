using UnityEngine;

namespace Bipolar.ChunkSystem.Generation
{
    public abstract class ScriptableChunkProvider : ScriptableObject , IChunkProvider
    {
        public abstract Chunk GetChunk();
    }
}
