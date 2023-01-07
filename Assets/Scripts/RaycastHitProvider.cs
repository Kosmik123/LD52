using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastHitProvider : MonoBehaviour
{
    private RaycastHit hit;
    public RaycastHit Hit => hit;

    private bool isHit;
    public bool IsHit => isHit;

    [SerializeField]
    private new Camera camera;
    [SerializeField]
    private float rayLength;
    [SerializeField]
    private LayerMask detectedLayers;

    [SerializeField, ReadOnly]
    private Vector3 hitPosition;
    public Vector3 Point => hitPosition;

    private Ray ray;

    private void Update()
    {
        ray = camera.ScreenPointToRay(Input.mousePosition);
        isHit = Physics.Raycast(ray, out hit, rayLength, detectedLayers);
        hitPosition = hit.point;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(ray.origin, ray.direction * rayLength);
    }
}
