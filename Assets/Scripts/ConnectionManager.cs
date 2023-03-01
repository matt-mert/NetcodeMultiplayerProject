using System.Collections;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine;

// Written by https://github.com/matt-mert
// Thanks to Code Monkey for great tutorial

public class ConnectionManager : NetworkBehaviour
{
    public static ConnectionManager Instance { get; private set; }

    [HideInInspector]
    public string joinCode = " ";

    private Lobby hostLobby;
    private Lobby joinedLobby;

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

    private async void Start()
    {
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in: " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateLobby()
    {
        try
        {
            string lobbyName = "MyLobby";
            int maxPlayers = 4;
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers);
            hostLobby = lobby;
            Debug.Log("Created Lobby: " + lobby.Name + " " + lobby.MaxPlayers);
            StartCoroutine(HeartbeatCoroutine());
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async void JoinLobby()
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
            Lobby lobby = await Lobbies.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id);
            joinedLobby = lobby;
            Debug.Log("Joined lobby: " + lobby.Name + " " + lobby.MaxPlayers);
            StartCoroutine(LobbyPollCoroutine());
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }
    
    private async void QuickJoinLobby()
    {
        try
        {
            await LobbyService.Instance.QuickJoinLobbyAsync();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    private async void ListLobbies()
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
            Debug.Log("Lobbies found: " + queryResponse.Results.Count);
            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    private IEnumerator HeartbeatCoroutine()
    {
        while (hostLobby != null)
        {
            HandleLobbyHeatbeat();
            yield return new WaitForSeconds(15);
        }
        StopCoroutine(HeartbeatCoroutine());
    }

    private IEnumerator LobbyPollCoroutine()
    {
        while (joinedLobby != null)
        {
            HandleLobbyPollForUpdates();
            yield return new WaitForSeconds(2);
        }
        StopCoroutine(LobbyPollCoroutine());
    }

    private async void HandleLobbyHeatbeat()
    {
        if (hostLobby == null) return;

        try
        {
            await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    private async void HandleLobbyPollForUpdates()
    {
        if (joinedLobby == null) return;

        try
        {
            Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
            joinedLobby = lobby;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async void CreateRelay()
    {
        try
        {
            Allocation alloc = await RelayService.Instance.CreateAllocationAsync(2);
            joinCode = await RelayService.Instance.GetJoinCodeAsync(alloc.AllocationId);
            RelayServerData relayServerData = new RelayServerData(alloc, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async void JoinRelay(string code)
    {
        try
        {
            JoinAllocation alloc = await RelayService.Instance.JoinAllocationAsync(code);
            RelayServerData relayServerData = new RelayServerData(alloc, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }
    }
}
