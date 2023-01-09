using NaughtyAttributes;
using UnityEngine;

public class BirdSpawnController : MonoBehaviour
{
    [Header("To link")]
    [SerializeField] private Enemy[] birdPrefabs;
    [SerializeField, ReadOnly] private BirdPool[] birdPools;
    
    [Header("Spawning Settings")]
    [SerializeField] private Transform mapCenter;
    [SerializeField] private float spawnDistanceFromCenter = 100;

    [Header("Waves Time")]
    [SerializeField] private float minRandomSpawnTimeValue = 15;
    [SerializeField] private float maxRandomSpawnTimeValue = 25;

    [SerializeField, ReadOnly]
    private float spawnTimer;

    [Header("Difficulty")]
    [SerializeField] private float difficultyModifier = 0.1f;
    [SerializeField, ReadOnly]
    private float difficulty;

    private void Awake()
    {
        birdPools = new BirdPool[birdPrefabs.Length];
        for (int i = 0; i < birdPrefabs.Length; i++)
        {
            var pool = CreatePoolForBirdType(birdPrefabs[i]);
            pool.Origin = mapCenter;
            pool.SpawnDistance = spawnDistanceFromCenter;
            birdPools[i] = pool;
        }
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
        while (enemyPoints > 0)
        {
            int points = SpawnBird();
            enemyPoints -= points;
        }
    }

    private int SpawnBird()
    {
        int randomPoolIndex = Random.Range(0, birdPools.Length);
        var pool = birdPools[randomPoolIndex];
        var bird = pool.SpawnBird();
        return bird.Difficulty;
    }

    private float GetRandomTimeInterval()
    {
        float spawnTimer = Random.Range(minRandomSpawnTimeValue, maxRandomSpawnTimeValue);
        return spawnTimer;
    }
}
