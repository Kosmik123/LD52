using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.ChunkSystem.MeshChunks
{
    public class ChunkMeshManager : MonoBehaviour
    {
        public static event System.Action<ChunkMesh> OnColliderCreated;

        private readonly Queue<ChunkMesh> meshChunksToActivate = new Queue<ChunkMesh>();

        private void OnEnable()
        {
            ChunkMesh.OnMeshCreated += EnqueueMesh;
        }

        private void EnqueueMesh(ChunkMesh chunk)
        {
            meshChunksToActivate.Enqueue(chunk);
        }

        private void Update()
        {
            if (meshChunksToActivate.Count > 0)
            {
                var chunk = meshChunksToActivate.Dequeue();
                var mesh = chunk.MeshFilter.mesh;
                var collider = chunk.MeshCollider;
                if (collider != null)
                {
                    collider.sharedMesh = mesh;
                    collider.enabled = true;
                    OnColliderCreated?.Invoke(chunk);
                }
            }
        }

        private void OnDisable()
        {
            ChunkMesh.OnMeshCreated -= EnqueueMesh;
        }
    }
}
