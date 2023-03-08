using UnityEngine;

// Written by https://github.com/matt-mert

public abstract class GenericUnitCard : MonoBehaviour
{
    public delegate void IncreaseHealth(int prev, int next);
    public event IncreaseHealth OnIncreaseHealth;
    public delegate void DecreaseHealth(int prev, int next);
    public event DecreaseHealth OnDecreaseHealth;
    public delegate void IncreaseAttack(int prev, int next);
    public event IncreaseAttack OnIncreaseAttack;
    public delegate void DecreaseAttack(int prev, int next);
    public event DecreaseAttack OnDecreaseAttack;
    public delegate void IncreaseMoveEnergy(int prev, int next);
    public event IncreaseMoveEnergy OnIncreaseMoveEnergy;
    public delegate void DecreaseMoveEnergy(int prev, int next);
    public event DecreaseMoveEnergy OnDecreaseMoveEnergy;
    public delegate void IncreaseSpawnEnergy(int prev, int next);
    public event IncreaseSpawnEnergy OnIncreaseSpawnEnergy;
    public delegate void DecreaseSpawnEnergy(int prev, int next);
    public event DecreaseSpawnEnergy OnDecreaseSpawnEnergy;

    public GameCard cardSO;

    [HideInInspector]
    public int attack;
    [HideInInspector]
    public int health;
    [HideInInspector]
    public int spawnEnergy;
    [HideInInspector]
    public int moveEnergy;

    private void Awake()
    {
        attack = cardSO.cardPower;
        health = cardSO.cardHealth;
        spawnEnergy = cardSO.spawnEnergy;
        moveEnergy = cardSO.moveEnergy;
    }

    public virtual void IncreaseHealthClientRpc(int amount)
    {
        int prev = health;
        health += amount;
        if (OnIncreaseHealth != null) OnIncreaseHealth.Invoke(prev, health);
    }

    public virtual void DecreaseHealthClientRpc(int amount)
    {
        int prev = health;
        health -= amount;
        if (OnDecreaseHealth != null) OnDecreaseHealth.Invoke(prev, health);
    }

    public virtual void IncreaseAttackClientRpc(int amount)
    {
        int prev = attack;
        attack += amount;
        if (OnIncreaseAttack != null) OnIncreaseAttack.Invoke(prev, attack);
    }

    public virtual void DecreaseAttackClientRpc(int amount)
    {
        int prev = attack;
        attack -= amount;
        if (OnDecreaseAttack != null) OnDecreaseAttack.Invoke(prev, attack);
    }

    public virtual void IncreaseMoveEnergyClientRpc(int amount)
    {
        int prev = moveEnergy;
        moveEnergy += amount;
        if (OnIncreaseMoveEnergy != null) OnIncreaseMoveEnergy.Invoke(prev, moveEnergy);
    }

    public virtual void DecreaseMoveEnergyClientRpc(int amount)
    {
        int prev = moveEnergy;
        moveEnergy -= amount;
        if (OnDecreaseMoveEnergy != null) OnDecreaseMoveEnergy.Invoke(prev, moveEnergy);
    }

    public virtual void IncreaseSpawnEnergyClientRpc(int amount)
    {
        int prev = spawnEnergy;
        spawnEnergy += amount;
        if (OnIncreaseSpawnEnergy != null) OnIncreaseSpawnEnergy.Invoke(prev, spawnEnergy);
    }

    public virtual void DecreaseSpawnEnergyClientRpc(int amount)
    {
        int prev = spawnEnergy;
        spawnEnergy += amount;
        if (OnDecreaseSpawnEnergy != null) OnDecreaseSpawnEnergy.Invoke(prev, spawnEnergy);
    }
}
