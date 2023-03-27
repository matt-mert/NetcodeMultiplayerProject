using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FlowManager : NetworkBehaviour
{
    public static FlowManager Instance { get; private set; }

    public delegate void TurnCounterUpdate(int prev, int next);
    public event TurnCounterUpdate OnTurnCounterUpdate;
    public delegate void PlayerTimerUpdate(int prev, int next);
    public event PlayerTimerUpdate OnPlayerTimerUpdate;
    public delegate void OpponentTimerUpdate(int prev, int next);
    public event OpponentTimerUpdate OnOpponentTimerUpdate;
    public delegate void PlayerEnergyUpdate(int prev, int next);
    public event PlayerEnergyUpdate OnPlayerEnergyUpdate;
    public delegate void OpponentEnergyUpdate(int prev, int next);
    public event OpponentEnergyUpdate OnOpponentEnergyUpdate;

    [HideInInspector]
    public NetworkVariable<int> TurnCounter;
    [HideInInspector]
    public NetworkVariable<int> HostEnergy;
    [HideInInspector]
    public NetworkVariable<int> HostTimer;
    [HideInInspector]
    public NetworkVariable<int> ClientEnergy;
    [HideInInspector]
    public NetworkVariable<int> ClientTimer;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        TurnCounter = new NetworkVariable<int>();
        HostEnergy = new NetworkVariable<int>();
        HostTimer = new NetworkVariable<int>();
        ClientEnergy = new NetworkVariable<int>();
        ClientTimer = new NetworkVariable<int>();
    }

    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneChanged;
        StatesManager.Instance.OnStateChangedToStart += InitializationsClientRpc;
        StatesManager.Instance.OnStateChangedToHost2 += StartHostTurnServerRpc;
        StatesManager.Instance.OnStateChangedToClient2 += StartClientTurnServerRpc;
    }

    private void OnSceneChanged(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if (!IsOwner) return;

    }

    [ClientRpc]
    private void InitializationsClientRpc()
    {
        if (!IsServer) return;

        TurnCounter.Value = 0;
        HostEnergy.Value = 0;
        HostTimer.Value = 0;
        ClientEnergy.Value = 0;
        ClientTimer.Value = 0;
    }

    [ServerRpc(RequireOwnership = false)]
    public void IncrementTurnCounterServerRpc(int amount = 1)
    {
        int prev = TurnCounter.Value;
        int next = prev + amount;
        TurnCounter.Value = next;
        TurnCounterEventHandlerClientRpc(prev, next);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetHostTimerServerRpc(int value)
    {
        if (value <= 0) return;
        int prev = HostTimer.Value;
        int next = value;
        HostTimer.Value = next;
        HostTimerEventHandlerClientRpc(prev, next);
    }

    [ServerRpc(RequireOwnership = false)]
    public void IncrementHostTimerServerRpc(int amount = 1)
    {
        if (amount <= 0) return;
        int prev = HostTimer.Value;
        int next = prev + amount;
        HostTimer.Value = next;
        HostTimerEventHandlerClientRpc(prev, next);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DecrementHostTimerServerRpc(int amount = 1)
    {
        if (amount <= 0) return;
        int prev = HostTimer.Value;
        int next = ((prev - amount) > 0) ? (prev - amount) : 0;
        HostTimer.Value = next;
        HostTimerEventHandlerClientRpc(prev, next);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetClientTimerServerRpc(int value)
    {
        if (value <= 0) return;
        int prev = ClientTimer.Value;
        int next = value;
        ClientTimer.Value = next;
        ClientTimerEventHandlerClientRpc(prev, next);
    }

    [ServerRpc(RequireOwnership = false)]
    public void IncrementClientTimerServerRpc(int amount = 1)
    {
        if (amount <= 0) return;
        int prev = ClientTimer.Value;
        int next = prev + amount;
        ClientTimer.Value = next;
        ClientTimerEventHandlerClientRpc(prev, next);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DecrementClientTimerServerRpc(int amount = 1)
    {
        if (amount <= 0) return;
        int prev = ClientTimer.Value;
        int next = ((prev - amount) > 0) ? (prev - amount) : 0;
        ClientTimer.Value = next;
        ClientTimerEventHandlerClientRpc(prev, next);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetHostEnergyServerRpc(int value)
    {
        int prev = HostEnergy.Value;
        int next = value;
        HostEnergy.Value = next;
        HostEnergyEventHandlerClientRpc(prev, next);
    }

    [ServerRpc(RequireOwnership = false)]
    public void IncrementHostEnergyServerRpc()
    {
        int prev = HostEnergy.Value;
        int next = prev + TurnCounter.Value;
        HostEnergy.Value = next;
        HostEnergyEventHandlerClientRpc(prev, next);
    }

    [ServerRpc(RequireOwnership = false)]
    public void IncrementHostEnergyServerRpc(int amount)
    {
        if (amount <= 0) return;
        int prev = HostEnergy.Value;
        int next = prev + amount;
        HostEnergy.Value = next;
        HostEnergyEventHandlerClientRpc(prev, next);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DecrementHostEnergyServerRpc(int amount)
    {
        if (amount <= 0) return;
        int prev = HostEnergy.Value;
        int next = ((prev - amount) > 0) ? (prev - amount) : 0;
        HostEnergy.Value = next;
        HostEnergyEventHandlerClientRpc(prev, next);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetClientEnergyServerRpc(int value)
    {
        int prev = ClientEnergy.Value;
        int next = value;
        ClientEnergy.Value = next;
        ClientEnergyEventHandlerClientRpc(prev, next);
    }

    [ServerRpc(RequireOwnership = false)]
    public void IncrementClientEnergyServerRpc()
    {
        int prev = ClientEnergy.Value;
        int next = prev + TurnCounter.Value;
        ClientEnergy.Value = next;
        ClientEnergyEventHandlerClientRpc(prev, next);
    }

    [ServerRpc(RequireOwnership = false)]
    public void IncrementClientEnergyServerRpc(int amount)
    {
        if (amount <= 0) return;
        int prev = ClientEnergy.Value;
        int next = prev + amount;
        ClientEnergy.Value = next;
        ClientEnergyEventHandlerClientRpc(prev, next);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DecrementClientEnergyServerRpc(int amount)
    {
        if (amount <= 0) return;
        int prev = ClientEnergy.Value;
        int next = ((prev - amount) > 0) ? (prev - amount) : 0;
        ClientEnergy.Value = next;
        ClientEnergyEventHandlerClientRpc(prev, next);
    }

    [ClientRpc]
    private void TurnCounterEventHandlerClientRpc(int prev, int next)
    {
        if (OnTurnCounterUpdate != null) OnTurnCounterUpdate.Invoke(prev, next);
    }

    [ClientRpc]
    private void HostTimerEventHandlerClientRpc(int prev, int next)
    {
        if (IsHost)
        {
            if (OnPlayerTimerUpdate != null) OnPlayerTimerUpdate.Invoke(prev, next);
        }
        else
        {
            if (OnOpponentTimerUpdate != null) OnOpponentTimerUpdate.Invoke(prev, next);
        }
    }

    [ClientRpc]
    private void ClientTimerEventHandlerClientRpc(int prev, int next)
    {
        if (IsHost)
        {
            if (OnOpponentTimerUpdate != null) OnOpponentTimerUpdate.Invoke(prev, next);
        }
        else
        {
            if (OnPlayerTimerUpdate != null) OnPlayerTimerUpdate.Invoke(prev, next);
        }
    }

    [ClientRpc]
    private void HostEnergyEventHandlerClientRpc(int prev, int next)
    {
        if (IsHost)
        {
            if (OnPlayerEnergyUpdate != null) OnPlayerEnergyUpdate.Invoke(prev, next);
        }
        else
        {
            if (OnOpponentEnergyUpdate != null) OnOpponentEnergyUpdate.Invoke(prev, next);
        }
    }

    [ClientRpc]
    private void ClientEnergyEventHandlerClientRpc(int prev, int next)
    {
        if (IsHost)
        {
            if (OnOpponentEnergyUpdate != null) OnOpponentEnergyUpdate.Invoke(prev, next);
        }
        else
        {
            if (OnPlayerEnergyUpdate != null) OnPlayerEnergyUpdate.Invoke(prev, next);
        }
    }

    [ServerRpc]
    public void StartHostTurnServerRpc()
    {
        SetHostTimerServerRpc(40);
        HostTurnTimerAsync().Forget();
    }

    [ServerRpc(RequireOwnership = false)]
    public void EndHostTurnServerRpc()
    {
        SetHostTimerServerRpc(0);
        StatesManager.Instance.ChangeStateToHost3ClientRpc();
    }

    [ServerRpc]
    public void StartClientTurnServerRpc()
    {
        SetClientTimerServerRpc(40);
        ClientTurnTimerAsync().Forget();
    }

    [ServerRpc(RequireOwnership = false)]
    public void EndClientTurnServerRpc()
    {
        SetClientTimerServerRpc(0);
        StatesManager.Instance.ChangeStateToClient3ClientRpc();
    }

    private async UniTask HostTurnTimerAsync()
    {
        while (HostTimer.Value > 0)
        {
            DecrementHostTimerServerRpc();
            await UniTask.Delay(1000);
        }
        EndHostTurnServerRpc();
    }

    private async UniTask ClientTurnTimerAsync()
    {
        while (ClientTimer.Value > 0)
        {
            DecrementClientTimerServerRpc();
            await UniTask.Delay(1000);
        }
        EndClientTurnServerRpc();
    }
}
