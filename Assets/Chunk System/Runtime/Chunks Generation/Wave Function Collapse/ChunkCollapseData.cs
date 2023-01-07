using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.ChunkSystem.Generation.WaveFunctionCollapse
{
    [RequireComponent(typeof(Chunk))]
    public class ChunkCollapseData : MonoBehaviour
    {
        public event System.Action<ChunkCollapseData, int> OnCollapseDataCreated;
        public event System.Action<ChunkCollapseData, int> OnCollapseDataChanged;

        private Chunk chunk;
        public Chunk Chunk
        {
            get
            {
                if (chunk == null)
                    chunk = GetComponent<Chunk>();
                return chunk;
            }
        }

        private void Awake()
        {
            chunk = GetComponent<Chunk>();
        }


    }

    public class WaveCollapseFunctionSettings : ScriptableObject
    {
        [SerializeField]
        private List<string> patternNames = new List<string>();
        public List<string> PatternNames => patternNames;
    }




}
