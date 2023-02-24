using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class StateManager : NetworkBehaviour
{
    public static StateManager Instance { get; private set; }

    [HideInInspector]
    public bool host1StateComplete;
    [HideInInspector]
    public bool client1StateComplete;

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
        GameStates.Instance.OnStateChangedToHost1 += Host1State;
        GameStates.Instance.OnStateChangedToClient1 += Client1State;
    }

    public override void OnNetworkDespawn()
    {
        GameStates.Instance.OnStateChangedToHost1 -= Host1State;
        GameStates.Instance.OnStateChangedToClient1 -= Client1State;
    }

    public void StepToNextState()
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

    private void Host1State()
    {
        CardManager.Instance.HostDrawCardClientRpc();
        // StartCoroutine(Host1StateCoroutine());
    }

    private IEnumerator Host1StateCoroutine()
    {
        while (!host1StateComplete)
        {
            yield return new WaitForSeconds(1);
        }
        StepToNextState();
        StopCoroutine(Host1StateCoroutine());
    }

    private void Client1State()
    {
        CardManager.Instance.ClientDrawCardClientRpc();
        // StartCoroutine(Client1StateCoroutine());
    }

    private IEnumerator Client1StateCoroutine()
    {
        while (!client1StateComplete)
        {
            yield return new WaitForSeconds(1);
        }
        StepToNextState();
        StopCoroutine(Client1StateCoroutine());
    }

    public void EndPlayerTurn()
    {
        if (IsHost && GameStates.Instance.currentState.Value == GameStates.GameState.host2)
        {
            StepToNextState();
        }
        else if (!IsHost && GameStates.Instance.currentState.Value == GameStates.GameState.client2)
        {
            StepToNextState();
        }
    }

    public void EndGame()
    {

    }
}
