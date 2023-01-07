using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    [SerializeField]
    private Vector3 offset;

    [SerializeField]
    private float distance;

    [SerializeField]
    private Vector2 angles;

    private void LateUpdate()
    {
        UpdatePositionAndRotation();
    }

    private void UpdatePositionAndRotation()
    {
        float radianAngle = angles.y * Mathf.Deg2Rad;
        Vector3 cameraOffset = new Vector3(0, Mathf.Sin(radianAngle), -Mathf.Cos(radianAngle)) * distance;
        Vector3 relativePosition = Quaternion.AngleAxis(angles.x, Vector3.up) * cameraOffset;
        transform.position = target.position + offset + relativePosition;
        transform.rotation = Quaternion.Euler(angles.y, angles.x, 0);
    }

    private void OnValidate()
    {
        if (enabled)
        {
            angles.y = Mathf.Clamp(angles.y, -90f, 90f);
            UpdatePositionAndRotation();
        }
    }

}
