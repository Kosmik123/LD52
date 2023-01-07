using System;
using UnityEngine;

namespace Bipolar.RaycastSystem
{
    public class RaycastController : MonoBehaviour
    {
        public enum RaycastType
        {
            Forward,
            Cursor,
        }

        public event Action<RaycastTarget> OnRayEnter;
        public event Action<RaycastTarget> OnRayExit;

        [Header("Settings")]
        [SerializeField]
        private new Camera camera;

        [SerializeField]
        private LayerMask raycastedLayers;
        public LayerMask RaycastedLayers 
        { 
            get => raycastedLayers; 
            set => raycastedLayers = value; 
        }
        
        [SerializeField]
        private float raycastDistance;
        public float RaycastDistance
        {
            get => raycastDistance;
            set => raycastDistance = value;
        }
        
        [Header("States")]
        [SerializeField]
        private RaycastType raycastMode;
        public RaycastType RaycastMode
        {
            get => raycastMode;
            set => raycastMode = value;
        }
        
        [SerializeField]
        private RaycastTarget currentTarget;
        public RaycastTarget Target => currentTarget;


        private Ray ray;

        private void Awake()
        {
            if (camera == null)
                camera = Camera.main;
        }

        private void Update()
        {
            DoRaycast();
        }

        private void DoRaycast()
        {
            ray = (raycastMode == RaycastType.Cursor) ? camera.ScreenPointToRay(Input.mousePosition) : new Ray(transform.position, transform.forward);
            if (TryGetRaycastTarget(ray, out var target))
            {
                if (target != currentTarget)
                {
                    if (currentTarget != null)
                        CallExitEvents(currentTarget);

                    currentTarget = target;
                    if (target != null)
                        CallEnterEvents(target);
                }
                else
                {
                    currentTarget.RayStay();
                }
            }
            else
            {
                if (currentTarget != null)
                {
                    CallExitEvents(currentTarget);
                    currentTarget = null;
                }
            }
        }

        private bool TryGetRaycastTarget(Ray ray, out RaycastTarget target)
        {
            target = null;
            if (Physics.Raycast(ray, out var hit, raycastDistance, raycastedLayers) == false)
                return false;

            if (hit.collider.TryGetComponent<RaycastCollider>(out var raycastCollider) == false)
                return false;

            return TryGetRaycastTarget(raycastCollider, out target);
        }

        static bool TryGetRaycastTarget(RaycastCollider collider, out RaycastTarget target)
        {
            target = collider.RaycastTarget;
            return target != null;
        }

        private void CallEnterEvents(RaycastTarget target)
        {
            OnRayEnter?.Invoke(target);
            target.RayEnter();
        }

        private void CallExitEvents(RaycastTarget target)
        {
            OnRayExit?.Invoke(target);
            target.RayExit();
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(ray.origin, ray.direction * raycastDistance);
        }

    }
}