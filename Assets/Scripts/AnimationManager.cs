using UnityEngine;

public class AnimationManager : MonoBehaviour
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

        GameStates.Instance.OnStateChangedToStart += InitializeLights;
    }

    private void InitializeLights()
    {
        northAnimator.SetTrigger("OnNorthLightAnim");
        southAnimator.SetTrigger("OnSouthLightAnim");
    }
}
