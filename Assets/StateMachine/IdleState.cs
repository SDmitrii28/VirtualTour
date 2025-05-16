using UnityEngine;

public class IdleState : IState
{
    private Animator animator;

    public IdleState(Animator animator)
    {
        this.animator = animator;
    }

    public void Enter()
    {
        animator.SetBool("IsWalking", false);
    }

    public void Execute() { }

    public void Exit() { }
}
