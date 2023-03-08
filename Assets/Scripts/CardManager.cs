using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

// Written by https://github.com/matt-mert

public class CardManager : NetworkBehaviour
{
    public static CardManager Instance { get; private set; }

    public NetworkList<int> HostCardsList;
    public NetworkList<int> ClientCardsList;
    public NetworkList<int> FieldCardsList;

    [SerializeField]
    private GameObject genericCard;
    [SerializeField]
    private GameDeck gameDeckSO;
    [SerializeField]
    private int handLimit = 5;

    private bool isReordering = false;

    private Dictionary<CardLocation, Vector3> dictionary;
    private List<GameObject> genericCardsList;
    private List<GameObject> fieldHostObjectsList;
    private List<GameObject> fieldClientObjectsList;
    private List<GameObject> hostHandObjectsList;
    private List<GameObject> clientHandObjectsList;

    // Debug

    [SerializeField]
    private GameObject exampleCard;
    [SerializeField]
    private GameObject exampleHandCard;
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
        genericCardsList = new List<GameObject>();
        fieldHostObjectsList = new List<GameObject>();
        fieldClientObjectsList = new List<GameObject>();
        hostHandObjectsList = new List<GameObject>();
        clientHandObjectsList = new List<GameObject>();

        HostCardsList = new NetworkList<int>();
        ClientCardsList = new NetworkList<int>();
        FieldCardsList = new NetworkList<int>();
    }

    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneChanged;
        GameStates.Instance.OnStateChangedToStart += InitializeLocationsClientRpc;
        HostCardsList.OnListChanged += HostHandListListener;
        ClientCardsList.OnListChanged += ClientHandListListener;
        FieldCardsList.OnListChanged += FieldCardListListener;
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

        for (int i = 0; i < handLimit; i++)
        {
            genericCardsList.Add(null);
        }

        for (int i = 0; i < fieldLocations.Length + 1; i++)
        {
            fieldHostObjectsList.Add(null);
        }

        for (int i = 0; i < fieldLocations.Length + 1; i++)
        {
            fieldClientObjectsList.Add(null);
        }

        for (int i = 0; i < handLimit; i++)
        {
            hostHandObjectsList.Add(null);
        }

        for (int i = 0; i < handLimit; i++)
        {
            clientHandObjectsList.Add(null);
        }

        if (!IsServer) return;
        
        for (int i = 0; i < handLimit; i++)
        {
            HostCardsList.Add(0);
        }

        for (int i = 0; i < handLimit; i++)
        {
            ClientCardsList.Add(0);
        }

        for (int i = 0; i < 16; i++)
        {
            FieldCardsList.Add(0);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void InsertHostListServerRpc(int index, int cardId)
    {
        HostCardsList[index] = cardId;
    }

    [ServerRpc(RequireOwnership = false)]
    public void RemoveHostListServerRpc(int index)
    {
        HostCardsList[index] = 0;
    }

    private void HostHandListListener(NetworkListEvent<int> change)
    {
        AfterHostPlayedClientRpc(change.Value, change.Index);
    }

    [ClientRpc]
    private void AfterHostPlayedClientRpc(int value, int index)
    {
        if (GameStates.Instance.currentState.Value != GameStates.GameState.host2)
            return;

        if (value == 0 && !isReordering)
        {
            if (IsHost)
            {
                Destroy(hostHandObjectsList[index]);
                hostHandObjectsList[index] = null;
                ReorderObjectList(1, hostHandObjectsList);
                isReordering = true;
                ReorderNetworkList(HostCardsList);
                isReordering = false;
            }
            else
            {
                Destroy(genericCardsList[index]);
                genericCardsList[index] = null;
                ReorderObjectList(1, genericCardsList);
            }
        }
    }

    [ClientRpc]
    public void HostDrawCardClientRpc()
    {
        Quaternion rotation = Quaternion.Euler(new Vector3(-90f, 90f, 0f));
        if (IsHost)
        {
            int index = FindFirstEmptyIndex(HostCardsList);
            Vector3 pos = FindPositionOfIndex(1, index);
            GameObject spawnedCard = Instantiate(exampleHandCard, pos, rotation);
            hostHandObjectsList[index] = spawnedCard;
            CardHandler cardHandler = spawnedCard.GetComponent<CardHandler>();
            CardIndexer cardIndexer = spawnedCard.GetComponent<CardIndexer>();
            cardHandler.cardSO = gameCardSO;
            cardIndexer.SetIndex(index);
            InsertHostListServerRpc(index, cardHandler.cardId);
        }
        else
        {
            int index = FindFirstEmptyIndex(genericCardsList);
            Vector3 pos = FindPositionOfIndex(1, index);
            GameObject generic = Instantiate(genericCard, pos, rotation);
            CardIndexer cardIndexer = generic.GetComponent<CardIndexer>();
            cardIndexer.SetIndex(index);
            if (index != -1) genericCardsList[index] = generic;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void InsertClientListServerRpc(int index, int cardId)
    {
        ClientCardsList[index] = cardId;
    }

    [ServerRpc(RequireOwnership = false)]
    public void RemoveClientListServerRpc(int index)
    {
        ClientCardsList[index] = 0;
    }

    private void ClientHandListListener(NetworkListEvent<int> change)
    {
        AfterClientPlayedClientRpc(change.Value, change.Index);
    }

    [ClientRpc]
    private void AfterClientPlayedClientRpc(int value, int index)
    {
        if (GameStates.Instance.currentState.Value != GameStates.GameState.client2)
            return;

        if (value == 0 && !isReordering)
        {
            if (IsHost)
            {
                Destroy(genericCardsList[index]);
                genericCardsList[index] = null;
                ReorderObjectList(0, genericCardsList);
                isReordering = true;
                ReorderNetworkList(ClientCardsList);
                isReordering = false;
            }
            else
            {
                Destroy(clientHandObjectsList[index]);
                clientHandObjectsList[index] = null;
                ReorderObjectList(0, clientHandObjectsList);
            }
        }
    }

    [ClientRpc]
    public void ClientDrawCardClientRpc()
    {
        Quaternion rotation = Quaternion.Euler(new Vector3(-90f, -90f, 0f));
        if (IsHost)
        {
            int index = FindFirstEmptyIndex(genericCardsList);
            Vector3 pos = FindPositionOfIndex(0, index);
            GameObject generic = Instantiate(genericCard, pos, rotation);
            CardIndexer cardIndexer = generic.GetComponent<CardIndexer>();
            cardIndexer.SetIndex(index);
            if (index != -1) genericCardsList[index] = generic;
        }
        else
        {
            int index = FindFirstEmptyIndex(ClientCardsList);
            Vector3 pos = FindPositionOfIndex(0, index);
            GameObject spawnedCard = Instantiate(exampleHandCard, pos, rotation);
            clientHandObjectsList[index] = spawnedCard;
            CardHandler cardHandler = spawnedCard.GetComponent<CardHandler>();
            CardIndexer cardIndexer = spawnedCard.GetComponent<CardIndexer>();
            cardHandler.cardSO = gameCardSO;
            cardIndexer.SetIndex(index);
            InsertClientListServerRpc(index, cardHandler.cardId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void InsertFieldListServerRpc(int location, int cardId)
    {
        FieldCardsList[location] = cardId;
    }

    [ServerRpc(RequireOwnership = false)]
    public void RemoveFieldListServerRpc(int index)
    {
        FieldCardsList[index] = 0;
    }

    private void FieldCardListListener(NetworkListEvent<int> change)
    {
        if (GameStates.Instance.currentState.Value != GameStates.GameState.host2 &&
            GameStates.Instance.currentState.Value != GameStates.GameState.client2)
            return;

        if (!IsServer) return;

        if (change.Value == 0)
        {
            DespawnFieldCardClientRpc(change.Index);
        }
        else if (change.PreviousValue == 0)
        {
            if (GameStates.Instance.currentState.Value == GameStates.GameState.host2)
            {
                SpawnHostCardClientRpc(change.Index, 1);
            }
            else if (GameStates.Instance.currentState.Value == GameStates.GameState.client2)
            {
                SpawnClientCardClientRpc(change.Index, 1);
            }
        }
    }

    [ClientRpc]
    private void SpawnHostCardClientRpc(int index, int cardId)
    {
        if (IsHost)
        {
            CardLocation location = (CardLocation)index;
            Vector3 hostPosition = dictionary[location];
            Quaternion hostRotation = Quaternion.Euler(new Vector3(-90f, 90f, 0f));
            GameObject spawned = Instantiate(exampleCard, hostPosition, hostRotation);
            spawned.tag = "HostCard";
            fieldHostObjectsList[index] = spawned;
        }
        else
        {
            CardLocation location = (CardLocation)index;
            Vector3 clientPosition = dictionary[location];
            Quaternion clientRotation = Quaternion.Euler(new Vector3(-90f, -90f, 0f));
            GameObject spawned = Instantiate(exampleCard, clientPosition, clientRotation);
            spawned.tag = "HostCard";
            fieldHostObjectsList[index] = spawned;
        }
    }

    [ClientRpc]
    private void SpawnClientCardClientRpc(int index, int cardId)
    {
        if (IsHost)
        {
            CardLocation location = (CardLocation)index;
            Vector3 hostPosition = dictionary[location];
            Quaternion hostRotation = Quaternion.Euler(new Vector3(-90f, 90f, 0f));
            GameObject spawned = Instantiate(exampleCard, hostPosition, hostRotation);
            spawned.tag = "ClientCard";
            fieldClientObjectsList[index] = spawned;
        }
        else
        {
            CardLocation location = (CardLocation)index;
            Vector3 clientPosition = dictionary[location];
            Quaternion clientRotation = Quaternion.Euler(new Vector3(-90f, -90f, 0f));
            GameObject spawned = Instantiate(exampleCard, clientPosition, clientRotation);
            spawned.tag = "ClientCard";
            fieldClientObjectsList[index] = spawned;
        }
    }

    [ClientRpc]
    private void DespawnFieldCardClientRpc(int index)
    {
        if (fieldHostObjectsList[index] != null)
        {
            Destroy(fieldHostObjectsList[index]);
            fieldHostObjectsList[index] = null;
        }
        else if (fieldClientObjectsList[index] != null)
        {
            Destroy(fieldClientObjectsList[index]);
            fieldClientObjectsList[index] = null;
        }
    }

    [ClientRpc]
    private void MoveHostCardClientRpc(int from, int to)
    {

    }

    [ClientRpc]
    private void MoveClientCardClientRpc(int from, int to)
    {

    }

    // Static methods

    private static int FindFirstEmptyIndex(NetworkList<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == 0) return i;
            else if (list[list.Count - 1] != 0) return -1;
        }
        return -1;
    }

    private static int FindFirstEmptyIndex(List<GameObject> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == null) return i;
            else if (list[list.Count - 1] != null) return -1;
        }
        return -1;
    }

    private static Vector3 FindPositionOfIndex(int isHost, int index)
    {
        if (isHost == 1)
        {
            if (index == 0) return new Vector3(0f, 15.03f, 25f);
            else if (index == 1) return new Vector3(3f, 15.02f, 25f);
            else if (index == 2) return new Vector3(-3f, 15.04f, 25f);
            else if (index == 3) return new Vector3(6f, 15.01f, 25f);
            else if (index == 4) return new Vector3(-6f, 15.05f, 25f);
            else return Vector3.zero;
        }
        else
        {
            if (index == 0) return new Vector3(0f, 15.03f, -25f);
            else if (index == 1) return new Vector3(3f, 15.04f, -25f);
            else if (index == 2) return new Vector3(-3f, 15.02f, -25f);
            else if (index == 3) return new Vector3(6f, 15.05f, -25f);
            else if (index == 4) return new Vector3(-6f, 15.01f, -25f);
            else return Vector3.zero;
        }
    }

    private static void ReorderObjectList(int isHost, List<GameObject> list)
    {
        if (list.Count == 0) return;

        bool nullFound = false;

        for (int i = 0; i < list.Count; i++)
        {
            if (nullFound && list[i] != null)
            {
                Vector3 pos = FindPositionOfIndex(isHost, i - 1);
                list[i - 1] = list[i];
                list[i - 1].transform.position = pos;
                list[i - 1].GetComponent<CardIndexer>().SetIndex(i - 1);
                list[i] = null;
            }
            else if (list[i] == null) nullFound = true;
        }
    }
    
    private static void ReorderNetworkList(NetworkList<int> list)
    {
        bool zeroFound = false;

        for (int i = 0; i < list.Count; i++)
        {
            if (zeroFound && list[i] != 0)
            {
                list[i - 1] = list[i];
                list[i] = 0;
            }
            else if (list[i] == 0) zeroFound = true;
        }
    }
}
