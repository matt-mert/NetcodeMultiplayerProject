using UnityEngine;

public class AdjustFPS : MonoBehaviour
{
    [SerializeField]
    private int targetFPS = 15;

    private void Awake()
    {
        Application.targetFrameRate = targetFPS;
    }
}
