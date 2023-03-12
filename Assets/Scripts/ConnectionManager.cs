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
using Cysharp.Threading.Tasks;

// Written by https://github.com/matt-mert
// Thanks to Code Monkey for great tutorial

namespace NotEnoughCheddar.Networking
{
    public class ConnectionManager : NetworkBehaviour
    {
        public static ConnectionManager Instance { get; private set; }

        [HideInInspector]
        public string joinCode = " ";

        /*
        private Lobby hostLobby;
        private Lobby joinedLobby;
        */

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
            NetworkManager.Singleton.OnClientConnectedCallback += OnPlayerConnect;
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

        public async void CreateRelay()
        {
            try
            {
                Allocation alloc = await RelayService.Instance.CreateAllocationAsync(2, "europe-west2");
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

        public async UniTask CreateRelayAsync()
        {
            int waitTime = 500;
            Debug.Log("Creating relay server...");
            Allocation alloc = await GetCreateAllocation(2, "europe-west2");
            Debug.Log("Created Allocation successfully.");
            joinCode = await GetRelayCodeAsync(alloc.AllocationId);
            Debug.Log("Created join code successfully.");
            UniTask.Create(async () =>
            {
                await UniTask.Delay(waitTime);
                RelayServerData relayServerData = new RelayServerData(alloc, "dtls");
                Debug.Log("New RelayServerData created.");
                await UniTask.Delay(waitTime);
                UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                Debug.Log("Connected to UnityTransport.");
                await UniTask.Delay(waitTime);
                unityTransport.SetRelayServerData(relayServerData);
                Debug.Log("Successfully passed RelayServerData to UnityTransport.");
                await UniTask.Delay(waitTime);
                NetworkManager.Singleton.StartHost();
                Debug.Log("Host started successfully");
            }).Forget(e => throw e);
        }

        private async UniTask<Allocation> GetCreateAllocation(int count, string region)
        {
            await UniTask.Delay(500);
            Debug.Log("Creating Allocation...");
            return await RelayService.Instance.CreateAllocationAsync(count, region);
        }

        private async UniTask<string> GetRelayCodeAsync(System.Guid id)
        {
            await UniTask.Delay(500);
            Debug.Log("Creating relay code...");
            return await RelayService.Instance.GetJoinCodeAsync(id);
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

        public async UniTask JoinRelayAsync(string code)
        {
            int waitTime = 500;
            Debug.Log("Joining relay server...");
            JoinAllocation alloc = await GetJoinAllocation(code);
            Debug.Log("Created JoinAllocation successfully.");
            UniTask.Create(async () =>
            {
                await UniTask.Delay(waitTime);
                RelayServerData relayServerData = new RelayServerData(alloc, "dtls");
                Debug.Log("New RelayServerData created.");
                await UniTask.Delay(waitTime);
                UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                Debug.Log("Connected to UnityTransport.");
                await UniTask.Delay(waitTime);
                unityTransport.SetRelayServerData(relayServerData);
                Debug.Log("Successfully passed RelayServerData to UnityTransport.");
                await UniTask.Delay(waitTime);
                NetworkManager.Singleton.StartClient();
                Debug.Log("Client started successfully");
            }).Forget(e => throw e);
        }

        private async UniTask<JoinAllocation> GetJoinAllocation(string code)
        {
            await UniTask.Delay(500);
            Debug.Log("Creating JoinAllocation...");
            return await RelayService.Instance.JoinAllocationAsync(code);
        }

        private void OnPlayerConnect(ulong info)
        {
            if (!IsServer) return;

            if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
            {
                GameStates.Instance.ChangeStateToInitialClientRpc();
            }
        }

        /*
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
        */
    }
}
