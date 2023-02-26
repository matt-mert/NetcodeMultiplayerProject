using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CardManager : NetworkBehaviour
{
    public static CardManager Instance { get; private set; }

    public NetworkList<int> HostCardsList;
    public NetworkList<int> ClientCardsList;
    public NetworkList<int> FieldCardsList;

    [SerializeField]
    private GameDeck gameDeckSO;

    private Dictionary<CardLocation, Vector3> dictionary;

    [SerializeField]
    private GameObject genericCard;

    // Debug

    [SerializeField]
    private GameObject exampleCard;
    [SerializeField]
    private GameCard gameCardSO;

    public enum CardLocation
    {
        Default,
        FarSouth1,
        FarSouth2,
        FarSouth3,
        MidSouth1,
        MidSouth2,
        MidSouth3,
        MidNorth1,
        MidNorth2,
        MidNorth3,
        FarNorth1,
        FarNorth2,
        FarNorth3,
        MidSide,
        SouthBase,
        NorthBase,
        HostHand,
        ClientHand
    }

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

        dictionary = new Dictionary<CardLocation, Vector3>();

        HostCardsList = new NetworkList<int>();
        ClientCardsList = new NetworkList<int>();
        FieldCardsList = new NetworkList<int>();
    }

    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneChanged;
        GameStates.Instance.OnStateChangedToStart += InitializeLocationsClientRpc;
        HostCardsList.OnListChanged += HostDrawListener;
        ClientCardsList.OnListChanged += ClientDrawListener;
        FieldCardsList.OnListChanged += SpawnCardListener;
    }

    private void OnSceneChanged(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if (!IsOwner) return;

    }

    [ClientRpc]
    private void InitializeLocationsClientRpc()
    {
        Vector3 elevate = new Vector3(0f, 0.2f, 0f);
        FieldLocation[] fieldLocations = FindObjectsOfType<FieldLocation>();
        for (int i = 0; i < fieldLocations.Length; i++)
        {
            FieldLocation fieldLocation = fieldLocations[i];
            CardLocation location = fieldLocation.location;
            dictionary.Add(location, fieldLocation.transform.position + elevate);
        }

        if (!IsServer) return;

        for (int i = 0; i < 5; i++)
        {
            HostCardsList.Add(0);
        }

        for (int i = 0; i < 5; i++)
        {
            ClientCardsList.Add(0);
        }

        for (int i = 0; i < 16; i++)
        {
            FieldCardsList.Add(0);
        }
    }

    [ServerRpc]
    public void InsertHostListServerRpc(int index, int cardId)
    {
        HostCardsList.Insert(index, cardId);
    }

    private void HostDrawListener(NetworkListEvent<int> change)
    {
        if (GameStates.Instance.currentState.Value != GameStates.GameState.host1)
            return;

        if (!IsServer) return;

        HostDrawCardClientRpc();
    }

    [ClientRpc]
    public void HostDrawCardClientRpc()
    {
        Vector3 position = new Vector3(-3f, 15.01f, 25f);
        Quaternion rotation = Quaternion.Euler(new Vector3(-90f, 90f, 0f));
        if (IsHost)
        {
            GameObject spawnedCard = Instantiate(exampleCard, position, rotation);
            spawnedCard.GetComponent<CardHandler>().cardSO = gameCardSO;
        }
        else
        {
            Instantiate(genericCard, position, rotation);
        }
    }

    [ServerRpc]
    public void InsertClientListServerRpc(int index, int cardId)
    {
        ClientCardsList.Insert(index, cardId);
    }

    private void ClientDrawListener(NetworkListEvent<int> change)
    {
        if (GameStates.Instance.currentState.Value != GameStates.GameState.client1)
            return;

        if (!IsServer) return;

        ClientDrawCardClientRpc();
    }

    [ClientRpc]
    public void ClientDrawCardClientRpc()
    {
        Vector3 position = new Vector3(3f, 15.01f, -25f);
        Quaternion rotation = Quaternion.Euler(new Vector3(-90f, -90f, 0f));
        if (IsHost)
        {
            Instantiate(genericCard, position, rotation);
        }
        else
        {
            GameObject spawnedCard = Instantiate(exampleCard, position, rotation);
            spawnedCard.GetComponent<CardHandler>().cardSO = gameCardSO;
        }
    }

    [ServerRpc]
    public void InsertFieldListServerRpc(int location, int cardId)
    {
        FieldCardsList.Insert(location, cardId);
    }

    private void SpawnCardListener(NetworkListEvent<int> change)
    {
        if (GameStates.Instance.currentState.Value != GameStates.GameState.host2 &&
            GameStates.Instance.currentState.Value != GameStates.GameState.client2)
            return;

        int index = change.Index;

        if (!IsServer) return;

        SpawnCardClientRpc(index);
    }

    [ClientRpc]
    private void SpawnCardClientRpc(int index)
    {
        if (IsHost)
        {
            Vector3 hostPosition = dictionary[(CardLocation)index];
            Quaternion hostRotation = Quaternion.Euler(new Vector3(-90f, 90f, 0f));
            Instantiate(exampleCard, hostPosition, hostRotation);
        }
        else
        {
            Vector3 clientPosition = dictionary[(CardLocation)index];
            Quaternion clientRotation = Quaternion.Euler(new Vector3(-90f, -90f, 0f));
            Instantiate(exampleCard, clientPosition, clientRotation);
        }
    }
}
