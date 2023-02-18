using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField]
    private float playerSpeed = 10.0f;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction toggleAction;
    private Vector2 inputVector;

    public override void OnNetworkSpawn()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        toggleAction = playerInput.actions["Toggle"];
        if (IsOwner && IsClient && !IsHost) PlayerCamera.Instance.AdjustAngle();
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
}
