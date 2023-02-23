using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CardManager : NetworkBehaviour
{
    public static CardManager Instance { get; private set; }

    public delegate void HostDrawCard();
    public event HostDrawCard OnHostDrawCard;
    public delegate void ClientDrawCard();
    public event ClientDrawCard OnClientDrawCard;

    [SerializeField]
    private GameDeck gameDeck;

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

    public Dictionary<CardLocation, int> fieldDictionary;

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

    [ClientRpc]
    public void HostDrawCardClientRpc()
    {
        if (OnHostDrawCard != null) OnHostDrawCard.Invoke();
    }

    [ClientRpc]
    public void ClientDrawCardClientRpc()
    {
        if (OnClientDrawCard != null) OnClientDrawCard.Invoke();
    }
}
