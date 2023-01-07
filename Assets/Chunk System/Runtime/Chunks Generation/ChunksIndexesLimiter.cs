using UnityEngine;

namespace Bipolar.ChunkSystem.Generation
{
    public class ChunksIndexesLimiter : MonoBehaviour, IChunkInstanceProvider
    {
        [SerializeField]
        private Object chunksProvider;
        public IChunkInstanceProvider ChunksProvider
        {
            get => chunksProvider as IChunkInstanceProvider;
            set
            {
                chunksProvider = (Object)value;
            }
        }

        [SerializeField]
        private Vector3Int minIndex;
        [SerializeField]
        private Vector3Int maxIndex;

        public Chunk GetChunk(int x, int y, int z)
        {
            if (x < minIndex.x || x > maxIndex.x)
                return null;
            if (y < minIndex.y || y > maxIndex.y)
                return null;
            if (z < minIndex.z || z > maxIndex.z)
                return null;

            return ChunksProvider.GetChunk(x, y, z);
        }
    }
}
