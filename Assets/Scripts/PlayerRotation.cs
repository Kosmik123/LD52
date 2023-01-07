using NaughtyAttributes;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    [SerializeField]
    private Camera viewCamera;

    [SerializeField]
    private Transform forwardProvider;

    private void Update()
    {
        Vector3 screenPosition = viewCamera.WorldToScreenPoint(transform.position);
        Vector3 relativeMousePosition = Input.mousePosition - screenPosition;
        relativeMousePosition.z = 0;

        Quaternion relativeRotation = Quaternion.FromToRotation( relativeMousePosition, Vector3.right);
        Vector3 relativeRotationEuler = relativeRotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0, forwardProvider.eulerAngles.y + relativeRotationEuler.z + 90, 0);
    }

}
