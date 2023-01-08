using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdSpawnController : MonoBehaviour
{
    [Header("To link")]
    [SerializeField] private BirdPool birdPool;
    [SerializeField] private GameObject bird;

    [Header("Waves Time")]
    [SerializeField] private float minRandomSpawnTimeValue;
    [SerializeField] private float maxRandomSpawnTimeValue;

    [SerializeField, ReadOnly]
    private float spawnTimer;

    [Header("Difficulty")]
    [SerializeField] private float difficultyModifier = 0.1f;
    [SerializeField, ReadOnly]
    private float difficulty;

    private IEnumerator GetBirdFromPool()
    {
        float time = GetRandomTimeInterval();
        yield return new WaitForSeconds(time);
        birdPool.SpawnBirds();
    }

    private void SetPlaceToSpawnBird()
    {
        transform.position = birdPool.transform.position;
    }

    private float GetRandomTimeInterval()
    {
        float spawnTimer = Random.Range(minRandomSpawnTimeValue, maxRandomSpawnTimeValue);
        return spawnTimer;
    }
}
