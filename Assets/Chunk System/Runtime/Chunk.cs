using System;
using UnityEngine;

namespace Bipolar.ChunkSystem
{
    public class Chunk : MonoBehaviour
    {
        public event Action OnChunkInitialized;

        private ChunksSettings settings;
        public ChunksSettings Settings => settings;

        [SerializeField]
        private Vector3Int index;
        public Vector3Int Index => index;

        [SerializeField]
        private Transform content;
        public Transform Content
        {
            get => content;
            set => content = value;
        }

        public void Init(ChunksSettings settings)
        {
            Init(settings, new Vector3Int(
                GetIndexComponent(transform.position.x, settings.ChunkSize.x),
                GetIndexComponent(transform.position.y, settings.ChunkSize.y),
                GetIndexComponent(transform.position.z, settings.ChunkSize.z)));
        }

        private int GetIndexComponent(float positionComponent, float chunkSizeComponent)
        {
            if (chunkSizeComponent == 0)
                return 0;

            return Mathf.RoundToInt(positionComponent / chunkSizeComponent); 
        }

        public void Init(ChunksSettings settings, Vector3Int index)
        {
            this.settings = settings;
            this.index = index;
            OnChunkInitialized?.Invoke();
        }

        public void RefreshPosition(Vector3Int shift)
        {
            var size = settings.ChunkSize;
            transform.position = new Vector3(
                (index.x + shift.x) * size.x,
                (index.y + shift.y) * size.y,
                (index.z + shift.z) * size.z);
        }


#if UNITY_EDITOR
        [ContextMenu("Snap")]
        private void Snap()
        {
            var chunksData = transform.GetComponentInParent<ChunksData>();
            if (chunksData == null)
                chunksData = FindObjectOfType<ChunksData>();
            var size = chunksData.ChunkSize;
            index = new Vector3Int(
                GetIndexComponent(transform.position.x, size.x),
                GetIndexComponent(transform.position.y, size.y),
                GetIndexComponent(transform.position.z, size.z));

            transform.position = chunksData.IndexToPosition(index);
        }
#endif
    }
}
