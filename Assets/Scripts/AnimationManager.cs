using Unity.Netcode;
using UnityEngine;

// Written by https://github.com/matt-mert

public class AnimationManager : NetworkBehaviour
{
    public AnimationManager Instance { get; private set; }

    [SerializeField]
    private Animator northAnimator;
    [SerializeField]
    private Animator southAnimator;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void OnNetworkSpawn()
    {
        StatesManager.Instance.OnStateChangedToStart += InitializeLightsClientRpc;
    }

    [ClientRpc]
    private void InitializeLightsClientRpc()
    {
        northAnimator.SetTrigger("OnNorthLightAnim");
        southAnimator.SetTrigger("OnSouthLightAnim");
    }
}
