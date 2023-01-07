using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.ChunkSystem
{
    public class ChunksData : MonoBehaviour
    {
        public event Action<Chunk> OnChunkAdded;

        [SerializeField]
        private Transform container;

        [SerializeField]
        private ChunksSettings settings;
        public ChunksSettings Settings => settings;
        public Vector3 ChunkSize => settings.ChunkSize;

        [Header("States")]
        [SerializeField]
        private List<Chunk> chunks = new List<Chunk>();
        private readonly Dictionary<Vector3Int, Chunk> chunksDictionary = new Dictionary<Vector3Int, Chunk>();

        public Dictionary<Vector3Int, Chunk>.ValueCollection AllChunks => chunksDictionary.Values;

        [SerializeField]
        private Vector3Int chunksShift;
        public Vector3Int ChunksShift 
        { 
            get => chunksShift; 
            set => chunksShift = value; 
        }

        private void Awake()
        {
            chunks.Clear();
            var chunksInContainer = GetChunksFromContainer();
            foreach (var chunk in chunksInContainer)
            {
                //if (chunk.gameObject.activeInHierarchy)
                {
                    chunk.Init(settings);
                    chunks.Add(chunk);
                }
            }
            InitializeDictionary();
        }

        private Chunk[] GetChunksFromContainer()
        {
            if (container == null)
                return FindObjectsOfType<Chunk>();

            return container.GetComponentsInChildren<Chunk>();
        }

        private void InitializeDictionary()
        {
            chunksDictionary.Clear();
            foreach (var chunk in chunks)
                chunksDictionary[chunk.Index] = chunk;
        }

        public Vector3Int ValidateIndex(Vector3Int index)
        {
            return ValidateIndex(index.x, index.y, index.z);
        }

        public Vector3Int ValidateIndex(int x, int y, int z)
        {
            return new Vector3Int(
                ChunkSize.x == 0 ? 0 : x,
                ChunkSize.y == 0 ? 0 : y,
                ChunkSize.z == 0 ? 0 : z);
        }

        public Vector3 IndexToPosition(Vector3Int index)
        {
            return new Vector3(
               (index.x + chunksShift.x) * ChunkSize.x,
               (index.y + chunksShift.y) * ChunkSize.y,
               (index.z + chunksShift.z) * ChunkSize.z);
        }

        public Vector3Int PositionToIndex(Vector3 position) => PositionToIndex(position.x, position.y, position.z);

        public Vector3Int PositionToIndex(float x, float y, float z)
        {
            return ValidateIndex(-chunksShift + new Vector3Int(
                Mathf.RoundToInt(x / ChunkSize.x),
                Mathf.RoundToInt(y / ChunkSize.y),
                Mathf.RoundToInt(z / ChunkSize.z)));
        }

        public void AddChunk(Chunk chunk)
        {
            var index = ValidateIndex(chunk.Index);

            chunksDictionary.Add(index, chunk);
            chunks.Add(chunk);
            OnChunkAdded?.Invoke(chunk);
        }

        public void Init(ChunksData chunksManager)
        {
        }

        public Chunk GetChunk(int x, int z)
        {
            return GetChunk(x, 0, z);
        }

        public Chunk GetChunk(int x, int y, int z)
        {
            if (ChunkSize.x == 0)
                x = 0;
            if (ChunkSize.y == 0)
                y = 0;
            if (ChunkSize.z == 0)
                z = 0;

            if (chunksDictionary.TryGetValue(new Vector3Int(x, y, z), out var chunk))
                return chunk;

            return null;
        }
    }
}
