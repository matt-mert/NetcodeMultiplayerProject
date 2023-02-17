using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField]
    private float playerSpeed = 10.0f;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private Vector2 inputVector;
    private Vector2 testVector;

    private void Awake()
    {
        if (!IsOwner) return;
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
    }

    private void OnEnable()
    {
        if (!IsOwner) return;
        moveAction.performed += GetInput;
    }

    private void OnDisable()
    {
        if (!IsOwner) return;
        moveAction.performed -= GetInput;
    }

    private void Update()
    {
        if (!IsOwner) return;
        inputVector = moveAction.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        Vector3 moveVector = new Vector3(inputVector.x, 0, inputVector.y);
        transform.Translate(moveVector * playerSpeed * Time.fixedDeltaTime);
    }

    private void GetInput(InputAction.CallbackContext ctx)
    {
        if (!IsOwner) return;
        testVector = ctx.ReadValue<Vector2>();
    }
}
