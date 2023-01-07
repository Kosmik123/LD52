namespace Bipolar.ChunkSystem.Generation
{
    public interface IMapModifier<T>
    {
        void ModifyMap(ref T[,] map);
    }

    public interface IMapModifier : IMapModifier<float>
    { }

}