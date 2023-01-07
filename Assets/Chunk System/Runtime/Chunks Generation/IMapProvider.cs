using System;
using UnityEngine;

namespace Bipolar.ChunkSystem.Generation
{
    public interface IMapProvider : IMapProvider<float>
    {

    }

    public interface IMapProvider<T> 
    {
        T[,] GetMap(int width, int height, float xScale = 1, float yScale = 1, float xOffset = 0, float yOffset = 0);
        void GetMapNonAlloc(ref T[,] map, int width, int height, float xScale = 1, float yScale = 1, float xOffset = 0, float yOffset = 0);
    }

    public abstract class ScriptableMapProvider<T> : ScriptableObject, IMapProvider<T>, IEquatable<IMapProvider<T>>
    {
        public abstract bool Equals(IMapProvider<T> other);
        public abstract T[,] GetMap(int width, int height, float xScale = 1, float yScale = 1, float xOffset = 0, float yOffset = 0);
        public abstract void GetMapNonAlloc(ref T[,] map, int width, int height, float xScale = 1, float yScale = 1, float xOffset = 0, float yOffset = 0);
    }

    public interface IMap3DProvider<T>
    {
        T[,] GetMap3D(int width, int height, int depth, float xScale = 1, float yScale = 1, float zScale = 1, float xOffset = 0, float yOffset = 0, float zOffset = 0);
        void GetMap3DNonAlloc(ref T[,,] map, int width, int height, int depth, float xScale = 1, float yScale = 1, float zScale = 1, float xOffset = 0, float yOffset = 0, float zOffset = 0);
    }


}
