using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JLXB.Framework.FSM;
using JLXB.Framework.Timer;

public class SitState : StateBase<CharacterState>
{
    private Animator anim;
    private float sitTime;
    private Timer timer;
    public SitState(Animator anim, float sitTime, bool needsExitTime, bool isGhostState = false) : base(needsExitTime, isGhostState)
    {
        this.anim = anim;
        this.sitTime = sitTime;
    }

    public override void OnEnter()
    {
        anim.Play("Sit");
        timer = TimerUtils.TimerOnce(sitTime, () => fsm.StateCanExit());
    }
}
