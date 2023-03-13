using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.SceneManagement;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

// Written by https://github.com/matt-mert

public class PlayerNetwork : NetworkBehaviour
{
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

    private InputAction toggleAction;
    private InputAction testAction;

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
        toggleAction = playerInput.actions["Toggle"];
        testAction = playerInput.actions["Test"];
    }

    private void FingerDown(Finger finger)
    {
        if (!IsOwner) return;

        if (draggingObject != null || IsDragging) return;

        activeFinger = finger;
        if (activeFinger.index > 0) return;

        Ray touchRay = playerCamera.ScreenPointToRay(activeFinger.screenPosition);
        RaycastHit checkHit1;
        if (Physics.Raycast(touchRay, out checkHit1, Mathf.Infinity, 1 << 6))
        {
            if (checkHit1.transform.CompareTag("HandCard"))
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
            else if (checkHit1.transform.CompareTag("HostCard") && IsHost)
            {
                draggingObject = checkHit1.transform;
                CardIndexer indexer = draggingObject.GetComponent<CardIndexer>();
                fromLocation = (CardManager.CardLocation)indexer.GetFieldIndex();

                startPosition = draggingObject.position;
                startScale = draggingObject.localScale;
                draggingObject.localScale = startScale * 2f;
                draggingObject.position = checkHit1.point;
                IsDragging = true;
            }
            else if (checkHit1.transform.CompareTag("ClientCard") && !IsHost)
            {
                draggingObject = checkHit1.transform;
                CardIndexer indexer = draggingObject.GetComponent<CardIndexer>();
                fromLocation = (CardManager.CardLocation)indexer.GetFieldIndex();

                startPosition = draggingObject.position;
                startScale = draggingObject.localScale;
                draggingObject.localScale = startScale * 2f;
                draggingObject.position = checkHit1.point;
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
        if (Physics.Raycast(touchRay, out checkHit, Mathf.Infinity, 1 << 3))
        {
            draggingObject.transform.position = checkHit.point;
        }
        else
        {
            fromLocation = CardManager.CardLocation.Default;
            toLocation = CardManager.CardLocation.Default;

            draggingObject.position = startPosition;
            draggingObject.localScale = startScale;
            activeFinger = null;
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

        if ((IsHost && GameStates.Instance.currentState.Value != GameStates.GameState.host2) ||
            (!IsHost && GameStates.Instance.currentState.Value != GameStates.GameState.client2))
        {
            fromLocation = CardManager.CardLocation.Default;
            toLocation = CardManager.CardLocation.Default;

            draggingObject.position = startPosition;
            draggingObject.localScale = startScale;
            activeFinger = null;
            draggingObject = null;
            IsDragging = false;
            return;
        }

        Ray touchRay = playerCamera.ScreenPointToRay(finger.screenPosition);
        RaycastHit checkHit1;
        RaycastHit[] checkHit2;
        // Checking if there is a field plane
        if (Physics.Raycast(touchRay, out checkHit1, Mathf.Infinity, 1 << 7))
        {
            toLocation = checkHit1.transform.GetComponent<FieldLocation>().location;
            checkHit2 = Physics.RaycastAll(touchRay, Mathf.Infinity, 1 << 6);

            // Checking if there is a movable object and it is not the draggingObject
            if (checkHit2.Length > 1)
            {
                Transform obj = checkHit2[1].transform;

                if (obj.CompareTag("HostCard"))
                {

                }
                else if (obj.CompareTag("ClientCard"))
                {

                }
            }
            else
            {
                if (fromLocation == toLocation)
                {
                    fromLocation = CardManager.CardLocation.Default;
                    toLocation = CardManager.CardLocation.Default;

                    draggingObject.position = startPosition;
                    draggingObject.localScale = startScale;
                    activeFinger = null;
                    draggingObject = null;
                    IsDragging = false;
                    return;
                }
                else if (fromLocation == CardManager.CardLocation.HostHand)
                {
                    toLocation = checkHit1.transform.GetComponent<FieldLocation>().location;
                    CardHandler cardHandler = draggingObject.GetComponent<CardHandler>();
                    CardIndexer cardIndexer = draggingObject.GetComponent<CardIndexer>();
                    int cardId = cardHandler.cardId;
                    CardManager.Instance.InsertFieldListServerRpc((int)toLocation, cardId);
                    CardManager.Instance.RemoveHostListServerRpc(cardIndexer.GetHandIndex());
                    Debug.Log("from " + fromLocation + " to " + toLocation);

                    fromLocation = CardManager.CardLocation.Default;
                    toLocation = CardManager.CardLocation.Default;

                    activeFinger = null;
                    draggingObject = null;
                    IsDragging = false;
                    return;
                }
                else if (fromLocation == CardManager.CardLocation.ClientHand)
                {
                    toLocation = checkHit1.transform.GetComponent<FieldLocation>().location;
                    CardHandler cardHandler = draggingObject.GetComponent<CardHandler>();
                    CardIndexer cardIndexer = draggingObject.GetComponent<CardIndexer>();
                    int cardId = cardHandler.cardId;
                    CardManager.Instance.InsertFieldListServerRpc((int)toLocation, cardId);
                    CardManager.Instance.RemoveClientListServerRpc(cardIndexer.GetHandIndex());
                    Debug.Log("from " + fromLocation + " to " + toLocation);

                    fromLocation = CardManager.CardLocation.Default;
                    toLocation = CardManager.CardLocation.Default;

                    activeFinger = null;
                    draggingObject = null;
                    IsDragging = false;
                    return;
                }
                else if (fromLocation != CardManager.CardLocation.Default)
                {
                    toLocation = checkHit1.transform.GetComponent<FieldLocation>().location;
                    CardHandler cardHandler = draggingObject.GetComponent<CardHandler>();
                    CardIndexer cardIndexer = draggingObject.GetComponent<CardIndexer>();
                    int cardId = cardHandler.cardId;
                    CardManager.Instance.InsertFieldListServerRpc((int)toLocation, cardId);
                    CardManager.Instance.RemoveFieldListServerRpc((int)fromLocation);
                    Debug.Log("from " + fromLocation + " to " + toLocation);

                    fromLocation = CardManager.CardLocation.Default;
                    toLocation = CardManager.CardLocation.Default;

                    activeFinger = null;
                    draggingObject = null;
                    IsDragging = false;
                    return;
                }
            }
        }
        else
        {
            fromLocation = CardManager.CardLocation.Default;
            toLocation = CardManager.CardLocation.Default;

            draggingObject.position = startPosition;
            draggingObject.localScale = startScale;
            activeFinger = null;
            draggingObject = null;
            IsDragging = false;
            return;
        }
    }

    // Debug

    private void ToggleCode()
    {
        if (!IsHost) return;

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
