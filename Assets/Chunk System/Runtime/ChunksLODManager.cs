using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.ChunkSystem
{
    [RequireComponent(typeof(ChunksData))]
    public class ChunksLODManager : MonoBehaviour
    {
        private ChunksData manager;

        [Header("To Link")]
        [SerializeField]
        private OnObserverChunkChangeListener listener;

        [Header("Settings")]
        [SerializeField]
        private int indexCheckDistance;

        private readonly Dictionary<Vector3Int, ChunkLOD> chunkLODsDictionary = new Dictionary<Vector3Int, ChunkLOD>();
        private readonly List<ChunkLOD> chunksSeenInPreviousFrame = new List<ChunkLOD>();

        private void Awake()
        {
            manager = GetComponent<ChunksData>();
        }

        private void OnEnable()
        {
            listener.OnChunkIndexChanged += HandleLODChange;
        }

        private void Start()
        {
            var index = manager.PositionToIndex(listener.Observer.position);
            HandleLODChange(index);
        }

        private void HandleLODChange(Vector3Int observerIndex)
        {
            for (int i = 0; i < chunksSeenInPreviousFrame.Count; i++)
                chunksSeenInPreviousFrame[i].DisableContent();
            chunksSeenInPreviousFrame.Clear();

            int xIndexCheckDistance = manager.Settings.ChunkSize.x == 0 ? 0 : indexCheckDistance;
            int yIndexCheckDistance = manager.Settings.ChunkSize.y == 0 ? 0 : indexCheckDistance;
            int zIndexCheckDistance = manager.Settings.ChunkSize.z == 0 ? 0 : indexCheckDistance;
            for (int z = observerIndex.z - xIndexCheckDistance; z <= observerIndex.z + xIndexCheckDistance; z++)
            {
                for (int y = observerIndex.y - yIndexCheckDistance; y <= observerIndex.y + yIndexCheckDistance; y++)
                {
                    for (int x = observerIndex.x - zIndexCheckDistance; x <= observerIndex.x + zIndexCheckDistance; x++)
                    {
                        var chunkLODIndex = new Vector3Int(x, y, z);
                        if (chunkLODsDictionary.TryGetValue(chunkLODIndex, out ChunkLOD chunkLOD) == false)
                        {
                            var chunk = manager.GetChunk(x, y, z);
                            if (chunk == null)
                                continue;
                            if (chunk.TryGetComponent(out chunkLOD))
                                chunkLODsDictionary.Add(chunkLODIndex, chunkLOD);
                        }
                        if (chunkLOD != null)
                        {
                            chunksSeenInPreviousFrame.Add(chunkLOD);
                            chunkLOD.Refresh(manager.IndexToPosition(observerIndex));
                        }
                    }
                }
            }
        }

        private void OnDisable()
        {
            listener.OnChunkIndexChanged -= HandleLODChange;
            foreach (var chunk in manager.AllChunks)
                if (chunk.TryGetComponent<ChunkLOD>(out var chunkLOD))
                    chunkLOD.EnableContent();
        }
    }
}
