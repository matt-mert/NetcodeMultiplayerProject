using Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerNetwork : NetworkBehaviour
{
    public delegate void StartTouch(Ray ray, float time);
    public event StartTouch OnStartTouch;
    public delegate void DuringTouch(Ray ray, float time);
    public event DuringTouch OnDuringTouch;
    public delegate void EndTouch(Ray ray, float time);
    public event EndTouch OnEndTouch;

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
    private CinemachineVirtualCamera cmvc;

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

        if (!IsHost)
        {
            cmvc = FindObjectOfType<CinemachineVirtualCamera>();
            cmvc.transform.Rotate(new Vector3(0, 0, 180));
            transform.Rotate(new Vector3(0, 0, 180));
        }
    }

    private void TestCode()
    {
        if (GameStates.Instance.currentState == GameStates.GameState.menu)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
        }
        else if (GameStates.Instance.currentState == GameStates.GameState.initial)
        {
            GameStates.Instance.ChangeStateToStart();
        }
        else if (GameStates.Instance.currentState == GameStates.GameState.start)
        {
            GameStates.Instance.ChangeStateToInitial();
        }
    }

    private void StartTouchPrimary(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;

        if (OnStartTouch == null) return;
        Ray touchRay = playerCamera.ScreenPointToRay(primaryPositionVector);
        OnStartTouch(touchRay, (float)context.startTime);
    }

    private void DuringTouchPrimary(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;

        if (OnDuringTouch == null) return;
        Ray touchRay = playerCamera.ScreenPointToRay(primaryPositionVector);
        OnDuringTouch(touchRay, (float)context.time);
    }

    private void EndTouchPrimary(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;

        if (OnEndTouch == null) return;
        Ray touchRay = playerCamera.ScreenPointToRay(primaryPositionVector);
        OnEndTouch(touchRay, (float)context.startTime);
    }
}
