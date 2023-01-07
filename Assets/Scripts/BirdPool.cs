using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdPool : MonoBehaviour
{
    [SerializeField] private GameObject enemy;
    [SerializeField] private List<GameObject> pool = new List<GameObject>();

    [SerializeField] private float minRandomSpawnTimeValue;
    [SerializeField] private float maxRandomSpawnTimeValue;

    [SerializeField] private int poolSize = 5;

    private float spawnTimer;
    
    private void Awake()
    {
        PopulatePool();
    }
    
    private void Start()
    {
        SpawnBird();
        StartCoroutine("SpawnBird");
    }

    private void PopulatePool()
    {
        foreach(GameObject bird in pool)
        {
            pool.Add(Instantiate(enemy, transform));
            bird.SetActive(false);
        }
    }

    IEnumerator SpawnBird()
    {
        while (true)
        {
            EnableBirdInPool();
            SetRandomTimeInterval();
            yield return new WaitForSeconds(spawnTimer);
        }
    }

    private void EnableBirdInPool()
    {
        foreach (GameObject bird in pool)
        {
            if (bird.activeInHierarchy)
            {
                bird.SetActive(true);
                return;
            }
        }
    }    

    private void SetRandomTimeInterval()
    {
        spawnTimer = Random.Range(minRandomSpawnTimeValue, maxRandomSpawnTimeValue);
    }


}
