using UnityEngine;

namespace Bipolar.ChunkSystem.Generation
{
    public class RandomChunkProvider : MonoChunkProvider
    {
        [SerializeField]
        private int seed;
        public int Seed
        {
            get => seed;
            set
            {
                if (value != seed)
                {
                    seed = value;
                    rng = new System.Random(value);
                }
            }
        }

        [SerializeField]
        private Chunk[] chunks;

        private System.Random rng;

        private void Awake()
        {
            rng = new System.Random(seed);
        }

        public override Chunk GetChunk()
        {
            if (rng == null)
                rng = new System.Random(seed);
            int random = rng.Next(chunks.Length);
            return chunks[random];
        }
    }
}
