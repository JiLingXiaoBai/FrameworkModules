using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JLXB.Framework.FSM;
using JLXB.Framework.EventCenter;

public enum CharacterState
{
    Idle,
    Run,
    Sit,
}

public enum SuperState
{
    Day,
    Night,
    CharacterAttack,
    Any
}

public class TestFSM : MonoBehaviour
{
    [SerializeField] private Color dayColor;
    [SerializeField] private Color nightColor;

    [SerializeField] private Animator anim;
    [SerializeField] private float idleTime = 2f;
    [SerializeField] private float sitTime = 2f;
    [SerializeField] private Transform[] wayPoints;
    [SerializeField] private float speed = 2f;
    [SerializeField] private SpriteRenderer restZone;

    private StateMachine<SuperState, CharacterState, string> daySuperState;
    private StateMachine<SuperState, CharacterState, string> nightSuperState;
    private StateMachine<SuperState, string> fsm;
    private int currentIndex = 0;

    private void OnEnable()
    {
        EventCenter.Register<int>("TestFSM.ChangeDirection", OnChangeDirection);
        EventCenter.Register("TestFSM.EnterCollision", OnEnterCollision);
    }

    private void Start()
    {
        daySuperState = new StateMachine<SuperState, CharacterState, string>();
        daySuperState.AddState(CharacterState.Idle, new IdleState(anim, idleTime, true));
        daySuperState.AddState(CharacterState.Run, new RunState(anim, wayPoints, anim.transform, speed, false));
        daySuperState.AddTransition(CharacterState.Idle, CharacterState.Run);
        daySuperState.AddTransition(CharacterState.Run, CharacterState.Idle, transition =>
            Vector2.Distance(anim.transform.position, wayPoints[currentIndex].position) < 0.1f);

        nightSuperState = new StateMachine<SuperState, CharacterState, string>();
        nightSuperState.AddState(CharacterState.Idle, new IdleState(anim, idleTime, true));
        nightSuperState.AddState(CharacterState.Run, new RunState(anim, wayPoints, anim.transform, speed, false));
        nightSuperState.AddState(CharacterState.Sit, new SitState(anim, sitTime, true));
        nightSuperState.AddTransition(CharacterState.Idle, CharacterState.Run);
        nightSuperState.AddTransition(CharacterState.Run, CharacterState.Idle, transition =>
            Vector2.Distance(anim.transform.position, wayPoints[currentIndex].position) < 0.1f && !restZone.bounds.Contains(anim.transform.position));
        nightSuperState.AddTransition(CharacterState.Sit, CharacterState.Run);
        nightSuperState.AddTransition(CharacterState.Run, CharacterState.Sit, transition =>
            Vector2.Distance(anim.transform.position, wayPoints[currentIndex].position) < 0.1f && restZone.bounds.Contains(anim.transform.position));

        fsm = new StateMachine<SuperState, string>();
        fsm.AddState(SuperState.Day, daySuperState);
        fsm.AddState(SuperState.Night, nightSuperState);
        fsm.AddState(SuperState.CharacterAttack, new AttackState(anim, false));

        fsm.AddTriggerTransition("Day", new Transition<SuperState>(SuperState.Night, SuperState.Day));
        fsm.AddTriggerTransition("Night", new Transition<SuperState>(SuperState.Day, SuperState.Night));

        fsm.AddTriggerTransitionFromAny("Attack", new Transition<SuperState>(SuperState.Any, SuperState.CharacterAttack));

        fsm.Init();
    }

    private void FixedUpdate()
    {
        fsm.OnLogic();
    }

    private void OnDisable()
    {
        EventCenter.Remove<int>("TestFSM.ChangeDirection", OnChangeDirection);
        EventCenter.Remove("TestFSM.EnterCollision", OnEnterCollision);
    }

    private void OnChangeDirection(int currentIndex)
    {
        this.currentIndex = currentIndex;
    }

    private void OnEnterCollision()
    {
        fsm.Trigger("Attack");
    }

    public void OnChangeNight(bool isNight)
    {
        Camera.main.backgroundColor = isNight ? nightColor : dayColor;
        fsm.Trigger(isNight ? "Night" : "Day");
    }
}
