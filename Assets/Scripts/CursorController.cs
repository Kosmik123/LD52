using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum InteractionMode
{
    None = 0,
    Build = 1,
    Destroy = 2,
}


public class CursorController : MonoBehaviour
{
    [SerializeField]
    private CursorSettings settings;

    [Header("To Link")]
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private SnapToTile snapToTile;

    [Header("States")]
    [SerializeField, ReadOnly]
    private int angle;
    public int Angle => angle;

    public Vector2 Position => new Vector2(transform.position.x, transform.position.z);

    [SerializeField]
    private Vector2Int size;
    public Vector2Int Size
    {
        get => size;
        set
        {
            size = value;
            spriteRenderer.size = snapToTile.TileSize = angle % 180 == 0 ? size : new Vector2Int(size.y, size.x);
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
            Size = Size;
        }
    }

    private void OnValidate()
    {
        Size = Size;
    }

    public void SetMode(InteractionMode mode)
    {
        Texture2D icon = settings.DefaultCursorIcon;
        switch (mode)
        {
            case InteractionMode.Build:
                spriteRenderer.color = Color.white;
                icon = settings.BuildCursorIcon;
                break;
            case InteractionMode.Destroy:
                spriteRenderer.color = Color.red;
                icon = settings.DestroyCursorIcon;
                break;
            default:
                spriteRenderer.color = Color.clear;
                break;
        }
        //Cursor.SetCursor(icon, Vector2.zero, CursorMode.Auto);

    }
}
