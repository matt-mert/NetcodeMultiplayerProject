using UnityEngine;

public class CardIndexer : MonoBehaviour
{
    [SerializeField]
    private int cardIndex;

    public int GetIndex()
    {
        return cardIndex;
    }

    public void SetIndex(int value)
    {
        cardIndex = value;
    }
}
