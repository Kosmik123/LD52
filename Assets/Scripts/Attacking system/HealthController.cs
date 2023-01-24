using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [SerializeField] private int maxHP;
    [SerializeField] private int currentHP;

    private void OnEnable()
    {
        currentHP = maxHP;
    }

    private void OnParticleCollision(GameObject other)
    {
        ;
    }

    public void ProcessHit(int dmg)
    {
        Debug.Log("ProcessHit");
        DecreaseHP(dmg);
        if (!CheckIfObjectIsAlive())
        {
            KillObject();
        }
    }

    private bool CheckIfObjectIsAlive()
    {
        if (currentHP <= 0)
            return false;
        return true;
    }

    private void KillObject()
    {
        Destroy(gameObject);
    }

    private void DecreaseHP(int dmg)
    {
        Debug.Log("ProcessHit");

        currentHP -= dmg;
    }
}
