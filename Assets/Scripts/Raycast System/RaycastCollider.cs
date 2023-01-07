using NaughtyAttributes;
using UnityEngine;

namespace Bipolar.RaycastSystem
{
    [RequireComponent(typeof(Collider))]
    public class RaycastCollider : MonoBehaviour
    {
        public const int ignoreRaycastLayer = 2;

        [SerializeField]
        private RaycastTarget[] raycastTargets;
        public RaycastTarget RaycastTarget
        {
            get
            {
                foreach(var target in raycastTargets)
                    if (target.enabled)
                        return target;

                return default;
            }
        }

        [SerializeField, ReadOnly]
        private int initialLayer;

        private void Reset()
        {
            raycastTargets = GetComponentsInParent<RaycastTarget>();
        }

        private void Awake()
        {
            initialLayer = gameObject.layer;
        }
    }
}
