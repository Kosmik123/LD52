using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private FlatPosition flat;

    [SerializeField]
    private Transform forwardProvider;

    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private Object inputProvider;
    public IMoveInputProvider InputProvider => (IMoveInputProvider)inputProvider;

    private void Update()
    {
        Vector3 move = forwardProvider.forward * InputProvider.GetVertical() + forwardProvider.right * InputProvider.GetHorizontal();
        Vector2 flatMove = new Vector2(move.x, move.z);
        flat.Position += moveSpeed * Time.deltaTime * flatMove;
    }

    private void OnValidate()
    {
        inputProvider = InputProvider as Object;
    }
}