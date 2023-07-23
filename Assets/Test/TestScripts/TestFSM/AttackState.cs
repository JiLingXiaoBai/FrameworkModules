using System;
using System.Collections.Generic;
using JLXB.Framework.FSM;
using UnityEngine;


public class AttackState : StateBase<SuperState>
{
    private Animator anim;
    public AttackState(Animator anim, bool needsExitTime, bool isGhostState = false) : base(needsExitTime, isGhostState)
    {
        this.anim = anim;
    }

    public override void OnEnter()
    {
        anim.Play("Attack");
    }
}