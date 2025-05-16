using UnityEngine;

public class TalkingState : IState
{
    private Animator animator;

    public TalkingState(Animator animator)
    {
        this.animator = animator;
    }

    public void Enter()
    {
        animator.SetTrigger("Talking");
    }

    public void Execute() { }

    public void Exit() { }
}
