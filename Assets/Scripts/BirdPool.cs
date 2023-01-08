using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class BirdPool : MonoBehaviour
{
    [SerializeField] private Enemy birdPrefab;
    public Enemy BirdPrefab
    {
        get => birdPrefab;
        set => birdPrefab = value; 
    }

    [SerializeField, ReadOnly] public List<Enemy> birds = new List<Enemy>();
    [SerializeField, ReadOnly] private Transform origin;
    public Transform Origin 
    { 
        get => origin;
        set => origin = value;
    }

    [SerializeField] private float spawnDistance;
    public float SpawnDistance { get => spawnDistance; set => spawnDistance = value; }

    
    public Enemy SpawnBird()
    {
        Enemy enemy = birds.Count > 0 ? WithdrawElementOfList() : InstantiateBird();
        enemy.gameObject.SetActive(true);
        return enemy;
    }

    private Enemy WithdrawElementOfList()
    {
        int firstElementOfList = 0;
        Enemy currentBird = birds[firstElementOfList];
        currentBird.transform.position = origin.transform.position;
        return currentBird;
    }

    private Enemy InstantiateBird()
    {
        float randomAngle = Random.Range(-180, 180);
        Vector3 randomPosition = Quaternion.AngleAxis(randomAngle, Vector3.up) * Vector3.forward * spawnDistance;
        Enemy currentBird = Instantiate(birdPrefab, origin.position + randomPosition, Quaternion.identity);
        return currentBird;
    }    
}
