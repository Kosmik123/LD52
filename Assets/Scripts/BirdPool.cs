using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdPool : MonoBehaviour
{
    [SerializeField] public List<GameObject> birds;
    [SerializeField] private GameObject origin;
    [SerializeField] private GameObject bird;
    [SerializeField] private int maxObjects = 5;

    private void Start()
    {
        birds = new List<GameObject>();
    }

    public GameObject SpawnBirds()
    {
        return birds.Count > 0 ? WithdrawElementOfList() : InstantiateBird();
    }

    private GameObject WithdrawElementOfList()
    {
        int firstElementOfList = 0;
        GameObject currentBird = birds[firstElementOfList];
        currentBird.transform.position = origin.transform.position;
        return currentBird;
    }

    private GameObject InstantiateBird()
    {
        GameObject currentBird = Instantiate(bird, origin.transform.position, Quaternion.identity);
        return currentBird;
    }    
}
