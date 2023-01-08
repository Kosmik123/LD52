using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassRandomizer : MonoBehaviour
{
    [SerializeField]
    private Bounds bounds;

    [SerializeField]
    private Transform[] randomizedObjects;

    private void Start()
    {
        Randomize();
    }

    [ContextMenu("Randomize")]
    public void Randomize()
    {
        foreach (var obj in randomizedObjects)
        {
            float x = Random.Range(bounds.min.x, bounds.max.x);
            float y = Random.Range(bounds.min.y, bounds.max.y);
            float z = Random.Range(bounds.min.z, bounds.max.z);

            obj.position = transform.position + bounds.center + new Vector3(x, y, z);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position + bounds.center, bounds.size);
    }
}
