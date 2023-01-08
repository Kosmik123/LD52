using NaughtyAttributes;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("To Link")]
    [SerializeField]
    private CharacterController characterController;

    [SerializeField]
    private Object inputProvider;
    public IMoveInputProvider InputProvider => (IMoveInputProvider)inputProvider;
    [SerializeField]
    private Transform forwardProvider;

    [Header("Settings")]
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float gravityScale;
    
    [Header("States")]
    [SerializeField, ReadOnly]
    private Vector3 velocity;
    [SerializeField, ReadOnly]
    private CollisionFlags collisionFlags;

    private void Update()
    {
        Vector3 forward = forwardProvider.forward;
        forward.y = 0;
        Vector3 right = forwardProvider.right;
        right.y = 0;

        Vector3 motion = forward * InputProvider.GetVertical() + right * InputProvider.GetHorizontal();

        velocity = moveSpeed * Time.deltaTime * motion;
        ApplyGravity();
        collisionFlags = characterController.Move(velocity);
    }

    private void ApplyGravity()
    {
        if (characterController.isGrounded && velocity.y <= 0)
            velocity.y = 0.5f * gravityScale * Physics.gravity.y;
        else
            velocity += gravityScale * Time.deltaTime * Physics.gravity;
    }

    private void OnValidate()
    {
        inputProvider = InputProvider as Object;
    }
}