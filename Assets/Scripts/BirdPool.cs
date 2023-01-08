using System.Collections;
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

    [SerializeField] public List<Enemy> birds = new List<Enemy>();
    [SerializeField] private GameObject origin;
    [SerializeField] private int maxObjects = 5;
    
    public Enemy SpawnBirds()
    {
        return birds.Count > 0 ? WithdrawElementOfList() : InstantiateBird();
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
        Enemy currentBird = Instantiate(birdPrefab, origin.transform.position, Quaternion.identity);
        return currentBird;
    }    
}
