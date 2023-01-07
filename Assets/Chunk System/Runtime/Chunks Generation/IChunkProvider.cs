namespace Bipolar.ChunkSystem.Generation
{
    public interface IChunkProvider
    {
        //void Init(ChunksData chunksData);
        Chunk GetChunk();
    }

    public interface IChunkInstanceProvider 
    {
        Chunk GetChunk(int x, int y, int z);
    }

}
