using UnityEngine;

public class WalkingState : IState
{
    private Animator animator;

    public WalkingState(Animator animator)
    {
        this.animator = animator;
    }

    public void Enter()
    {
        animator.SetBool("IsWalking", true);
    }

    public void Execute() { }

    public void Exit()
    {
        animator.SetBool("IsWalking", false);
    }
}
