using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    [SerializeField]
    private Animator northAnimator;
    [SerializeField]
    private Animator southAnimator;

    private void Awake()
    {
        GameStates.Instance.OnStateChangedToStart += InitializeLights;
    }

    private void InitializeLights()
    {
        northAnimator.SetTrigger("OnNorthLightAnim");
        southAnimator.SetTrigger("OnSouthLightAnim");
    }
}
