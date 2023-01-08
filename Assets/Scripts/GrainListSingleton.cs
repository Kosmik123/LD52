using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrainListSingleton : MonoBehaviour
{
    [SerializeField] private List<GameObject> grains = new List<GameObject>();
    public static GrainListSingleton Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }    
        else
        {
            Instance = this;
        }    
    }
}
