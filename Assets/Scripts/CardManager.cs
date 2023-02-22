using Unity.Netcode;
using UnityEngine;

public class CardManager : NetworkBehaviour
{
    public static CardManager Instance { get; private set; }

    [SerializeField]
    private GameDeck gameDeck;

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

    public void PlayCardAtPosition()
    {

    }
}
