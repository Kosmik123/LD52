using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlatPosition : MonoBehaviour
{
    [SerializeField]
    private Vector2 flatPosition;
    public Vector2 Position
    {
        get => flatPosition;
        set
        {
            flatPosition = value;
            UpdatePosition();
        }
    }

    [SerializeField]
    private LayerMask groundLayers = 1 << 8;

    public void UpdatePosition()
    {
        Vector3 rayStart = transform.position + new Vector3(0, 0.5f);
        if (Physics.Raycast(rayStart, Vector3.down, out var hitInfo, 1, groundLayers))
            transform.position = hitInfo.point;
    }
}
