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

    public delegate void PlayerMove(PlayerMoves move);
    public event PlayerMove OnPlayerMove;

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

    public enum PlayerMoves
    {
        NoMove,
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
        NorthBase
    }

    private PlayerMoves currentMove = PlayerMoves.NoMove;

    public override void OnNetworkSpawn()
    {
        InitialConfigurations();
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneChanged;
        Touch.onFingerDown += FingerDown;
        Touch.onFingerUp += FingerUp;

        toggleAction.started += ctx => TestCode();
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneChanged;
        Touch.onFingerDown -= FingerDown;
        Touch.onFingerUp -= FingerUp;

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

    private void FingerUp(Finger finger)
    {
        activeFinger = finger;
        Ray touchRay = playerCamera.ScreenPointToRay(activeFinger.screenPosition);
        RaycastHit checkHit;
        if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, LayerMask.GetMask("FarSouth1")))
        {
            currentMove = PlayerMoves.FarSouth1;
            OnPlayerMove.Invoke(currentMove);
        }
        else if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, LayerMask.GetMask("FarSouth2")))
        {
            currentMove = PlayerMoves.FarSouth2;
            OnPlayerMove.Invoke(currentMove);
        }
        else if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, LayerMask.GetMask("FarSouth3")))
        {
            currentMove = PlayerMoves.FarSouth3;
            OnPlayerMove.Invoke(currentMove);
        }
        else if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, LayerMask.GetMask("MidSouth1")))
        {
            currentMove = PlayerMoves.MidSouth1;
            OnPlayerMove.Invoke(currentMove);
        }
        else if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, LayerMask.GetMask("MidSouth2")))
        {
            currentMove = PlayerMoves.MidSouth2;
            OnPlayerMove.Invoke(currentMove);
        }
        else if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, LayerMask.GetMask("MidSouth3")))
        {
            currentMove = PlayerMoves.MidSouth3;
            OnPlayerMove.Invoke(currentMove);
        }
        else if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, LayerMask.GetMask("MidNorth1")))
        {
            currentMove = PlayerMoves.MidNorth1;
            OnPlayerMove.Invoke(currentMove);
        }
        else if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, LayerMask.GetMask("MidNorth2")))
        {
            currentMove = PlayerMoves.MidNorth2;
            OnPlayerMove.Invoke(currentMove);
        }
        else if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, LayerMask.GetMask("MidNorth3")))
        {
            currentMove = PlayerMoves.MidNorth3;
            OnPlayerMove.Invoke(currentMove);
        }
        else if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, LayerMask.GetMask("FarNorth1")))
        {
            currentMove = PlayerMoves.FarNorth1;
            OnPlayerMove.Invoke(currentMove);
        }
        else if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, LayerMask.GetMask("FarNorth2")))
        {
            currentMove = PlayerMoves.FarNorth1;
            OnPlayerMove.Invoke(currentMove);
        }
        else if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, LayerMask.GetMask("FarNorth3")))
        {
            currentMove = PlayerMoves.FarNorth3;
            OnPlayerMove.Invoke(currentMove);
        }
        else if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, LayerMask.GetMask("SouthBase")))
        {
            currentMove = PlayerMoves.SouthBase;
            OnPlayerMove.Invoke(currentMove);
        }
        else if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, LayerMask.GetMask("NorthBase")))
        {
            currentMove = PlayerMoves.NorthBase;
            OnPlayerMove.Invoke(currentMove);
        }
        else if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, LayerMask.GetMask("MidSide")))
        {
            currentMove = PlayerMoves.MidSide;
            OnPlayerMove.Invoke(currentMove);
        }
        else
        {
            currentMove = PlayerMoves.NoMove;
        }
    }

    [ServerRpc]
    private void HostMoveCardRequestServerRpc()
    {

    }

    [ServerRpc]
    private void ClientMoveCardRequestServerRpc()
    {

    }

    private void OnSceneChanged(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if (!IsOwner) return;

        InitialConfigurations();
    }

    private void TestCode()
    {
        if (GameStates.Instance.currentState == GameStates.GameState.menu)
        {
            GameStates.Instance.ChangeStateToInitialClientRpc();
        }
        else if (GameStates.Instance.currentState == GameStates.GameState.initial)
        {
            GameStates.Instance.ChangeStateToStartClientRpc();
        }
        else if (GameStates.Instance.currentState == GameStates.GameState.start)
        {
            GameStates.Instance.ChangeStateToHost1ClientRpc();
        }
        else if (GameStates.Instance.currentState == GameStates.GameState.host1)
        {
            GameStates.Instance.ChangeStateToHost2ClientRpc();
        }
        else if (GameStates.Instance.currentState == GameStates.GameState.host2)
        {
            GameStates.Instance.ChangeStateToClient1ClientRpc();
        }
        else if (GameStates.Instance.currentState == GameStates.GameState.client1)
        {
            GameStates.Instance.ChangeStateToClient2ClientRpc();
        }
        else if (GameStates.Instance.currentState == GameStates.GameState.client2)
        {
            GameStates.Instance.ChangeStateToHost1ClientRpc();
        }
    }
}
