using UnityEngine;

// Written by https://github.com/matt-mert

[CreateAssetMenu(fileName = "New Deck", menuName = "My Assets/Deck Data")]
public class GameDeck : ScriptableObject
{
    public string deckName;
    [SerializeField]
    public GameCard[] deck;
    public GameCard headquarters;
}
