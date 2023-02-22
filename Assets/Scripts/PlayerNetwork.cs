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

    public delegate void HostMove();
    public event HostMove OnHostMove;
    public delegate void ClientMove();
    public event ClientMove OnClientMove;
    public delegate void HostDraw();
    public event HostDraw OnHostDraw;
    public delegate void ClientDraw();
    public event ClientDraw OnClientDraw;

    [HideInInspector]
    public bool IsDragging;

    private PlayerInput playerInput = null;
    private Camera playerCamera = null;
    private Transform draggingObject = null;
    private Vector3 startPosition = Vector3.zero;
    private Vector3 startScale = Vector3.zero;
    private Finger activeFinger = null;
    private bool moveIsValid;
    private NetworkVariable<bool> networkMoveIsValid;

    // Debug
    private InputAction moveAction;
    private InputAction toggleAction;
    private Vector2 inputVector;

    public enum Locations
    {
        Default,
        FarSouth1,
        FarSouth2,
        FarSouth3,
        MidSouth1,
        MidSouth2,
        MidSouth3,
        MidNorth1,
        MidNorth2,
        MidNorth3,
        FarNorth1,
        FarNorth2,
        FarNorth3,
        MidSide,
        SouthBase,
        NorthBase,
        HostHand,
        ClientHand,
    }

    public override void OnNetworkSpawn()
    {
        InitialConfigurations();
        GameStates.Instance.currentState.Value = GameStates.GameState.menu;
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneChanged;
        Touch.onFingerDown += FingerDown;
        Touch.onFingerUp += FingerUp;

        // Debug
        networkMoveIsValid.Value = true;
        toggleAction.started += ctx => TestCode();
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneChanged;
        Touch.onFingerDown -= FingerDown;
        Touch.onFingerUp -= FingerUp;

        // Debug
        toggleAction.started -= ctx => TestCode();
    }

    private void InitialConfigurations()
    {
        TouchSimulation.Enable();
        EnhancedTouchSupport.Enable();
        playerCamera = Camera.main;
        playerInput = GetComponent<PlayerInput>();

        // Debug
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
            draggingObject = checkHit.transform;
            startPosition = draggingObject.position;
            startScale = draggingObject.localScale;
            draggingObject.localScale = startScale * 2f;
            draggingObject.position = checkHit.point;
            IsDragging = true;
            StartCoroutine(DragUpdate(draggingObject));
        }
    }

    private IEnumerator DragUpdate(Transform obj)
    {
        while (activeFinger.isActive)
        {
            Ray touchRay = playerCamera.ScreenPointToRay(activeFinger.screenPosition);
            RaycastHit checkHit;
            if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, LayerMask.GetMask("TablePlane")))
            {
                obj.transform.position = checkHit.point;
                yield return new WaitForFixedUpdate();
            }
            else break;
        }
        StopCoroutine(DragUpdate(obj));
    }

    private void FingerUp(Finger finger)
    {
        if (!IsOwner) return;

        if (!IsDragging) return;
        if (IsHost && GameStates.Instance.currentState.Value != GameStates.GameState.host2) return;
        if (!IsHost && GameStates.Instance.currentState.Value != GameStates.GameState.client2) return;
        Ray touchRay = playerCamera.ScreenPointToRay(finger.screenPosition);
        RaycastHit checkHit;
        if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, LayerMask.GetMask("FarSouth1")))
        {
            // Check if the move is hand move or field move - compare tag
            // Check if the move is valid
            // Check if current state is host2 or client2


        }
        else if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, LayerMask.GetMask("FarSouth2")))
        {
            
        }
        else if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, LayerMask.GetMask("FarSouth3")))
        {
            
        }
        else if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, LayerMask.GetMask("MidSouth1")))
        {
            
        }
        else if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, LayerMask.GetMask("MidSouth2")))
        {
            
        }
        else if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, LayerMask.GetMask("MidSouth3")))
        {
            
        }
        else if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, LayerMask.GetMask("MidNorth1")))
        {
            
        }
        else if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, LayerMask.GetMask("MidNorth2")))
        {
            
        }
        else if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, LayerMask.GetMask("MidNorth3")))
        {
            
        }
        else if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, LayerMask.GetMask("FarNorth1")))
        {
            
        }
        else if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, LayerMask.GetMask("FarNorth2")))
        {
            
        }
        else if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, LayerMask.GetMask("FarNorth3")))
        {
            
        }
        else if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, LayerMask.GetMask("SouthBase")))
        {
            
        }
        else if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, LayerMask.GetMask("NorthBase")))
        {
            
        }
        else if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, LayerMask.GetMask("MidSide")))
        {
            
        }
        else if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, LayerMask.GetMask("HostHand")))
        {
            
        }
        else if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, LayerMask.GetMask("ClientHand")))
        {

        }
        else
        {
            draggingObject.position = startPosition;
            draggingObject.localScale = startScale;
        }

        draggingObject = null;
        IsDragging = false;
    }

    private void CheckHostMoveValidity(Locations to, Locations from, int cardId)
    {
        moveIsValid = true;
    }

    private void CheckClientMoveValidity(Locations to, Locations from, int cardId)
    {
        moveIsValid = true;
    }

    [ServerRpc]
    private void CheckHostMoveValidityServerRpc(Locations to, Locations from, int cardId)
    {
        networkMoveIsValid.Value = true;
    }

    [ServerRpc]
    private void CheckClientMoveValidityServerRpc(Locations to, Locations from, int cardId)
    {
        networkMoveIsValid.Value = true;
    }

    [ServerRpc]
    private void HostMoveServerRpc(Locations to, Locations from, int cardId)
    {
        // Do same checks here again

        Debug.Log("Detected " + to + " from the host.");
        if (OnHostMove != null) OnHostMove.Invoke();
    }

    [ServerRpc]
    private void ClientMoveServerRpc(Locations to, Locations from, int cardId)
    {
        // Do same checks here again
        
        Debug.Log("Detected " + to + " from the client.");
        if (OnClientMove != null) OnClientMove.Invoke();
    }

    private void OnSceneChanged(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if (!IsOwner) return;

        InitialConfigurations();
    }

    private void TestCode()
    {
        if (GameStates.Instance.currentState.Value == GameStates.GameState.menu)
        {
            GameStates.Instance.ChangeStateToInitialClientRpc();
        }
        else if (GameStates.Instance.currentState.Value == GameStates.GameState.initial)
        {
            GameStates.Instance.ChangeStateToStartClientRpc();
        }
        else if (GameStates.Instance.currentState.Value == GameStates.GameState.start)
        {
            GameStates.Instance.ChangeStateToHost1ClientRpc();
        }
        else if (GameStates.Instance.currentState.Value == GameStates.GameState.host1)
        {
            GameStates.Instance.ChangeStateToHost2ClientRpc();
        }
        else if (GameStates.Instance.currentState.Value == GameStates.GameState.host2)
        {
            GameStates.Instance.ChangeStateToClient1ClientRpc();
        }
        else if (GameStates.Instance.currentState.Value == GameStates.GameState.client1)
        {
            GameStates.Instance.ChangeStateToClient2ClientRpc();
        }
        else if (GameStates.Instance.currentState.Value == GameStates.GameState.client2)
        {
            GameStates.Instance.ChangeStateToHost1ClientRpc();
        }
    }
}
