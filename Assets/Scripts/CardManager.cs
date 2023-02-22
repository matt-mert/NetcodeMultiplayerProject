using Unity.Netcode;
using UnityEngine;

public class CardManager : NetworkBehaviour
{
    public static CardManager Instance { get; private set; }

    [SerializeField]
    private GameDeck gameDeck;

    public delegate void HostDraw();
    public event HostDraw OnHostDraw;
    public delegate void ClientDraw();
    public event ClientDraw OnClientDraw;

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

    private void InitializeGame()
    {

    }

    public void HostDrawCards(int amount)
    {
        
        if (OnHostDraw == null) return;
        OnHostDraw.Invoke();
    }

    public void ClientDrawCards(int amount)
    {
        if (OnClientDraw == null) return;
        OnClientDraw.Invoke();
    }

    public void PlayCardAtPosition()
    {

    }
}
