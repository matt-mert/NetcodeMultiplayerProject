using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

// Written by https://github.com/matt-mert

public class GameStates : NetworkBehaviour
{
    public static GameStates Instance { get; private set; }

    public delegate void StateChangedToMenu();
    public event StateChangedToMenu OnStateChangedToMenu;
    public delegate void StateChangedToInitial();
    public event StateChangedToInitial OnStateChangedToInitial;
    public delegate void StateChangedToStart();
    public event StateChangedToStart OnStateChangedToStart;
    public delegate void StateChangedToHost1();
    public event StateChangedToHost1 OnStateChangedToHost1;
    public delegate void StateChangedToHost2();
    public event StateChangedToHost2 OnStateChangedToHost2;
    public delegate void StateChangedToClient1();
    public event StateChangedToClient1 OnStateChangedToClient1;
    public delegate void StateChangedToClient2();
    public event StateChangedToClient2 OnStateChangedToClient2;
    public delegate void StateChangedToEnd();
    public event StateChangedToEnd OnStateChangedToEnd;
    public delegate void StateChangedToLoading();
    public event StateChangedToLoading OnStateChangedToLoading;

    public enum GameState
    {
        menu,
        initial,
        start,
        host1,
        host2,
        client1,
        client2,
        end,
        loading,
    }

    public NetworkVariable<GameState> currentState;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void OnNetworkSpawn()
    {
        currentState.Value = GameState.menu;
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneChanged;
    }

    private void OnSceneChanged(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if (!IsOwner) return;

    }

    [ClientRpc]
    public void ChangeStateToMenuClientRpc()
    {
        Debug.Log("Changing state to menu...");
        NetworkManager.Singleton.SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
        currentState.Value = GameState.menu;
        if (OnStateChangedToMenu != null) OnStateChangedToMenu.Invoke();
    }

    [ClientRpc]
    public void ChangeStateToInitialClientRpc()
    {
        Debug.Log("Changing state to initial...");
        NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        currentState.Value = GameState.initial;
        if (OnStateChangedToInitial != null) OnStateChangedToInitial.Invoke();
    }

    [ClientRpc]
    public void ChangeStateToStartClientRpc()
    {
        if (!IsHost) Camera.main.transform.Rotate(new Vector3(0f, 0f, 180f));
        Debug.Log("Changing state to start...");
        currentState.Value = GameState.start;
        if (OnStateChangedToStart != null) OnStateChangedToStart.Invoke();
    }

    [ClientRpc]
    public void ChangeStateToHost1ClientRpc()
    {
        Debug.Log("Changing state to Host1...");
        currentState.Value = GameState.host1;
        if (OnStateChangedToHost1 != null) OnStateChangedToHost1.Invoke();

        CardManager.Instance.HostDrawCardClientRpc();
    }

    [ClientRpc]
    public void ChangeStateToHost2ClientRpc()
    {
        Debug.Log("Changing state to Host2...");
        currentState.Value = GameState.host2;
        if (OnStateChangedToHost2 != null) OnStateChangedToHost2.Invoke();
    }

    [ClientRpc]
    public void ChangeStateToClient1ClientRpc()
    {
        Debug.Log("Changing state to Client1...");
        currentState.Value = GameState.client1;
        if (OnStateChangedToClient1 != null) OnStateChangedToClient1.Invoke();

        CardManager.Instance.ClientDrawCardClientRpc();
    }

    [ClientRpc]
    public void ChangeStateToClient2ClientRpc()
    {
        Debug.Log("Changing state to Client2...");
        currentState.Value = GameState.client2;
        if (OnStateChangedToClient2 != null) OnStateChangedToClient2.Invoke();
    }

    [ClientRpc]
    public void ChangeStateToEndClientRpc()
    {
        Debug.Log("Changing state to end...");
        currentState.Value = GameState.end;
        if (OnStateChangedToEnd != null) OnStateChangedToEnd.Invoke();
    }

    [ClientRpc]
    public void ChangeStateToLoadingClientRpc()
    {
        Debug.Log("Changing state to loading...");
        currentState.Value = GameState.loading;
        if (OnStateChangedToLoading != null) OnStateChangedToLoading.Invoke();

    }
}
