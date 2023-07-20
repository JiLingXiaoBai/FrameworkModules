using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JLXB.Framework.FSM
{
    public sealed class FSMController : MonoBehaviour
    {
        private class StateData
        {
            public IState State;
            public bool IsRunning = false;
            public StateData(IState state, bool isRunning)
            {
                State = state;
                IsRunning = isRunning;
            }
        }

        [SerializeField] string defaultState = String.Empty;

        private Dictionary<Type, StateData> states = new Dictionary<Type, StateData>();

        public bool IsRunning<T>() where T : IState => IsRunning(typeof(T));
        public void Enter<T>() where T : IState => Enter(typeof(T));
        public void Exit<T>() where T : IState => Exit(typeof(T));

        bool IsRunning(Type type)
        {
            if (states.ContainsKey(type))
            {
                if (states[type].IsRunning) return true;
            }
            return false;
        }

        void Enter(Type type)
        {

        }

        void Exit(Type type)
        {

        }

        /// <summary>
        /// 不存在该状态时则添加并初始化
        /// </summary>
        /// <param name="type"></param>
        void CheckAddState(Type type)
        {
            if (!states.ContainsKey(type))
            {
                var st = (IState)Activator.CreateInstance(type);
                st.Machine = this;
                st.OnInit();
                states.Add(type, new StateData(st, false));
            }
        }
    }
}
