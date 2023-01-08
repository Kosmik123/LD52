using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdSpawnController : MonoBehaviour
{
    [SerializeField] private BirdPool birdPool;
    [SerializeField] private GameObject bird;

    [SerializeField] private float minRandomSpawnTimeValue;
    [SerializeField] private float maxRandomSpawnTimeValue;

    [SerializeField, ReadOnly]
    private float spawnTimer;

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
