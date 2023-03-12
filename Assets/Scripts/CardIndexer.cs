using UnityEngine;

// Written by https://github.com/matt-mert

public class CardIndexer : MonoBehaviour
{
    private int cardHandIndex;
    private int cardFieldIndex;

    public int GetHandIndex()
    {
        return cardHandIndex;
    }

    public void SetHandIndex(int value)
    {
        cardHandIndex = value;
    }

    public int GetFieldIndex()
    {
        return cardFieldIndex;
    }

    public void SetFieldIndex(int value)
    {
        cardFieldIndex = value;
    }
}
