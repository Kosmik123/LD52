using UnityEngine;

namespace Bipolar.ChunkSystem
{
    [System.Serializable]
    public enum HeightmapResolutionEnum
    {
        [InspectorName("33 x 33")]
        _33 = 33,
        [InspectorName("65 x 65")]
        _65 = 65,
        [InspectorName("129 x 129")]
        _129 = 129,
        [InspectorName("257 x 257")]
        _257 = 257,
        [InspectorName("513 x 513")]
        _513 = 513,
        [InspectorName("1025 x 1025")]
        _1025 = 1025,
        [InspectorName("2049 x 2049")]
        _2049 = 2049,
        [InspectorName("4097 x 4097")]
        _4097 = 4097
    }

    [System.Serializable]
    public enum TextureResolutionEnum
    {
        [InspectorName("32 x 32")]
        _32 = 32,
        [InspectorName("64 x 64")]
        _64 = 64,
        [InspectorName("128 x 128")]
        _128 = 128,
        [InspectorName("256 x 256")]
        _256 = 256,
        [InspectorName("512 x 512")]
        _512 = 512,
        [InspectorName("1024 x 1024")]
        _1024 = 1024,
        [InspectorName("2048 x 2048")]
        _2048 = 2048,
        [InspectorName("4096 x 4096")]
        _4096 = 4096
    }

    [System.Serializable]
    public enum SquareChunkResolution
    {
        [InspectorName("2 x 2")]
        _2 = 2,
        [InspectorName("4 x 4")]
        _4 = 4,
        [InspectorName("8 x 8")]
        _8 = 8,
        [InspectorName("16 x 16")]
        _16 = 16,
        [InspectorName("32 x 32")]
        _32 = 32,
        [InspectorName("64 x 64")]
        _64 = 64,
        [InspectorName("128 x 128")]
        _128 = 128,
        [InspectorName("256 x 256")]
        _256 = 256,
    }


}