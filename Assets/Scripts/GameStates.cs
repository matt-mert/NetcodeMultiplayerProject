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
    public delegate void StateChangedToPlayer1();
    public event StateChangedToPlayer1 OnStateChangedToPlayer1;
    public delegate void StateChangedToPlayer2();
    public event StateChangedToPlayer2 OnStateChangedToPlayer2;
    public delegate void StateChangedToOpponent1();
    public event StateChangedToOpponent1 OnStateChangedToOpponent1;
    public delegate void StateChangedToOpponent2();
    public event StateChangedToOpponent2 OnStateChangedToOpponent2;
    public delegate void StateChangedToEnd();
    public event StateChangedToEnd OnStateChangedToEnd;
    public delegate void StateChangedToLoading();
    public event StateChangedToLoading OnStateChangedToLoading;

    public enum GameState
    {
        menu,
        initial,
        start,
        player1,
        player2,
        opponent1,
        opponent2,
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
    }

    public override void OnNetworkSpawn()
    {
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
        NetworkManager.Singleton.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
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
    public void ChangeStateToPlayer1ClientRpc()
    {
        Debug.Log("Changing state to player1...");
        currentState = GameState.player1;
        if (OnStateChangedToPlayer1 == null) return;
        OnStateChangedToPlayer1.Invoke();
    }

    [ClientRpc]
    public void ChangeStateToPlayer2ClientRpc()
    {
        Debug.Log("Changing state to player2...");
        currentState = GameState.player2;
        if (OnStateChangedToPlayer2 == null) return;
        OnStateChangedToPlayer2.Invoke();
    }

    [ClientRpc]
    public void ChangeStateToOpponent1ClientRpc()
    {
        Debug.Log("Changing state to opponent1...");
        currentState = GameState.opponent1;
        if (OnStateChangedToOpponent1 == null) return;
        OnStateChangedToOpponent1.Invoke();
    }

    [ClientRpc]
    public void ChangeStateToOpponent2ClientRpc()
    {
        Debug.Log("Changing state to opponent2...");
        currentState = GameState.opponent2;
        if (OnStateChangedToOpponent2 == null) return;
        OnStateChangedToOpponent2.Invoke();
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
