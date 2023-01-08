using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdSpawnController : MonoBehaviour
{
    [SerializeField] private BirdPool birdPool;
    [SerializeField] private GameObject bird;

    [SerializeField] private float minRandomSpawnTimeValue;
    [SerializeField] private float maxRandomSpawnTimeValue;

    private float spawnTimer;

    private void Start()
    {
        StartCoroutine("GetBirdFromPool",2f);
    }

    private void GetBirdFromPool()
    {
        SetRandomTimeInterval();
        birdPool.SpawnBirds();
    }

    private void SetPlaceToSpawnBird()
    {
        transform.position = birdPool.transform.position;
    }

    private float SetRandomTimeInterval()
    {
        spawnTimer = Random.Range(minRandomSpawnTimeValue, maxRandomSpawnTimeValue);
        return spawnTimer;
    }
}
