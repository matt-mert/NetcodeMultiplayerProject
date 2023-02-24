using Unity.Netcode;
using UnityEngine;

public class CardManager : NetworkBehaviour
{
    public static CardManager Instance { get; private set; }

    public delegate void HostDrawCard();
    public event HostDrawCard OnHostDrawCard;
    public delegate void ClientDrawCard();
    public event ClientDrawCard OnClientDrawCard;

    public NetworkList<int> HostCardsList;

    [SerializeField]
    private GameDeck gameDeck;

    // Debug

    [SerializeField]
    private GameObject exampleCard;
    [SerializeField]
    private GameObject exampleGenericCard;
    [SerializeField]
    private GameObject exampleNetworkCard;
    [SerializeField]
    private GameCard gameCardSO;
    [SerializeField]
    private GameCard genericCardSO;

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

        HostCardsList = new NetworkList<int>();
    }

    [ClientRpc]
    public void HostDrawCardClientRpc()
    {
        Vector3 position = new Vector3(-3f, 15.01f, 25f);
        Quaternion rotation = Quaternion.Euler(new Vector3(-90, 90, 0));
        if (IsHost)
        {
            GameObject spawnedCard = Instantiate(exampleCard, position, rotation);
            spawnedCard.GetComponent<CardHandler>().cardSO = gameCardSO;
        }
        else
        {
            Instantiate(exampleGenericCard, position, rotation);
        }
        if (OnHostDrawCard != null) OnHostDrawCard.Invoke();
    }

    [ClientRpc]
    public void ClientDrawCardClientRpc()
    {
        Vector3 position = new Vector3(3f, 15.01f, -25f);
        Quaternion rotation = Quaternion.Euler(new Vector3(-90, -90, 0));
        if (IsHost)
        {
            Instantiate(exampleGenericCard, position, rotation);
        }
        else
        {
            GameObject spawnedCard = Instantiate(exampleCard, position, rotation);
            spawnedCard.GetComponent<CardHandler>().cardSO = gameCardSO;
        }
        if (OnClientDrawCard != null) OnClientDrawCard.Invoke();
    }

    public void HostSpawnCard(int cardId, CardLocation location, Vector3 position)
    {
        Vector3 updatedPos = position + new Vector3(0, 0.1f, 0);
        GameObject spawnedCard = Instantiate(exampleNetworkCard, updatedPos, Quaternion.Euler(new Vector3(-90, 90, 0)));
        spawnedCard.GetComponent<NetworkCardHandler>().cardSO = gameCardSO;
        // Spawn with owner kullanilabilir
        spawnedCard.GetComponent<NetworkObject>().Spawn();
    }

    [ClientRpc]
    public void ClientSpawnCardClientRpc(int cardId, CardLocation location, Vector3 position)
    {
        Vector3 updatedPos = position + new Vector3(0, 0.1f, 0);
        GameObject spawnedCard = Instantiate(exampleNetworkCard, updatedPos, Quaternion.Euler(new Vector3(-90, -90, 0)));
        spawnedCard.GetComponent<NetworkCardHandler>().cardSO = gameCardSO;
        // Spawn with owner kullanilabilir
        spawnedCard.GetComponent<NetworkObject>().Spawn();
    }
}
