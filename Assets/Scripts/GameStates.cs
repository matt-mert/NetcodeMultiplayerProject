using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [HideInInspector]
    public GameState currentState;

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

        currentState = GameState.menu;
    }

    [ClientRpc]
    public void ChangeStateToMenuClientRpc()
    {
        Debug.Log("Changing state to menu...");
        NetworkManager.Singleton.SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
        currentState = GameState.menu;
        if (OnStateChangedToMenu == null) return;
        OnStateChangedToMenu.Invoke();
    }

    [ClientRpc]
    public void ChangeStateToInitialClientRpc()
    {
        Debug.Log("Changing state to initial...");
        NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        currentState = GameState.initial;
        if (OnStateChangedToInitial == null) return;
        OnStateChangedToInitial.Invoke();
    }

    [ClientRpc]
    public void ChangeStateToStartClientRpc()
    {
        Debug.Log("Changing state to start...");
        currentState = GameState.start;
        if (OnStateChangedToStart == null) return;
        OnStateChangedToStart.Invoke();
    }

    [ClientRpc]
    public void ChangeStateToHost1ClientRpc()
    {
        Debug.Log("Changing state to Host1...");
        currentState = GameState.host1;
        if (OnStateChangedToHost1 == null) return;
        OnStateChangedToHost1.Invoke();
    }

    [ClientRpc]
    public void ChangeStateToHost2ClientRpc()
    {
        Debug.Log("Changing state to Host2...");
        currentState = GameState.host2;
        if (OnStateChangedToHost2 == null) return;
        OnStateChangedToHost2.Invoke();
    }

    [ClientRpc]
    public void ChangeStateToClient1ClientRpc()
    {
        Debug.Log("Changing state to Client1...");
        currentState = GameState.client1;
        if (OnStateChangedToClient1 == null) return;
        OnStateChangedToClient1.Invoke();
    }

    [ClientRpc]
    public void ChangeStateToClient2ClientRpc()
    {
        Debug.Log("Changing state to Client2...");
        currentState = GameState.client2;
        if (OnStateChangedToClient2 == null) return;
        OnStateChangedToClient2.Invoke();
    }

    [ClientRpc]
    public void ChangeStateToEndClientRpc()
    {
        Debug.Log("Changing state to end...");
        currentState = GameState.end;
        if (OnStateChangedToEnd == null) return;
        OnStateChangedToEnd.Invoke();
    }

    [ClientRpc]
    public void ChangeStateToLoadingClientRpc()
    {
        Debug.Log("Changing state to loading...");
        currentState = GameState.loading;
        if (OnStateChangedToLoading == null) return;
        OnStateChangedToLoading.Invoke();
    }
}
