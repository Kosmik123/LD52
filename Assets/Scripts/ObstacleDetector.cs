using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDetector : MonoBehaviour
{
    private void FixedUpdate()
    {
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        LayerMask layerMask = 11;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.forward, out hit))
            print(gameObject.name +" collide: " + hit.collider.gameObject.name);
    }
}
