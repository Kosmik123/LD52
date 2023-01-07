using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.ChunkSystem
{ 
    public class ChunkLOD : MonoBehaviour
    {
        public event System.Action<float> OnChunkLODRefreshed;

        [System.Serializable]
        public struct LOD
        {
            public float startingDistance;
            public Mesh mesh;
        }

        [Header("To Link")]
        [SerializeField]
        private Chunk chunk;
        [SerializeField]
        private MeshFilter replacementsMeshFilter;

        [Header("Settings")]
        [SerializeField]
        private List<LOD> meshReplacements = new List<LOD>();
        public List<LOD> MeshReplacements => meshReplacements;

#if UNITY_EDITOR
        [ContextMenu("Setup Component")]
        private void SetupComponent()
        {
            // SETUP CHUNK
            chunk = GetComponent<Chunk>();
            if (chunk == null)
                chunk = GetComponentInParent<Chunk>();
            if (chunk == null)
                Debug.LogError("Chunk component cannot be located", gameObject);

            // SETUP MESH FILTER
            var allMeshRenderers = GetComponentsInChildren<MeshRenderer>();
            foreach (var renderer in allMeshRenderers)
            {
                if (renderer.transform == transform || renderer.transform.parent == transform)
                {
                    var meshFilter = renderer.GetComponent<MeshFilter>();
                    if (meshFilter == null)
                        meshFilter = renderer.gameObject.AddComponent<MeshFilter>();

                    replacementsMeshFilter = meshFilter;
                    return;
                }
            }
            var filterObject = new GameObject("Replacement Mesh Filter");
            filterObject.transform.parent = transform;
            filterObject.transform.localPosition = Vector3.zero;
            filterObject.transform.localScale = Vector3.one;
            replacementsMeshFilter = filterObject.AddComponent<MeshFilter>();
            filterObject.AddComponent<MeshRenderer>();

            if (chunk.Content == null)
            {
                var contentGameObject = new GameObject("Content");
                var contentTransform = contentGameObject.transform;
                contentTransform.parent = chunk.transform;
                contentTransform.localPosition = Vector3.zero;
                contentTransform.SetAsFirstSibling();
                chunk.Content = contentTransform;
            }
            var content = chunk.Content;
            var replacementTransform = filterObject.transform;
            if (content == replacementTransform || 
                content == replacementTransform.parent || 
                content.parent == replacementTransform)
            {
                Debug.LogError("Chunk content and replacement mesh cannot be in parent-child relation", gameObject);
            }
        }
#endif 
        internal void Refresh(Vector3 observerPosition)
        {
            Vector3 position = new Vector3(
                transform.position.x + 0.5f * chunk.Settings.ChunkSize.x,
                transform.position.y + 0.5f * chunk.Settings.ChunkSize.y,
                transform.position.z + 0.5f * chunk.Settings.ChunkSize.z);

            float distance = (position - observerPosition).magnitude;
            for (int i = meshReplacements.Count - 1; i >= 0; i--)
            {
                if (distance < meshReplacements[i].startingDistance)
                    continue;

                chunk.Content.gameObject.SetActive(false);
                replacementsMeshFilter.gameObject.SetActive(true);
                replacementsMeshFilter.mesh = meshReplacements[i].mesh;
                return;
            }
            EnableContent();
            OnChunkLODRefreshed?.Invoke(distance);
        }

        public void EnableContent()
        {
            replacementsMeshFilter.gameObject.SetActive(false);
            chunk.Content.gameObject.SetActive(true);
        }

        public void DisableContent()
        {
            chunk.Content.gameObject.SetActive(false);
        }

        private void OnValidate()
        {
            if (meshReplacements != null)
                meshReplacements.Sort((lhs, rhs) => lhs.startingDistance.CompareTo(rhs.startingDistance));
        }

    }
}
