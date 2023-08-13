using System.Collections;
using System.Collections.Generic;
using JLXB.Framework.FSM;
using JLXB.Framework.EventCenter;
using UnityEngine;

public class RunState : StateBase<CharacterState>
{
    private Animator anim;
    private Transform[] wayPoints;
    private Transform characterTrans;
    private float speed;

    private int currentIndex = 0;
    private Vector3 direction;

    public RunState(
        Animator anim,
        Transform[] wayPoints,
        Transform characterTrans,
        float speed,
        bool needsExitTime,
        bool isGhostState = false) : base(needsExitTime, isGhostState)
    {
        this.anim = anim;
        this.wayPoints = wayPoints;
        this.characterTrans = characterTrans;
        this.speed = speed;
    }

    public override void OnEnter()
    {
        anim.Play("Run");
        direction = (wayPoints[currentIndex].position - characterTrans.position).normalized;
        characterTrans.localScale = new Vector3(direction.x < 0 ? 1f : -1f, 1f, 1f);
    }

    public override void OnLogic()
    {
        characterTrans.Translate(direction * (Time.fixedDeltaTime * speed));
    }

    public override void OnExit()
    {
        currentIndex = (currentIndex + 1) % wayPoints.Length;
        EventCenter.DispatchEvent<int>("TestFSM.ChangeDirection", currentIndex);
    }
}
