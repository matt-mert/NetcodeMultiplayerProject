using Unity.Netcode;
using UnityEngine;

public class VisibilityHandler : NetworkBehaviour
{
    private enum Visibility
    {
        host,
        client
    }

    [SerializeField]
    private Visibility visibility;

    private NetworkObject networkObject;

    private void Awake()
    {
        networkObject = GetComponent<NetworkObject>();
    }

    public override void OnNetworkSpawn()
    {
        if (visibility == Visibility.host && IsHost)
        {
            networkObject.CheckObjectVisibility = (OwnerClientId) =>
            {
                return true;
            };
        }
        else if (visibility == Visibility.client && !IsHost)
        {
            networkObject.CheckObjectVisibility = (OwnerClientId) =>
            {
                return true;
            };
        }
    }
}
