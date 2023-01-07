using UnityEngine;
// HOW TO IMPLEMENT A PERLIN NOISE FONCTION TO GET A TERRAIN HEIGHT IN UNITY
public static class NoiseCreator
{
    /// <summary>Perlin Noise applied multiple times</summary>
    /// <param name='x'>X position</param>
    /// <param name='z'>Z position</param>
    /// <param name='scale'>The scale of the "perlin noise" view</param>
    /// <param name='octaves'>Number of iterations (the more there is, the more detailed the terrain will be)</param>
    /// <param name='persistance'>The higher it is, the rougher the terrain will be (this value should be between 0 and 1 excluded)</param>
    /// <param name='lacunarity'>The higher it is, the more "feature" the terrain will have (should be strictly positive)</param>
    public static float GetNoiseAt(float x, float z, float scale, int octaves = 1, float persistance = 1, float lacunarity = 1)
    {
        float PerlinValue = 0f;
        float amplitude = scale;
        float frequency = 1f;

        for (int i = 0; i < octaves; i++)
        {
            // Get the perlin value at that octave and add it to the sum
            PerlinValue += Mathf.PerlinNoise(x * frequency, z * frequency) * amplitude;

            // Decrease the amplitude and the frequency
            amplitude *= persistance;
            frequency *= lacunarity;
        }

        // Return the noise value
        return PerlinValue;
    }
}