using UnityEngine;

namespace Bipolar.ChunkSystem.Generation.DirectionalPassage
{
    [RequireComponent(typeof(Chunk))]
    public class DirectionalPassageChunkData : MonoBehaviour
    {
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

        [System.Flags]
        public enum Passage
        {
            Forward = 1 << 0, 
            Left = 1 << 1, 
            Back = 1 << 2,
            Right = 1 << 3
        }

        public Passage passage;

        public static Passage OppositePassage(Passage passage)
        {
            return passage switch
            {
                Passage.Forward => Passage.Back,
                Passage.Left => Passage.Right,
                Passage.Back => Passage.Forward,
                Passage.Right => Passage.Left,
                _ => (Passage)0
            };
        }

        private void Awake()
        {
            chunk = GetComponent<Chunk>();
        }
    }
}
