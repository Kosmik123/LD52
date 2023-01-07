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
            var newChunk = Instantiate(ChunksProvider.GetChunk(), chunksContainer);

            return newChunk;
        }

        private void OnValidate()
        {
            ChunksProvider = ChunksProvider;
        }
    }
}
