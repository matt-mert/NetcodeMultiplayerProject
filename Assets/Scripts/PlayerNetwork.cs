using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField]
    private float playerSpeed = 10.0f;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction toggleAction;
    private InputAction primaryPosition;
    private InputAction primaryContact;
    private Vector2 inputVector;
    private Vector2 primaryPositionVector;
    private Camera playerCamera;
    private GameObject draggedObject;
    private Vector3 startPosition;
    private Vector3 startScale;
    private float startTime;
    private float endTime;

    [HideInInspector]
    public bool IsDragging;

    public override void OnNetworkSpawn()
    {
        InitialConfigurations();
        primaryContact.started += ctx => StartTouchPrimary(ctx);
        primaryPosition.performed += ctx => DuringTouchPrimary(ctx);
        primaryContact.canceled += ctx => EndTouchPrimary(ctx);
        SceneManager.activeSceneChanged += OnSceneChanged;
        NetworkManager.Singleton.OnClientConnectedCallback += ctx => GameStates.Instance.OnClientConnect();
        NetworkManager.Singleton.OnClientDisconnectCallback += ctx => GameStates.Instance.OnClientDisconnect();

        toggleAction.started += ctx => TestCode();
    }

    public override void OnNetworkDespawn()
    {
        primaryContact.started -= ctx => StartTouchPrimary(ctx);
        primaryPosition.performed -= ctx => DuringTouchPrimary(ctx);
        primaryContact.canceled -= ctx => EndTouchPrimary(ctx);
        SceneManager.activeSceneChanged -= OnSceneChanged;
        NetworkManager.Singleton.OnClientConnectedCallback -= ctx => GameStates.Instance.OnClientConnect();
        NetworkManager.Singleton.OnClientDisconnectCallback -= ctx => GameStates.Instance.OnClientDisconnect();

        toggleAction.started -= ctx => TestCode();
    }

    private void InitialConfigurations()
    {
        playerCamera = Camera.main;
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        toggleAction = playerInput.actions["Toggle"];
        primaryContact = playerInput.actions["PrimaryContact"];
        primaryPosition = playerInput.actions["PrimaryPosition"];
    }

    private void OnConnectedToServer()
    {
        if (!IsOwner) return;
    }

    private void Update()
    {
        if (!IsOwner) return;

        inputVector = moveAction.ReadValue<Vector2>();
        primaryPositionVector = primaryPosition.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        Vector3 moveVector = new Vector3(inputVector.x, 0, inputVector.y);
        transform.Translate(moveVector * playerSpeed * Time.fixedDeltaTime);
    }

    private void OnSceneChanged(Scene current, Scene next)
    {
        // Game state client tarafinda degismiyor.

        if (!IsOwner) return;

        InitialConfigurations();
        GameStates.Instance.ChangeStateToInitial();
    }

    private void ChangeSceneToGame()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }

    private void TestCode()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }

    private void StartTouchPrimary(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;

        //if (OnStartTouch == null) return;
        Ray touchRay = playerCamera.ScreenPointToRay(primaryPositionVector);
        //OnStartTouch(touchRay, (float)context.startTime);
        
        RaycastHit checkHit;
        if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, LayerMask.GetMask("PlayerCard")))
        {
            draggedObject = checkHit.transform.gameObject;
            startPosition = draggedObject.transform.position;
            startScale = draggedObject.transform.localScale;
            startTime = (float)context.time;
            draggedObject.transform.localScale = startScale * 2f;
            draggedObject.transform.position = checkHit.point;
            IsDragging = true;
        }

    }

    private void DuringTouchPrimary(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;

        if (draggedObject == null) return;

        //if (OnDuringTouch == null) return;
        Ray touchRay = playerCamera.ScreenPointToRay(primaryPositionVector);
        //OnDuringTouch(touchRay, (float)context.time);

        RaycastHit checkHit;
        if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, LayerMask.GetMask("TablePlane")))
        {
            draggedObject.transform.position = checkHit.point;
        }
        else
        {
            endTime = (float)context.time;
            draggedObject.transform.position = startPosition;
            draggedObject.transform.localScale = startScale;
            draggedObject = null;
            IsDragging = false;
        }
    }

    private void EndTouchPrimary(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;

        if (draggedObject == null) return;

        //if (OnEndTouch == null) return;
        Ray touchRay = playerCamera.ScreenPointToRay(primaryPositionVector);
        //OnEndTouch(touchRay, (float)context.startTime);

        endTime = (float)context.time;
        draggedObject.transform.position = startPosition;
        draggedObject.transform.localScale = startScale;
        draggedObject = null;
        IsDragging = false;
    }
}
