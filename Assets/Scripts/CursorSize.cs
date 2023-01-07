using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorSize : MonoBehaviour
{
    [Header("To Link")]
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private SnapToTile snapToTile;

    [Header("States")]
    [SerializeField]
    private Vector2Int size;
    public Vector2Int Size
    {
        get => size;
        set
        {
            size = value;
            spriteRenderer.size = size;
            snapToTile.TileSize = size;
        }
    }

    private void OnValidate()
    {
        Size = Size;
    }
}
