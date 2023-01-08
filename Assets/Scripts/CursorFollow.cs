using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorFollow : MonoBehaviour
{
    [SerializeField]
    private RaycastHitProvider hitProvider;
    [SerializeField]
    private SnapToTile snap;

    void Update()
    {
        transform.position = snap.GetSnappedPosition(hitProvider.Point);

    }
}
