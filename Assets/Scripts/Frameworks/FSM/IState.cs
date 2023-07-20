using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JLXB.Framework.FSM
{
    public interface IState
    {
        public FSMController Machine { get; set; }
        public void OnInit();
        public void OnEnter();
        public void OnUpdate();
        public void OnFixedUpdate();
        public void OnExit();
        public void OnDestroy();
    }

    public abstract class StateBase : IState
    {
        public FSMController Machine { get; set; }
        public abstract void OnDestroy();
        public abstract void OnEnter();
        public abstract void OnExit();
        public abstract void OnFixedUpdate();
        public abstract void OnInit();
        public abstract void OnUpdate();
    }
}