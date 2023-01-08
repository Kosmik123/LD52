using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdSpawnController : MonoBehaviour
{
    [Header("To link")]
    [SerializeField] private BirdPool[] birdPools;
    [SerializeField] private Enemy[] birdPrefabs;

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
    }

    private void Awake()
    {
        birdPools = new BirdPool[birdPrefabs.Length];
    }

    private BirdPool CreatePoolForBirdType(Enemy bird)
    {
        var poolGO = new GameObject($"Pool {bird.gameObject.name}");
        var pool = poolGO.AddComponent<BirdPool>();
        pool.BirdPrefab = bird;
        return pool;
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        difficulty += deltaTime * difficultyModifier;

        spawnTimer -= deltaTime;
        if (spawnTimer <= 0)
        {
            SpawnWave();
            spawnTimer = GetRandomTimeInterval(); // reset timer to random value
        }
    }

    private void SpawnWave()
    {
        int enemyPoints = Mathf.RoundToInt(difficulty);
    }

    private void SetPlaceToSpawnBird()
    {
    }

    private float GetRandomTimeInterval()
    {
        float spawnTimer = Random.Range(minRandomSpawnTimeValue, maxRandomSpawnTimeValue);
        return spawnTimer;
    }
}
