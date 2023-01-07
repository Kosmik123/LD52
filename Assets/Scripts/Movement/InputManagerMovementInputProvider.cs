using UnityEngine;
using NaughtyAttributes;

public class InputManagerMovementInputProvider : MonoBehaviour, IMoveInputProvider
{
    [SerializeField, InputAxis]
    private string horizontalAxis = "Horizontal";
    [SerializeField, InputAxis]
    private string verticalAxis = "Vertical";

    [SerializeField]
    private KeyCode dashKey = KeyCode.LeftShift;
    [SerializeField]
    private KeyCode crouchKey = KeyCode.LeftControl;

    public bool GetCrouching()
    {
        return Input.GetKey(crouchKey);
    }

    public bool GetDashing()
    {
        return Input.GetKey(dashKey);
    }

    public float GetHorizontal()
    {
        return Input.GetAxis(horizontalAxis);
    }

    public float GetVertical()
    {
        return Input.GetAxis(verticalAxis);
    }
}
