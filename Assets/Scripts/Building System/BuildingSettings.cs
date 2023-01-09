using UnityEngine;

namespace BuildingSystem
{
    [CreateAssetMenu]
    public class BuildingSettings : ScriptableObject
    {
        [field:SerializeField]
        public Material BlockedMaterial { get; private set; }
     
        [field:SerializeField]
        public Material ValidMaterial { get; private set; }
    }
}
