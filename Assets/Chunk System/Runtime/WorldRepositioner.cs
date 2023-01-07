using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.ChunkSystem
{
    [RequireComponent(typeof(ChunksData)), DisallowMultipleComponent]
    public class WorldRepositioner : MonoBehaviour
    {
        private ChunksData data;

        [SerializeField]
        private Transform observer;
        public Transform Observer
        {
            get => observer;
            set => observer = value;
        }

        [SerializeField, Tooltip("Observers distance from position (0, 0, 0) at which chunk shift will be invoked")]
        private float limitDistance = 1000;
        public float LimitDistance => limitDistance;

        [SerializeField]
        private float minTimeBetweenShifts = 1;
        public float MinTimeBetweenShifts { get => minTimeBetweenShifts; set => minTimeBetweenShifts = value; }
        
        private float lastShiftTime;
        public float LastShiftTime => lastShiftTime;
        public bool CanShift => isWaiting == false && Time.time > lastShiftTime + minTimeBetweenShifts;
        
        private bool isWaiting;
        WaitForSeconds wait;

        private void Awake()
        {
            wait = new WaitForSeconds(Time.fixedDeltaTime * 2);
            data = GetComponent<ChunksData>();
        }

        private void LateUpdate()
        {
            HandleWorldShift();
        }

        public void HandleWorldShift()
        {
            if (CanShift == false)
                return;

            float x = observer.position.x;
            float y = observer.position.y;
            float z = observer.position.z;
            
            var size = data.ChunkSize;
            float x2 = size.x == 0 ? 0 : x * x;
            float y2 = size.y == 0 ? 0 : y * y;      
            float z2 = size.z == 0 ? 0 : z * z;

            float sqrDistance = x2 + y2 + z2;
            if (sqrDistance > limitDistance * limitDistance)
            {
                int shiftX = CalculateShift(x, size.x);
                int shiftY = CalculateShift(y, size.y);
                int shiftZ = CalculateShift(z, size.z);

                lastShiftTime = Time.time;
                ShiftWorld(shiftX, shiftY, shiftZ);
            }
        }

        private static int CalculateShift(float positionComponent, float sizeComponent)
        {
            if (sizeComponent == 0)
                return 0;

            return positionComponent < 0 ?
                -Mathf.FloorToInt(positionComponent / sizeComponent) :
                -Mathf.CeilToInt(positionComponent / sizeComponent);
        }

        private void ShiftWorld(int xChunkShift, int yChunkShift, int zChunkShift)
        {
            bool autoSyncTransforms = Physics.autoSyncTransforms;
            Physics.autoSyncTransforms = true;
            var previousShift = data.ChunksShift;
            var currentShift = previousShift + new Vector3Int(xChunkShift, yChunkShift, zChunkShift);

            foreach (var chunk in data.AllChunks)
                RefreshChunkPosition(chunk, currentShift);

            Vector3 newPosition = new Vector3(
                observer.position.x + (currentShift.x - previousShift.x) * data.ChunkSize.x,
                observer.position.y + (currentShift.y - previousShift.y) * data.ChunkSize.y,
                observer.position.z + (currentShift.z - previousShift.z) * data.ChunkSize.z);
            observer.position = newPosition;
            data.ChunksShift = currentShift;
            StartCoroutine(RevertAutoSyncTransformsCo(autoSyncTransforms));
        }

        private IEnumerator RevertAutoSyncTransformsCo(bool value)
        {
            isWaiting = true;
            yield return wait;
            Physics.autoSyncTransforms = value;
            isWaiting = false;
        }

        private void RefreshChunkPosition(Chunk chunk, Vector3Int shift)
        {
            var size = data.ChunkSize;
            chunk.transform.position = new Vector3(
                (chunk.Index.x + shift.x) * size.x,
                (chunk.Index.y + shift.y) * size.y,
                (chunk.Index.z + shift.z) * size.z);
        }

        /*
        [ContextMenu("Rearrange")]
        private void RearrangeChunksHierarchy()
        {
            var allChunks = GetComponentsInChildren<Chunk>();
            foreach (var chunk in allChunks)
            {
                var parentChunk = Instantiate(chunk);
                var parentObject = parentChunk.gameObject;
                parentObject.name = $"Chunk ({parentChunk.Index.x}, {parentChunk.Index.y})";
                while(parentObject.transform.childCount > 0)
                    DestroyImmediate(parentObject.transform.GetChild(0).gameObject);

                parentObject.transform.parent = chunk.transform.parent;
                chunk.transform.parent = parentObject.transform;
                parentChunk.Transform = chunk.transform;
                chunk.gameObject.name = "Chunk Content";
                DestroyImmediate(chunk);
            }
        }
        */
    }
}
