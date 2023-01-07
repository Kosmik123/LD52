using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.ChunkSystem.Generation.DirectionalPassage
{
    //[CreateAssetMenu(menuName = "Chunk System/Generation/Directional Wave Function Collapse Generation Strategy")]
    public class DirectionalWaveFunctionCollapseGenerationStrategy : MonoChunkProvider
    {
        private static Vector3Int[] directionsToCheck =
        {
            Vector3Int.down,
            Vector3Int.right,
            Vector3Int.up,
            Vector3Int.left,
            Vector3Int.forward,
            Vector3Int.back
        };

        [SerializeField]
        private ChunksData data;

        [SerializeField]
        private DirectionalPassageChunkData[] allPossibleChunks;
        public DirectionalPassageChunkData[] AllPossibleChunks => allPossibleChunks;

        public override Chunk GetChunk()
        {
            // ZMIANA BO ZMIENIŁEM IMPLEMENTACJE IChunkProvider. 
            // Tu trzeba coś innego wymyślić
            int x = 123, y = 456, z = 789;

            var potentialChunks = new List<DirectionalPassageChunkData>(AllPossibleChunks);
            for (int i = 0; i < directionsToCheck.Length; i++)
            {
                var neighbourIndex = new Vector3Int(
                    directionsToCheck[i].x + x,
                    directionsToCheck[i].y + y,
                    directionsToCheck[i].z + z);

                var neighbourChunk = data.GetChunk(neighbourIndex.x, neighbourIndex.y, neighbourIndex.z);
                if (neighbourChunk != null)
                {
                    if (neighbourChunk.TryGetComponent<DirectionalPassageChunkData>(out var neighbourCollapseData))
                    {
                        var checkedPassage = (DirectionalPassageChunkData.Passage)(1 << i);
                        var oppositePassage = DirectionalPassageChunkData.OppositePassage(checkedPassage);

                        bool shouldHaveOppositePassage = (neighbourCollapseData.passage & checkedPassage) > 0;
                        for (int j = potentialChunks.Count - 1; j >= 0; j--)
                        {
                            bool hasOppositePassage = (potentialChunks[j].passage & oppositePassage) > 0;
                            if (shouldHaveOppositePassage != hasOppositePassage)
                                potentialChunks.RemoveAt(j);
                        }
                    }
                }
            }
            int random = Random.Range(0, potentialChunks.Count);
            return potentialChunks[random].Chunk;
        }
    }
}
