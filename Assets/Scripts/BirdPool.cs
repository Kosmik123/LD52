using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BirdPool : MonoBehaviour
{
    [SerializeField] public List<GameObject> birds;
    [SerializeField] private GameObject origin;
    [SerializeField] private GameObject bird;
    [SerializeField] private int maxObjects = 5;

    private void Start()
    {
        birds = new List<GameObject>();
    }

    public void SpawnBirds()
    {
        if(birds.Count > 0)
        {
            withdrawElementOfList();
        }
        else
        {
            InstantiateBird();
        }
    }

    private GameObject withdrawElementOfList()
    {
        int firstElementOFList = 0;
        GameObject currentBird = birds[firstElementOFList];
        currentBird.transform.position = origin.transform.position;
        return currentBird;
    }

    private GameObject InstantiateBird()
    {
        GameObject currentBird = Instantiate(bird, origin.transform.position, Quaternion.identity);
        currentBird.transform.position = origin.transform.position;
        return currentBird;
    }    
}