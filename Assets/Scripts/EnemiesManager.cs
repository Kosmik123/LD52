using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    [SerializeField]
    private int maxEnemiesCount;

    [SerializeField, ReadOnly]
    private List<Enemy> enemies = new List<Enemy>();

    private readonly List<Enemy> enemiesToRemove = new List<Enemy>();
    public void AddEnemy(Enemy enemy)
    {
        enemies.Add(enemy);
    }

    private void Update()
    {
        float delta = Time.deltaTime;
        foreach (var enemy in enemies)
        {
            if (enemy != null)
                enemy.DoUpdate(delta);
            else
                enemiesToRemove.Add(enemy);       
        }

        if (enemiesToRemove.Count > 0)
        {
            foreach (var enemy in enemiesToRemove)
                enemies.Remove(enemy);

            enemiesToRemove.Clear();
        }
    }

    public bool IsMaxReached()
    {
        return enemies.Count > maxEnemiesCount;
    }


}
