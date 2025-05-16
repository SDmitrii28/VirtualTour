using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager Instance { get; private set; }
    [SerializeField] private Animator animator;

    private StateMachine stateMachine;

    private IdleState idleState;
    private WalkingState walkingState;
    private TalkingState talkingState;

private void Awake()
{
    if (Instance == null)
    {
        Instance = this;
        DontDestroyOnLoad(gameObject); 
    }
    else
    {
        Destroy(gameObject);
    }
}

    private void Start()
    {
        stateMachine = new StateMachine();

        idleState = new IdleState(animator);
        walkingState = new WalkingState(animator);
        talkingState = new TalkingState(animator);

        stateMachine.ChangeState(idleState);
    }

    private void Update()
    {
        stateMachine.Update();
    }

    public void PlayIdleAnimation()
    {
        stateMachine.ChangeState(idleState);
    }

    public void PlayWalkingAnimation()
    {
        stateMachine.ChangeState(walkingState);
    }

    public void PlayTalkingAnimation()
    {
        stateMachine.ChangeState(talkingState);
    }
}
