using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BuildingSystem
{

    public class Building : MonoBehaviour
    {
        public enum BuildState
        {
            None,
            Blocked,
            Valid,
            Built,
        }

        public event System.Action<Building> OnBuildingDestroyed;

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
                state = value;
                ToggleColliders(state == BuildState.Built);
                RefreshMaterials();
            }
        }


        private BuildingSettings settings;

        private readonly Dictionary<Renderer, Material[]> materialsByRenderer = new Dictionary<Renderer, Material[]>();
        private Collider[] colliders;

        private void Awake()
        {
            InitializeRenderers();
            InitializeColliders();
        }

        public void Init(BuildingSettings settings)
        {
            this.settings = settings;
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
                    SetMaterials(settings.BlockedMaterial);
                    break;
                case BuildState.Valid:
                    SetMaterials(settings.ValidMaterial);
                    break;
                case BuildState.Built:
                    SetMaterialsToDefault();
                    break;
            }
        }



        private void InitializeRenderers()
        {
            var renderers = model.GetComponentsInChildren<Renderer>();
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

        private void SetMaterialsToDefault()
        {
            foreach (var rendererAndMaterials in materialsByRenderer)
                rendererAndMaterials.Key.materials = rendererAndMaterials.Value;
        }

        private void SetMaterials(Material material)
        {
            foreach (var rendererAndMaterials in materialsByRenderer)
            {
                int len = rendererAndMaterials.Value.Length;
                for (int i = 0; i < len; i++)
                    rendererAndMaterials.Value[i] = material;
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
