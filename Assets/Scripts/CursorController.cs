using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    [Header("To Link")]
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private SnapToTile snapToTile;

    [Header("States")]
    [SerializeField, ReadOnly]
    private float angle;
    public float Angle => angle;

    public Vector2 Position => new Vector2(transform.position.x, transform.position.z);

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

    [SerializeField]
    private Transform container;
    public Transform Container => container;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            angle += 90;
            angle %= 360;
            container.rotation = Quaternion.AngleAxis(angle, Vector3.up);
            int oldX = Size.x;
            int oldY = Size.y;
            Size = new Vector2Int(oldY, oldX);
        }
    }

    private void OnValidate()
    {
        Size = Size;
    }

    public void SetColor(Color color)
    {
        spriteRenderer.color = color;
    }
}
