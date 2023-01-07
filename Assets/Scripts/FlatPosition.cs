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

    private void UpdatePosition()
    {
        Vector3 rayStart = new Vector3(flatPosition.x, transform.position.y + 0.5f, flatPosition.y);
        if (Physics.Raycast(rayStart, Vector3.down, out var hitInfo, 1, groundLayers))
        {
            transform.position = hitInfo.point;
        }
        flatPosition = new Vector2(transform.position.x, transform.position.z);
    }
}
