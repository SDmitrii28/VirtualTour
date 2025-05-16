using UnityEngine;

public class GreetingState : IState
{
    private Animator animator;

    public GreetingState(Animator animator)
    {
        this.animator = animator;
    }

    public void Enter()
    {
        animator.SetTrigger("Greeting");
    }

    public void Execute() { }

    public void Exit() { }
}
