using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuildingSystem
{
    public class BuildingManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private BuildingSettings settings;
        [SerializeField]
        private BuildingVisual[] visualPrefabs;
        [SerializeField]
        private CursorController cursor;

        [SerializeField]
        private Transform buildingsContainer;
        [SerializeField]
        private Transform buildableBuildingsContainer;

        [SerializeField]
        private LayerMask collidingLayers;

        [Header("Properties")]
        [SerializeField, ReadOnly]
        private BuildingVisual[] cursorTemplates;

        [Header("States")]
        [SerializeField, ReadOnly]
        private int currentBuildingIndex;
        public int CurrentBuildingIndex
        {
            get => currentBuildingIndex;
            set => ChangeIndex(value);
        }

        [SerializeField, ReadOnly]
        private BuildingVisual currentlyHeldBuilding;

        [SerializeField, ReadOnly]
        private List<Building> buildings;
        private readonly Dictionary<Vector2, Building> buildingsDictionary = new Dictionary<Vector2, Building>();

        [SerializeField, ReadOnly]
        private bool canBuild;

        [SerializeField, ReadOnly]
        private bool destroyingMode;

        private void Awake()
        {
            cursorTemplates = new BuildingVisual[visualPrefabs.Length];
            for (int i = 0; i < visualPrefabs.Length; i++)
            {
                var template = Instantiate(visualPrefabs[i], buildableBuildingsContainer);
                cursorTemplates[i] = template;
                template.Init(settings);
                template.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            CurrentBuildingIndex = 0;
        }

        private Building SpawnBuilding(BuildingVisual buildingTemplate, float x, float z, float angle)
        {
            var buildingGO = new GameObject($"{buildingTemplate.gameObject.name} ({x}, {z})");
            buildingGO.transform.SetPositionAndRotation(new Vector3(x, 0, z), Quaternion.AngleAxis(angle, Vector3.up));

            var buildingVisual = Instantiate(buildingTemplate, buildingGO.transform);
            buildingVisual.SetMaterialsDict(buildingTemplate.MaterialsByRenderer);
            buildingVisual.transform.localPosition = Vector3.zero;
            buildingVisual.transform.localRotation = Quaternion.identity;
            buildingVisual.State = BuildingVisual.BuildState.Built;

            var building = buildingGO.AddComponent<Building>();
            building.Init(buildingVisual);

            return building;
        }

        private void RemoveFromList(Building building)
        {
            buildings.Remove(building);
        }

        private void ChangeIndex (int index)
        {
            if (currentlyHeldBuilding != null)
            {
                currentlyHeldBuilding.gameObject.SetActive(false);
                currentlyHeldBuilding.transform.parent = buildableBuildingsContainer;
            }

            currentBuildingIndex = (index + cursorTemplates.Length) % cursorTemplates.Length;
            currentlyHeldBuilding = cursorTemplates[currentBuildingIndex];
            currentlyHeldBuilding.gameObject.SetActive(true);
            currentlyHeldBuilding.transform.parent = cursor.Container;
            currentlyHeldBuilding.transform.localPosition = Vector3.zero;
            currentlyHeldBuilding.transform.localRotation = Quaternion.identity;
            cursor.Size = currentlyHeldBuilding.Size;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
                ToggleMode();



            canBuild = !Physics.CheckBox(cursor.transform.position + new Vector3(0, 0.5f), 0.45f * new Vector3(cursor.Size.x, 1, cursor.Size.y), cursor.transform.rotation, collidingLayers);
           
            if (destroyingMode)
                HandleDestroying();
            else
                HandleBuilding();
        }

        private void ToggleMode()
        {
            destroyingMode = !destroyingMode;
            cursor.SetColor(destroyingMode ? Color.red : Color.white);
            for (int i = 0; i < cursorTemplates.Length; i++)
                cursorTemplates[i].gameObject.SetActive(destroyingMode == false && currentBuildingIndex == i);
        }

        private void HandleDestroying()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hitInfo, 40, collidingLayers))
                {
                    var collider = hitInfo.collider;
                    if (collider == null)
                        return;

                    var targettedBuilding = collider.GetComponentInParent<Building>();
                    if (targettedBuilding != null)
                        DestroyBuilding(targettedBuilding);
                }
            }    
        }

        private void HandleBuilding()
        {
            float scroll = Input.mouseScrollDelta.y;
            if (scroll != 0)
                CurrentBuildingIndex += Mathf.RoundToInt(scroll);

            if (canBuild)
            {
                currentlyHeldBuilding.State = BuildingVisual.BuildState.Valid;
                if (Input.GetMouseButtonDown(0))
                {
                    Build(visualPrefabs[currentBuildingIndex],
                        cursor.Position.x, cursor.Position.y, cursor.Angle);
                }
            }
            else
            {
                currentlyHeldBuilding.State = BuildingVisual.BuildState.Blocked;
            }
        }

        private void Build(BuildingVisual template, float x, float z, float angle)
        {
            var building = SpawnBuilding(template, x, z, angle);
            building.transform.parent = buildingsContainer;
            buildings.Add(building);
        }

        private void DestroyBuilding(Building building)
        {
            RemoveFromList(building);
            Destroy(building.gameObject);
        }

    }
}
