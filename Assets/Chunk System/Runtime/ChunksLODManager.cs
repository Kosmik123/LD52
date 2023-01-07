using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.ChunkSystem
{
    [RequireComponent(typeof(ChunksData))]
    public class ChunksLODManager : MonoBehaviour
    {
        private ChunksData chunksData;

        [Header("To Link")]
        [SerializeField]
        private OnObserverChunkChangeListener listener;

        [Header("Settings")]
        [SerializeField]
        private int indexCheckDistance;

        private readonly Dictionary<Vector3Int, ChunkLOD> chunkLODsDictionary = new Dictionary<Vector3Int, ChunkLOD>();
        private readonly List<ChunkLOD> chunksSeenInPreviousFrame = new List<ChunkLOD>();
        //private readonly List<ChunkLOD> chunksSeenInThisFrame = new List<ChunkLOD>();

        private void Awake()
        {
            chunksData = GetComponent<ChunksData>();
        }

        private void OnEnable()
        {
            listener.OnChunkIndexChanged += HandleLODChange;
        }

        private void Start()
        {
            var index = chunksData.PositionToIndex(listener.Observer.position);
            HandleLODChange(index);
        }

        private List<ChunkLOD> chunksToBeDisabled = new List<ChunkLOD>();
        private void HandleLODChange(Vector3Int observerIndex)
        {
            chunksToBeDisabled.Clear();
            for (int i = 0; i < chunksSeenInPreviousFrame.Count; i++)
                chunksToBeDisabled.Add(chunksSeenInPreviousFrame[i]);

            chunksSeenInPreviousFrame.Clear();

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

                        ChunkLOD chunkLOD = GetChunkLOD(x, y, z);
                        if (chunkLOD != null)
                        {
                            chunksSeenInPreviousFrame.Add(chunkLOD);
                            chunkLOD.Refresh(chunksData.IndexToPosition(observerIndex));
                            chunksToBeDisabled.Remove(chunkLOD);
                        }
                    }
                }
            }

            foreach (var chunkToDisable in chunksToBeDisabled)
                chunkToDisable.DisableContent();
        }

        private ChunkLOD GetChunkLOD(int x, int y, int z)
        {
            var chunkLODIndex = new Vector3Int(x, y, z);
            if (chunkLODsDictionary.TryGetValue(chunkLODIndex, out var chunkLOD) == false)
            {
                var chunk = chunksData.GetChunk(x, y, z);
                if (chunk == null)
                    return null;
                if (chunk.TryGetComponent(out chunkLOD))
                {
                    chunkLODsDictionary.Add(chunkLODIndex, chunkLOD);
                    Debug.Log($"Adding chunk {chunk.gameObject.name} to LOD dictionary");
                }
            }

            return chunkLOD;
        }










        private void OnDisable()
        {
            listener.OnChunkIndexChanged -= HandleLODChange;
            foreach (var chunk in chunksData.AllChunks)
                if (chunk.TryGetComponent<ChunkLOD>(out var chunkLOD))
                    chunkLOD.EnableContent();
        }
    }
}
