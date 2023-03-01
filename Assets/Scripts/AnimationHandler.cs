using UnityEngine;

// Written by https://github.com/matt-mert

public class AnimationHandler : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void FinishAnimation()
    {
        animator.enabled = false;
    }
}
