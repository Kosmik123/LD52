using UnityEngine;

namespace Bipolar.ChunkSystem.Generation.WaveFunctionCollapse
{
    public class WaveFunctionCollapseGenerationStrategy : ScriptableChunkProvider
    {
        [SerializeField]
        private ChunkCollapseData[] allPossibleChunks;
        public ChunkCollapseData[] AllPossibleChunks => allPossibleChunks;

        private void Awake()
        {

        }

        public override Chunk GetChunk()
        {
            return default;
        }
    }
}
