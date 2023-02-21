using Unity.Netcode;
using UnityEngine;

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

    private void Start()
    {
        Debug.Log("Initial state is set to menu...");
        currentState = GameState.menu;
    }

    public void ChangeStateToMenu()
    {
        Debug.Log("Changing state to menu...");
        currentState = GameState.menu;
        OnStateChangedToMenu.Invoke();
    }

    public void ChangeStateToInitial()
    {
        Debug.Log("Changing state to initial...");
        currentState = GameState.initial;
        OnStateChangedToInitial.Invoke();
    }

    public void ChangeStateToStart()
    {
        Debug.Log("Changing state to start...");
        currentState = GameState.start;
        OnStateChangedToStart.Invoke();
    }

    public void ChangeStateToPlayer1()
    {
        Debug.Log("Changing state to player1...");
        currentState = GameState.player1;
        OnStateChangedToPlayer1.Invoke();
    }

    public void ChangeStateToPlayer2()
    {
        Debug.Log("Changing state to player2...");
        currentState = GameState.player2;
        OnStateChangedToPlayer2.Invoke();
    }

    public void ChangeStateToOpponent1()
    {
        Debug.Log("Changing state to opponent1...");
        currentState = GameState.opponent1;
        OnStateChangedToOpponent1.Invoke();
    }

    public void ChangeStateToOpponent2()
    {
        Debug.Log("Changing state to opponent2...");
        currentState = GameState.opponent2;
        OnStateChangedToOpponent2.Invoke();
    }

    public void ChangeStateToEnd()
    {
        Debug.Log("Changing state to end...");
        currentState = GameState.end;
        OnStateChangedToEnd.Invoke();
    }

    public void ChangeStateToLoading()
    {
        Debug.Log("Changing state to loading...");
        currentState = GameState.loading;
        OnStateChangedToLoading.Invoke();
    }

    public void OnClientConnect()
    {
        Debug.Log("Client connected.");
    }

    public void OnClientDisconnect()
    {
        Debug.Log("Client disconnected.");
    }
}
