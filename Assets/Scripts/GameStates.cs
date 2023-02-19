using Unity.Netcode;
using UnityEngine;

public class GameStates : NetworkSingleton<GameStates>
{
    public enum GameState
    {
        menu,
        game
    }

    [HideInInspector]
    public GameState currentState;

    public void OnClientConnect()
    {
        

        if (NetworkManager.Singleton.ConnectedClients.Count == 2)
        {

        }
    }

    public void OnClientDisconnect()
    {

    }
}
