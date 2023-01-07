using UnityEngine;

namespace Bipolar.ChunkSystem.Generation
{
    [RequireComponent(typeof(ChunksData)), DisallowMultipleComponent]
    public class ChunksLoader : MonoBehaviour
    {
        public event System.Action<Chunk> OnChunkLoaded;
        
        [SerializeField]
        private ChunksData chunksData;

        [SerializeField]
        private OnObserverChunkChangeListener listener;
        public OnObserverChunkChangeListener OnObserverChunkChangeListener
        {
            get => listener;
            set
            {
                if (listener == value)
                    return;

                if (listener != null)
                    listener.OnChunkIndexChanged -= GenerateMissingChunks;
                listener = value;
                if (listener != null)
                    listener.OnChunkIndexChanged += GenerateMissingChunks;
            }
        }

        [Header("Settings")]
        [SerializeField]
        private int indexCheckDistance;

        [SerializeField]
        private Object chunkInstancesProvider;
        public IChunkInstanceProvider ChunkInstancesProvider
        {
            get => chunkInstancesProvider as IChunkInstanceProvider;
            set
            {
                chunkInstancesProvider = (Object)value;
                if (value != null)
                    ;// value.Init(chunksData);
            }
        }

        private void Awake()
        {
            if (ChunkInstancesProvider != null)
                ;// ChunkInstancesProvider.Init(chunksData);
        }

        private void Start()
        {
            var observerIndex = chunksData.PositionToIndex(listener.Observer.position);
            GenerateMissingChunks(observerIndex);
        }

        private void OnEnable()
        {
            GenerateMissingChunks(listener != null ? listener.ObserverIndex : Vector3Int.zero);
            if (listener != null)
                listener.OnChunkIndexChanged += GenerateMissingChunks;
        }

        private void GenerateMissingChunks(Vector3Int observerIndex)
        {
            Vector3 chunkSize = chunksData.Settings.ChunkSize;
            int xIndexCheckDistance = chunkSize.x == 0 ? 0 : indexCheckDistance;
            int yIndexCheckDistance = chunkSize.y == 0 ? 0 : indexCheckDistance;
            int zIndexCheckDistance = chunkSize.z == 0 ? 0 : indexCheckDistance;

            int x, y, z;
            for (int k = -zIndexCheckDistance; k <= zIndexCheckDistance; k++)
            {
                for (int j = -yIndexCheckDistance; j <= yIndexCheckDistance; j++)
                {
                    for (int i = -xIndexCheckDistance; i <= xIndexCheckDistance; i++)
                    {
                        x = i + observerIndex.x;
                        y = j + observerIndex.y;
                        z = k + observerIndex.z;

                        if (chunksData.GetChunk(x, y, z) != null)
                            continue;

                        var newChunk = ChunkInstancesProvider.GetChunk(x, y, z);
                        if (newChunk == null)
                            continue;

                        chunksData.AddChunk(newChunk);
                        Debug.Log($"Adding missing chunk {newChunk.gameObject.name}");
                        newChunk.transform.localPosition = chunksData.IndexToPosition(new Vector3Int(x, y, z));
                        OnChunkLoaded?.Invoke(newChunk);
                    }
                }
            }
        }

        private void OnDisable()
        {
            if (listener != null)
               listener.OnChunkIndexChanged -= GenerateMissingChunks;
        }

        private void OnValidate()
        {
            ChunkInstancesProvider = ChunkInstancesProvider;
            if (indexCheckDistance < 0)
                indexCheckDistance = 0;
        }
    }
}
