using Unity.Netcode;
using UnityEngine;

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
        GameStates.Instance.OnStateChangedToStart += InitializeLightsClientRpc;
    }

    [ClientRpc]
    private void InitializeLightsClientRpc()
    {
        northAnimator.SetTrigger("OnNorthLightAnim");
        southAnimator.SetTrigger("OnSouthLightAnim");
    }
}
