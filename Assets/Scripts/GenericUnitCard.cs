using Unity.Netcode;
using UnityEngine;

public abstract class GenericUnitCard : NetworkBehaviour
{
    public delegate void TakeDamage(int prev, int next);
    public event TakeDamage OnTakeDamage;
    public delegate void GetHealed(int prev, int next);
    public event GetHealed OnGetHealed;
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

    public int attack;
    public int health;
    public int spawnEnergy;
    public int moveEnergy;

    public override void OnNetworkSpawn()
    {
        attack = cardSO.cardPower;
        health = cardSO.cardHealth;
        spawnEnergy = cardSO.spawnEnergy;
        moveEnergy = cardSO.moveEnergy;
    }

    [ClientRpc]
    public virtual void TakeDamageClientRpc(int amount)
    {
        int prev = health;
        health -= amount;
        if (OnTakeDamage != null) OnTakeDamage.Invoke(prev, health);
    }

    [ClientRpc]
    public virtual void GetHealedClientRpc(int amount)
    {
        int prev = health;
        health += amount;
        if (OnGetHealed != null) OnGetHealed.Invoke(prev, health);
    }

    [ClientRpc]
    public virtual void IncreaseAttackClientRpc(int amount)
    {
        int prev = attack;
        attack += amount;
        if (OnIncreaseAttack != null) OnIncreaseAttack.Invoke(prev, attack);
    }

    [ClientRpc]
    public virtual void DecreaseAttackClientRpc(int amount)
    {
        int prev = attack;
        attack -= amount;
        if (OnDecreaseAttack != null) OnDecreaseAttack.Invoke(prev, attack);
    }

    [ClientRpc]
    public virtual void IncreaseMoveEnergyClientRpc(int amount)
    {
        int prev = moveEnergy;
        moveEnergy += amount;
        if (OnIncreaseMoveEnergy != null) OnIncreaseMoveEnergy.Invoke(prev, moveEnergy);
    }

    [ClientRpc]
    public virtual void DecreaseMoveEnergyClientRpc(int amount)
    {
        int prev = moveEnergy;
        moveEnergy -= amount;
        if (OnDecreaseMoveEnergy != null) OnDecreaseMoveEnergy.Invoke(prev, moveEnergy);
    }

    [ClientRpc]
    public virtual void IncreaseSpawnEnergyClientRpc(int amount)
    {
        int prev = spawnEnergy;
        spawnEnergy += amount;
        if (OnIncreaseSpawnEnergy != null) OnIncreaseSpawnEnergy.Invoke(prev, spawnEnergy);
    }

    [ClientRpc]
    public virtual void DecreaseSpawnEnergyClientRpc(int amount)
    {
        int prev = spawnEnergy;
        spawnEnergy += amount;
        if (OnDecreaseSpawnEnergy != null) OnDecreaseSpawnEnergy.Invoke(prev, spawnEnergy);
    }

    [ClientRpc]
    public virtual void SpawnFromHandClientRpc()
    {

    }
}
