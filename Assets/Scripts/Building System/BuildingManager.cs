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
        private List<Building> buildings;
        private readonly Dictionary<Vector2Int, Building> buildingsDictionary = new Dictionary<Vector2Int, Building>();

        [SerializeField]
        private CursorSize cursor;

        [SerializeField]
        private Transform buildingsContainer;
        [SerializeField]
        private Transform buildableBuildingsContainer;

        [SerializeField]
        private LayerMask collidingLayers;

        [Header("Properties")]
        [SerializeField, ReadOnly]
        private Building[] templates;

        [Header("States")]
        [SerializeField, ReadOnly]
        private int currentBuildingIndex;
        public int CurrentBuildingIndex
        {
            get => currentBuildingIndex;
            set => ChangeIndex(value);
        }

        [SerializeField, ReadOnly]
        private Building currentlyHeldBuilding;

        private void Awake()
        {
            templates = buildableBuildingsContainer.GetComponentsInChildren<Building>(true); 
        }


        private void OnEnable()
        {
            foreach (var building in templates)
            {
                building.Init(settings);
                building.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            CurrentBuildingIndex = 0;
        }

        private Building SpawnBuilding(Building buildingTemplate)
        {
            var building = Instantiate(buildingTemplate, buildingTemplate.transform.position, buildingTemplate.transform.rotation, buildingsContainer);
            building.OnBuildingDestroyed += RemoveFromList;
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

            currentBuildingIndex = (index + templates.Length) % templates.Length;
            currentlyHeldBuilding = templates[currentBuildingIndex];
            currentlyHeldBuilding.gameObject.SetActive(true);
            currentlyHeldBuilding.transform.parent = cursor.transform;
            currentlyHeldBuilding.transform.localPosition = Vector3.zero;
            currentlyHeldBuilding.transform.localRotation = Quaternion.identity;
            cursor.Size = currentlyHeldBuilding.Size;
        }

        private void Update()
        {
            HandleBuilding();
        }

        private void HandleBuilding()
        {
            if (Physics.CheckBox(cursor.transform.position + new Vector3(0, 0.5f), 0.5f * new Vector3(cursor.Size.x, 1, cursor.Size.y), cursor.transform.rotation, collidingLayers))
            {
                currentlyHeldBuilding.State = Building.BuildState.Blocked;
            }
            else
            {
                currentlyHeldBuilding.State = Building.BuildState.Valid;
                if (Input.GetMouseButtonDown(0))
                {
                    Build(currentlyHeldBuilding);
                }
            }
        }

        private void Build(Building template)
        {
            var building = SpawnBuilding(template);
            building.State = Building.BuildState.Built;
        }
    }
}
