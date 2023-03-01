using UnityEngine;

// Written by https://github.com/matt-mert

public class CardIndexer : MonoBehaviour
{
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
