using UnityEngine;

namespace Bipolar.ChunkSystem.Generation
{
    [CreateAssetMenu(menuName = "Chunk System/Generation/Smoothing Settings")]
    public class SmoothingSettings : ScriptableObject
    {
        [SerializeField, Range(0, 0.5f)]
        private float seamLength;
        public float SeamLength => seamLength;

        [SerializeField]
        private bool useCurve;
        public bool UseCurve => useCurve;

        [SerializeField]
        private AnimationCurve curve;
        public AnimationCurve Curve => curve;

    }
}
