using Newtonsoft.Json.Bson;
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

    [HideInInspector]
    public bool IsDragging;

    private PlayerInput playerInput = null;
    private Camera playerCamera = null;
    private Transform draggingObject = null;
    private Vector3 startPosition = Vector3.zero;
    private Vector3 startScale = Vector3.zero;
    private Finger activeFinger = null;

    private CardManager.CardLocation fromLocation;
    private CardManager.CardLocation toLocation;

    // Debug

    private InputAction moveAction;
    private InputAction toggleAction;
    private InputAction testAction;
    private Vector2 inputVector;

    public override void OnNetworkSpawn()
    {
        TouchSimulation.Enable();
        EnhancedTouchSupport.Enable();
        playerCamera = Camera.main;
        playerInput = GetComponent<PlayerInput>();

        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneChanged;
        Touch.onFingerDown += FingerDown;
        Touch.onFingerMove += FingerMove;
        Touch.onFingerUp += FingerUp;

        // Debug
        moveAction = playerInput.actions["Move"];
        toggleAction = playerInput.actions["Toggle"];
        testAction = playerInput.actions["Test"];
        toggleAction.started += ctx => ToggleCode();
        testAction.started += ctx => TestCode();
    }
    
    private void OnSceneChanged(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if (!IsOwner) return;

        TouchSimulation.Enable();
        EnhancedTouchSupport.Enable();
        playerCamera = Camera.main;
        playerInput = GetComponent<PlayerInput>();

        // Debug
        moveAction = playerInput.actions["Move"];
        toggleAction = playerInput.actions["Toggle"];
        testAction = playerInput.actions["Test"];
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
        if (!IsOwner) return;

        if (draggingObject != null || IsDragging) return;

        activeFinger = finger;
        if (activeFinger.index > 0) return;

        Ray touchRay = playerCamera.ScreenPointToRay(activeFinger.screenPosition);
        RaycastHit checkHit1;
        RaycastHit checkHit2;
        if (Physics.Raycast(touchRay, out checkHit1, Mathf.Infinity, LayerMask.GetMask("PlayerCard")))
        {
            if (IsHost) fromLocation = CardManager.CardLocation.HostHand;
            else fromLocation = CardManager.CardLocation.ClientHand;

            draggingObject = checkHit1.transform;
            startPosition = draggingObject.position;
            startScale = draggingObject.localScale;
            draggingObject.localScale = startScale * 2f;
            draggingObject.position = checkHit1.point;
            IsDragging = true;
        }
        else if (Physics.Raycast(touchRay, out checkHit1, Mathf.Infinity, LayerMask.GetMask("FieldCard")))
        {
            if (Physics.Raycast(touchRay, out checkHit2, Mathf.Infinity, LayerMask.GetMask("NetworkHostCard")) && IsHost)
            {
                fromLocation = checkHit1.transform.GetComponent<FieldLocation>().location;

                draggingObject = checkHit2.transform;
                startPosition = draggingObject.position;
                startScale = draggingObject.localScale;
                draggingObject.localScale = startScale * 2f;
                draggingObject.position = checkHit2.point;
                IsDragging = true;
            }
            else if (Physics.Raycast(touchRay, out checkHit2, Mathf.Infinity, LayerMask.GetMask("NetworkClientCard")) && !IsHost)
            {
                fromLocation = checkHit1.transform.GetComponent<FieldLocation>().location;

                draggingObject = checkHit2.transform;
                startPosition = draggingObject.position;
                startScale = draggingObject.localScale;
                draggingObject.localScale = startScale * 2f;
                draggingObject.position = checkHit2.point;
                IsDragging = true;
            }
        }
    }

    private void FingerMove(Finger finger)
    {
        if (!IsOwner) return;

        if (draggingObject == null || !IsDragging) return;

        activeFinger = finger;
        if (activeFinger.index > 0) return;

        Ray touchRay = playerCamera.ScreenPointToRay(activeFinger.screenPosition);
        RaycastHit checkHit;
        if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, LayerMask.GetMask("TablePlane")))
        {
            draggingObject.transform.position = checkHit.point;
        }
        else
        {
            fromLocation = CardManager.CardLocation.Default;
            toLocation = CardManager.CardLocation.Default;

            draggingObject.position = startPosition;
            draggingObject.localScale = startScale;
            draggingObject = null;
            IsDragging = false;
            return;
        }
    }

    private void FingerUp(Finger finger)
    {
        if (!IsOwner) return;

        if (draggingObject == null || !IsDragging) return;

        activeFinger = finger;
        if (activeFinger.index > 0) return;

        if (IsHost && GameStates.Instance.currentState.Value != GameStates.GameState.host2)
        {
            fromLocation = CardManager.CardLocation.Default;
            toLocation = CardManager.CardLocation.Default;

            draggingObject.position = startPosition;
            draggingObject.localScale = startScale;
            draggingObject = null;
            IsDragging = false;
            return;
        }
        if (!IsHost && GameStates.Instance.currentState.Value != GameStates.GameState.client2)
        {
            fromLocation = CardManager.CardLocation.Default;
            toLocation = CardManager.CardLocation.Default;

            draggingObject.position = startPosition;
            draggingObject.localScale = startScale;
            draggingObject = null;
            IsDragging = false;
            return;
        }

        Ray touchRay = playerCamera.ScreenPointToRay(finger.screenPosition);
        RaycastHit checkHit1;
        RaycastHit checkHit2;
        if (Physics.Raycast(touchRay, out checkHit1, Mathf.Infinity, LayerMask.GetMask("FieldCard")))
        {
            toLocation = checkHit1.transform.GetComponent<FieldLocation>().location;

            if (Physics.Raycast(touchRay, out checkHit2, Mathf.Infinity, LayerMask.GetMask("NetworkHostCard")))
            {
                
            }
            else if (Physics.Raycast(touchRay, out checkHit2, Mathf.Infinity, LayerMask.GetMask("NetworkClientCard")))
            {

            }
            else
            {
                CardHandler cardHandler = draggingObject.GetComponent<CardHandler>();
                int cardId = cardHandler.cardId;
                int location = (int)checkHit1.transform.GetComponent<FieldLocation>().location;
                CardManager.Instance.InsertFieldListServerRpc(location, cardId);

                if (fromLocation == CardManager.CardLocation.HostHand)
                {
                    CardManager.Instance.RemoveHostListServerRpc(cardHandler.GetIndex());
                }
                else if (fromLocation == CardManager.CardLocation.ClientHand)
                {
                    CardManager.Instance.RemoveClientListServerRpc(cardHandler.GetIndex());
                }
                else
                {
                    CardManager.Instance.RemoveFieldListServerRpc((int)fromLocation);
                }

                Debug.Log(toLocation + " xxx " + fromLocation);
                Destroy(draggingObject.gameObject);

                fromLocation = CardManager.CardLocation.Default;
                toLocation = CardManager.CardLocation.Default;

                draggingObject = null;
                IsDragging = false;
                return;
            }
        }
        else
        {
            fromLocation = CardManager.CardLocation.Default;
            toLocation = CardManager.CardLocation.Default;

            draggingObject.position = startPosition;
            draggingObject.localScale = startScale;
            draggingObject = null;
            IsDragging = false;
            return;
        }
    }

    // Debug

    private void ToggleCode()
    {
        if (!IsOwner) return;

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

    private void TestCode()
    {
        
    }
}
