using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "My Assets/Card Data")]
public class GameCard : ScriptableObject
{
    public string cardName;
    public string cardDesc;
    public string cardType;
    public Material cardMaterial;
    public int spawnEnergy;
    public int moveEnergy;
    public int cardPower;
    public int cardHealth;
}
