using UnityEngine;

namespace Bipolar.ChunkSystem.Generation
{
    public abstract class Smoothing : MonoBehaviour, IMapModifier
    {
        [SerializeField]
        protected SmoothingSettings settings;

        public abstract void ModifyMap(ref float[,] map);
    }
}
