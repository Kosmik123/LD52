using System;
using System.Text;
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
                GetIndexComponent(transform.localPosition.x, settings.ChunkSize.x),
                GetIndexComponent(transform.localPosition.y, settings.ChunkSize.y),
                GetIndexComponent(transform.localPosition.z, settings.ChunkSize.z)));
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

            gameObject.name = GetName();
            OnChunkInitialized?.Invoke();
        }

        private string GetName()
        {
            bool hasX = settings.ChunkSize.x != 0;
            bool hasY = settings.ChunkSize.y != 0;
            bool hasZ = settings.ChunkSize.z != 0;

            StringBuilder nameBuilder = new StringBuilder("Chunk (");
            if (hasX)
            {
                nameBuilder.Append($"{index.x}");
                if (hasY || hasZ)
                    nameBuilder.Append(",");
            }
            if (hasY)
            {
                nameBuilder.Append($"{index.y}");
                if (hasZ)
                    nameBuilder.Append(",");
            }
            if (hasZ)
            {
                nameBuilder.Append($"{index.z}");
            }
            nameBuilder.Append(")");
            return nameBuilder.ToString();
        }


        public void RefreshPosition(Vector3Int shift)
        {
            var size = settings.ChunkSize;
            transform.localPosition = new Vector3(
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
                GetIndexComponent(transform.localPosition.x, size.x),
                GetIndexComponent(transform.localPosition.y, size.y),
                GetIndexComponent(transform.localPosition.z, size.z));

            transform.localPosition = chunksData.IndexToPosition(index);
        }
#endif
    }
}
