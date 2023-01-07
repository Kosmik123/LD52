using UnityEngine;

namespace Bipolar.ChunkSystem
{
    public class OnObserverChunkChangeListener : MonoBehaviour
    {
        public event System.Action<Vector3Int> OnChunkIndexChanged;

        [SerializeField]
        private ChunksData data;

        [SerializeField]
        private Transform observer;
        public Transform Observer
        {
            get => observer;
            set => observer = value;
        }

        [Header("States")]
        [SerializeField]
        private Vector3Int observerIndex;
        private Vector3Int previousObserverIndex;
        public Vector3Int ObserverIndex => observerIndex;

        private void Update()
        {
            previousObserverIndex = observerIndex;
            observerIndex = data.PositionToIndex(observer.position.x, observer.position.y, observer.position.z);
            if (observerIndex != previousObserverIndex)
            {
                Debug.Log($"Nowy index gracza to: {observerIndex}");
                OnChunkIndexChanged?.Invoke(observerIndex);
            }
        }
    }
}
