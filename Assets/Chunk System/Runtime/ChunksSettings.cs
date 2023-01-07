using UnityEngine;

namespace Bipolar.ChunkSystem
{
    [CreateAssetMenu(menuName = "Chunk System/Chunk Settings")]
    public class ChunksSettings : ScriptableObject
    {
        [SerializeField]
        private Vector3 chunkSize;
        public Vector3 ChunkSize => chunkSize;

        private void OnValidate()
        {
            if (chunkSize.x < 0)
                chunkSize.x = 0;
            if (chunkSize.y < 0)
                chunkSize.y = 0;
            if (chunkSize.z < 0)
                chunkSize.z = 0;
        }

    }
}
