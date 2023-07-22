using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JLXB.Framework.FSM;
using JLXB.Framework.Event;

public enum CharacterState
{
    Idle,
    Run,
    Sit,
}

public class TestFSM : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private float idleTime = 2f;
    [SerializeField] private Transform[] wayPoints;
    [SerializeField] private float speed = 2f;

    private StateMachine<CharacterState> fsm;
    private int currentIndex = 0;

    private void OnEnable()
    {
        EventCenter.Register<int>("TestFSM.ChangeDirection", OnChangeDirection);
    }

    private void Start()
    {
        fsm = new StateMachine<CharacterState>();
        fsm.AddState(CharacterState.Idle, new IdleState(anim, idleTime, true));
        fsm.AddState(CharacterState.Run, new RunState(anim, wayPoints, anim.transform, speed, false));
        fsm.AddTransition(CharacterState.Idle, CharacterState.Run);
        fsm.AddTransition(CharacterState.Run, CharacterState.Idle, transition => Vector2.Distance(anim.transform.position, wayPoints[currentIndex].position) < 0.1f);
        fsm.Init();
    }

    private void FixedUpdate()
    {
        fsm.OnLogic();
    }

    private void OnDisable()
    {
        EventCenter.Remove<int>("TestFSM.ChangeDirection", OnChangeDirection);
    }

    private void OnChangeDirection(int currentIndex)
    {
        this.currentIndex = currentIndex;
    }
}
