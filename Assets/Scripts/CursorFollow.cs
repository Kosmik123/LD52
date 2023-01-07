using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorFollow : MonoBehaviour
{
    [SerializeField]
    private RaycastHitProvider hitProvider;

    void Update()
    {
        transform.position = hitProvider.Point;
    }
}
