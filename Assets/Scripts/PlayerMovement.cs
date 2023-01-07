using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private FlatPosition flat;
    [SerializeField]
    private CharacterController characterController;

    [SerializeField]
    private Transform forwardProvider;

    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private Object inputProvider;
    public IMoveInputProvider InputProvider => (IMoveInputProvider)inputProvider;

    private void Update()
    {
        Vector3 forward = forwardProvider.forward;
        forward.y = 0;
        Vector3 right = forwardProvider.right;
        right.y = 0;

        Vector3 motion = forward * InputProvider.GetVertical() + right * InputProvider.GetHorizontal();
        characterController.Move(moveSpeed * Time.deltaTime * motion);

        //Vector2 flatMove = new Vector2(motion.x, motion.z);
        //flat.Position += moveSpeed * Time.deltaTime * flatMove;
    }


    private void LateUpdate()
    {
       // flat.UpdatePosition();
    }

    private void OnValidate()
    {
        inputProvider = InputProvider as Object;
    }
}