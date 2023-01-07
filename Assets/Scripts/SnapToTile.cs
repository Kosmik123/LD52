using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToTile : MonoBehaviour
{
    [SerializeField]
    private Vector2 gridSize;
    [SerializeField]
    private Vector2Int tileSize;
    public Vector2Int size;

    [SerializeField, ReadOnly]
    private Vector2 gridPosition;

    private void LateUpdate()
    {
        Snap();
    }

    private void Snap()
    {
        Vector3 position = transform.position;
        transform.position = GetSnappedPosition(position.x, position.y, position.z);
    }

    public Vector3 GetSnappedPosition(float x, float y, float z)
    {
        bool isXEven = tileSize.x % 2 == 0;
        bool isYEven = tileSize.y % 2 == 0;

        gridPosition.x = isXEven ? Mathf.RoundToInt(x - 0.5f) + 0.5f : Mathf.RoundToInt(x);
        gridPosition.y = isYEven ? Mathf.RoundToInt(z - 0.5f) + 0.5f : Mathf.RoundToInt(z);

        return new Vector3(gridPosition.x, y, gridPosition.y);
    }

    private void OnValidate()
    {
        if (gridSize.x <= 0)
            gridSize.x = 0.0001f;
        if (gridSize.y <= 0)
            gridSize.y = 0.0001f;

        if (tileSize.x <= 0)
            tileSize.x = 1;
        if (tileSize.y <= 0)
            tileSize.y = 1;
    }
}
