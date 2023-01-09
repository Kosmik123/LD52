using UnityEngine;

namespace BuildingSystem
{
    public class Building : MonoBehaviour
    {
        [SerializeField]
        private BuildingVisual visual;

        public void Init (BuildingVisual visual)
        {
            this.visual = visual;
        }



    }
}
