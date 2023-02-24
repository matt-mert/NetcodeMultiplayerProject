using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class NetworkCardHandler : NetworkBehaviour
{
    [SerializeField]
    private TextMeshProUGUI cardNameText;
    [SerializeField]
    private TextMeshProUGUI cardDescText;
    [SerializeField]
    private TextMeshProUGUI attackText;
    [SerializeField]
    private TextMeshProUGUI healthText;
    [SerializeField]
    private TextMeshProUGUI spawnEnergyText;
    [SerializeField]
    private TextMeshProUGUI moveEnergyText;
    [SerializeField]
    private MeshRenderer meshRenderer;

    [SerializeField]
    public GameCard cardSO;

    private GenericUnitCard genericUnitCard;

    public override void OnNetworkSpawn()
    {
        cardNameText.text = cardSO.cardName;
        cardDescText.text = cardSO.cardDesc;
        attackText.text = cardSO.cardPower.ToString();
        healthText.text = cardSO.cardHealth.ToString();
        spawnEnergyText.text = cardSO.spawnEnergy.ToString();
        moveEnergyText.text = cardSO.moveEnergy.ToString();
        meshRenderer.material = cardSO.cardMaterial;

        genericUnitCard = GetComponent<GenericUnitCard>();
        genericUnitCard.OnIncreaseAttack += UpdateAttackUIClientRpc;
        genericUnitCard.OnDecreaseAttack += UpdateAttackUIClientRpc;
        genericUnitCard.OnTakeDamage += UpdateHealthUIClientRpc;
        genericUnitCard.OnGetHealed += UpdateHealthUIClientRpc;
        genericUnitCard.OnIncreaseMoveEnergy += UpdateMoveEnergyUIClientRpc;
        genericUnitCard.OnDecreaseMoveEnergy += UpdateMoveEnergyUIClientRpc;
        genericUnitCard.OnIncreaseSpawnEnergy += UpdateSpawnEnergyUIClientRpc;
        genericUnitCard.OnDecreaseSpawnEnergy += UpdateSpawnEnergyUIClientRpc;
    }

    public override void OnNetworkDespawn()
    {
        genericUnitCard.OnIncreaseAttack -= UpdateAttackUIClientRpc;
        genericUnitCard.OnDecreaseAttack -= UpdateAttackUIClientRpc;
        genericUnitCard.OnTakeDamage -= UpdateHealthUIClientRpc;
        genericUnitCard.OnGetHealed -= UpdateHealthUIClientRpc;
        genericUnitCard.OnIncreaseMoveEnergy -= UpdateMoveEnergyUIClientRpc;
        genericUnitCard.OnDecreaseMoveEnergy -= UpdateMoveEnergyUIClientRpc;
        genericUnitCard.OnIncreaseSpawnEnergy -= UpdateSpawnEnergyUIClientRpc;
        genericUnitCard.OnDecreaseSpawnEnergy -= UpdateSpawnEnergyUIClientRpc;
    }

    [ClientRpc]
    public void UpdateAttackUIClientRpc(int prev, int next)
    {
        attackText.text = next.ToString();
    }

    [ClientRpc]
    public void UpdateHealthUIClientRpc(int prev, int next)
    {
        healthText.text = next.ToString();
    }

    [ClientRpc]
    public void UpdateSpawnEnergyUIClientRpc(int prev, int next)
    {
        spawnEnergyText.text = next.ToString();
    }

    [ClientRpc]
    public void UpdateMoveEnergyUIClientRpc(int prev, int next)
    {
        moveEnergyText.text = next.ToString();
    }
}
