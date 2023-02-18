using UnityEngine;

[CreateAssetMenu(fileName = "New Deck", menuName = "My Assets/Deck Data")]
public class GameDeck : ScriptableObject
{
    public string deckName;
    [SerializeField]
    public GameCard[] deck;
}
