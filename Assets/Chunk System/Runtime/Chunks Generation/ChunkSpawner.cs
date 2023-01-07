using UnityEngine;

namespace Bipolar.ChunkSystem.Generation
{
    public class ChunkSpawner : MonoBehaviour, IChunkInstanceProvider
    {
        [Header("To Link")]
        [SerializeField]
        private Transform chunksContainer;

        [SerializeField]
        private ChunksData chunksData;

        [SerializeField]
        private Object chunksProvider;
        public IChunkProvider ChunksProvider
        {
            get => chunksProvider as IChunkProvider;
            set
            {
                chunksProvider = (Object)value;
                if (value != null)
                    ;// value.Init(chunksData);
            }
        }

        public Chunk GetChunk(int x, int y, int z)
        {
            var chunkTemplate = ChunksProvider.GetChunk();
            if (chunkTemplate == null)
                return null;

            var newChunk = Instantiate(chunkTemplate, chunksContainer);
            var index = chunksData.ValidateIndex(x, y, z);
            newChunk.Init(chunksData.Settings, index);

            return newChunk;
        }

        private void OnValidate()
        {
            ChunksProvider = ChunksProvider;
        }
    }
}
