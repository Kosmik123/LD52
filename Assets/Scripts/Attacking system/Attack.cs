using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] private HealthController healthController;
    [SerializeField] private int damage;
    //[SerializeField] pri
    public enum DetectionType {Raycast, Triger }
    private DetectionType detectionType;
    public DetectionType By => detectionType;

    private void Update()
    {
        EnemyDetection();
    }

    private void EnemyDetection()
    {
        switch (By)
        {
            case DetectionType.Raycast:
                RaycastDetect();
                break;
            case DetectionType.Triger:
                break;
            default:
                break;
        }
    }

    private void RaycastDetect()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.forward, out hit, 1f ))
        {
            healthController.ProcessHit(damage);
        }   
    }
}
