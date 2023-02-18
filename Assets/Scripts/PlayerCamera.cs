using Cinemachine;
using UnityEngine;

public class PlayerCamera : Singleton<PlayerCamera>
{
    private CinemachineVirtualCamera cmvc;

    private void Awake()
    {
        cmvc = GetComponent<CinemachineVirtualCamera>();    
    }

    public void AdjustAngle()
    {
        cmvc.transform.Rotate(new Vector3(0, 0, 180));
    }
}
