using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "My Assets/Card Data")]
public class GameCard : ScriptableObject
{
    public int cardId;
    public string cardName;
    public string cardDesc;
    public Material cardMaterial;
    public int spawnEnergy;
    public int moveEnergy;
    public int cardPower;
    public int cardHealth;

    public enum CardTypes
    {
        Unit,
        Action,
    }

    public CardTypes cardType;

    public enum Properties
    {
        None,
        Blitz,
        Fury,
    }

    public Properties property;
}
