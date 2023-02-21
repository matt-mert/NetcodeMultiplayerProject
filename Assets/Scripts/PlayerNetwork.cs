using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.SceneManagement;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField]
    private float playerSpeed = 10.0f;

    private PlayerInput playerInput;

    private Camera playerCamera;
    private Vector3 startPosition;
    private Vector3 startScale;
    private Finger activeFinger;

    [HideInInspector]
    public bool IsDragging;

    private InputAction moveAction;
    private InputAction toggleAction;
    private Vector2 inputVector;

    public override void OnNetworkSpawn()
    {
        InitialConfigurations();
        SceneManager.activeSceneChanged += OnSceneChanged;
        Touch.onFingerDown += FingerDown;

        toggleAction.started += ctx => TestCode();
    }

    public override void OnNetworkDespawn()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;

        toggleAction.started -= ctx => TestCode();
    }

    private void InitialConfigurations()
    {
        TouchSimulation.Enable();
        EnhancedTouchSupport.Enable();
        playerCamera = Camera.main;
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        toggleAction = playerInput.actions["Toggle"];
    }

    private void OnConnectedToServer()
    {
        if (!IsOwner) return;
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

    private void FingerDown(Finger finger)
    {
        activeFinger = finger;
        Ray touchRay = playerCamera.ScreenPointToRay(activeFinger.screenPosition);
        RaycastHit checkHit;
        if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, LayerMask.GetMask("PlayerCard")))
        {
            GameObject objectToDrag = checkHit.transform.gameObject;
            startPosition = objectToDrag.transform.position;
            startScale = objectToDrag.transform.localScale;
            objectToDrag.transform.localScale = startScale * 2f;
            objectToDrag.transform.position = checkHit.point;
            IsDragging = true;
            StartCoroutine(DragUpdate(objectToDrag));
        }
    }

    private IEnumerator DragUpdate(GameObject draggingObject)
    {
        while (activeFinger.isActive)
        {
            Ray touchRay = playerCamera.ScreenPointToRay(activeFinger.screenPosition);
            RaycastHit checkHit;
            if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, LayerMask.GetMask("TablePlane")))
            {
                draggingObject.transform.position = checkHit.point;
                yield return new WaitForFixedUpdate();
            }
            else break;
        }
        draggingObject.transform.position = startPosition;
        draggingObject.transform.localScale = startScale;
        IsDragging = false;
        StopCoroutine(DragUpdate(draggingObject));
    }

    private void OnSceneChanged(Scene current, Scene next)
    {
        if (!IsOwner) return;

        InitialConfigurations();
    }

    private void TestCode()
    {
        GameStates.Instance.ChangeStateToInitialClientRpc();
    }
}
