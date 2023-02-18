using Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private CinemachineVirtualCamera cmvc;

    private void Awake()
    {
        cmvc = GetComponent<CinemachineVirtualCamera>();    
    }

    public void AdjustAngle()
    {
        
    }
}
