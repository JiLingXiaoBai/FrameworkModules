using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JLXB.Framework.FSM;
using JLXB.Framework.Timer;

public class IdleState : StateBase<CharacterState>
{
    private Animator anim;
    private float idleTime;
    private Timer timer;

    public IdleState(Animator anim, float idleTime, bool needsExitTime, bool isGhostState = false) : base(needsExitTime, isGhostState)
    {
        this.anim = anim;
        this.idleTime = idleTime;
    }

    public override void OnEnter()
    {
        anim.Play("Idle");
        timer = TimerUtils.TimerOnce(idleTime, () => fsm.StateCanExit());
    }

}
