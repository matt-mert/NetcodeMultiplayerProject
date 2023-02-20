using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStates : NetworkSingleton<GameStates>
{
    public delegate void StateChangedToMenu();
    public event StateChangedToMenu OnStateChangedToMenu;
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

    private void Start()
    {
        Debug.Log("Deneme");
    }

    public void ChangeStateToMenu()
    {
        currentState = GameState.menu;
        OnStateChangedToMenu.Invoke();
    }

    public void ChangeStateToStart()
    {
        currentState = GameState.start;
        OnStateChangedToStart.Invoke();
    }

    public void ChangeStateToPlayer1()
    {
        currentState = GameState.player1;
        OnStateChangedToPlayer1.Invoke();
    }

    public void ChangeStateToPlayer2()
    {
        currentState = GameState.player2;
        OnStateChangedToPlayer2.Invoke();
    }

    public void ChangeStateToOpponent1()
    {
        currentState = GameState.opponent1;
        OnStateChangedToOpponent1.Invoke();
    }

    public void ChangeStateToOpponent2()
    {
        currentState = GameState.opponent2;
        OnStateChangedToOpponent2.Invoke();
    }

    public void ChangeStateToEnd()
    {
        currentState = GameState.end;
        OnStateChangedToEnd.Invoke();
    }

    public void ChangeStateToLoading()
    {
        currentState = GameState.loading;
        OnStateChangedToLoading.Invoke();
    }

    public void OnClientConnect()
    {
        
    }

    public void OnClientDisconnect()
    {

    }
}
