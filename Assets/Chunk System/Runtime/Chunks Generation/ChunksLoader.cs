using UnityEngine;

namespace Bipolar.ChunkSystem.Generation
{
    [RequireComponent(typeof(ChunksData)), DisallowMultipleComponent]
    public class ChunksLoader : MonoBehaviour
    {
        [SerializeField]
        private ChunksData chunksData;

        public event System.Action<Chunk> OnChunkLoaded;

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
            for (int z = observerIndex.z - indexCheckDistance; z <= observerIndex.z + indexCheckDistance; z++)
            {
                for (int y = observerIndex.y - indexCheckDistance; y <= observerIndex.y + indexCheckDistance; y++)
                {
                    for (int x = observerIndex.x - indexCheckDistance; x <= observerIndex.x + indexCheckDistance; x++)
                    {
                        if (chunksData.GetChunk(x, y, z) != null)
                            continue;

                        var newChunk = ChunkInstancesProvider.GetChunk(x, y, z);
                        var index = chunksData.ValidateIndex(x, y, z);
                        newChunk.Init(chunksData.Settings, index);
                        chunksData.AddChunk(newChunk);
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
