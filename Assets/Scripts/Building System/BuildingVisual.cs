using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BuildingSystem
{
    public class BuildingVisual : MonoBehaviour
    {
        public enum BuildState
        {
            None,
            Blocked,
            Valid,
            Built,
        }

        public event System.Action<BuildingVisual> OnBuildingDestroyed;

        [SerializeField]
        private Transform model;

        [SerializeField]
        private Vector2Int size;
        public Vector2Int Size => size;

        [SerializeField, ReadOnly]
        private BuildState state;
        public BuildState State
        {
            get => state;
            set
            {
                if (state != value)
                {
                    state = value;
                    ToggleColliders(state == BuildState.Built);
                    RefreshMaterials();
                }
            }
        }

        [SerializeField, ReadOnly]
        private bool isInited;

        [SerializeField, ReadOnly] // debug
        private Material[] currentMaterials;

        private BuildingSettings settings;

        private readonly Dictionary<Renderer, Material[]> materialsByRenderer = new Dictionary<Renderer, Material[]>();
        public Dictionary<Renderer, Material[]> MaterialsByRenderer => materialsByRenderer;

        private readonly Dictionary<Renderer, Material[]> validMaterialsByRenderer = new Dictionary<Renderer, Material[]>();
        private readonly Dictionary<Renderer, Material[]> blockedMaterialsByRenderer = new Dictionary<Renderer, Material[]>();

        [SerializeField]
        private Collider[] colliders;

        public void Init(BuildingSettings settings)
        {
            InitializeRenderers();
            InitializeColliders();

            this.settings = settings;
            foreach (var rendererAndMaterials in materialsByRenderer)
            {
                var renderer = rendererAndMaterials.Key;
                int len = rendererAndMaterials.Value.Length;
                Material[] validMaterials = new Material[len];
                Material[] blockedMaterials = new Material[len];
                for (int i = 0; i < len; i++)
                {
                    validMaterials[i] = settings.ValidMaterial;
                    blockedMaterials[i] = settings.BlockedMaterial;
                }

                validMaterialsByRenderer[renderer] = validMaterials;
                blockedMaterialsByRenderer[renderer] = blockedMaterials;
            }
        }

        internal void SetMaterialsDict(Dictionary<Renderer, Material[]> renderersAndMaterials)
        {
            materialsByRenderer.Clear();
            foreach (var kvp in renderersAndMaterials)
                materialsByRenderer.Add(kvp.Key, kvp.Value);
        }

        private void InitializeColliders()
        {
            colliders = GetComponentsInChildren<Collider>();
        }

        private void RefreshMaterials()
        {
            switch (state)
            {
                case BuildState.Blocked:
                    SetMaterialsFromDict(blockedMaterialsByRenderer);
                    break;
                case BuildState.Valid:
                    SetMaterialsFromDict(validMaterialsByRenderer);
                    break;
                case BuildState.Built:
                    SetMaterialsFromDict(materialsByRenderer);
                    break;
            }
        }

        private void InitializeRenderers()
        {
            var renderers = model.GetComponentsInChildren<Renderer>();
            materialsByRenderer.Clear();
            foreach (var renderer in renderers)
            {
                int len = renderer.materials.Length;
                var materialsArray = new Material[len];
                System.Array.Copy(renderer.materials, materialsArray, len);
                materialsByRenderer.Add(renderer, materialsArray);
            }
        }

        private void ToggleColliders(bool enable)
        {
            foreach (var collider in colliders)
                collider.enabled = enable;
        }

        private void SetMaterialsFromDict(Dictionary<Renderer, Material[]> dictionary)
        {
            foreach (var rendererAndMaterials in dictionary)
            {
                currentMaterials = rendererAndMaterials.Key.materials = rendererAndMaterials.Value;
            }
        }

        private void OnDestroy()
        {
            OnBuildingDestroyed?.Invoke(this);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position + Vector3.up * 0.4f, new Vector3(size.x, 0.8f, size.y));
        }
    }
}
