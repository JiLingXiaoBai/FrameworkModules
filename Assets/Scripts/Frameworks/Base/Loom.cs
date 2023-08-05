using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace JLXB.Framework
{
    public class Loom : Singleton<Loom>
    {
        private struct DelayedQueultem
        {
            public float time;
            public Action<object> action;
            public object param;
        }

        private struct NoDelayedQueultem
        {
            public Action<object> action;
            public object param;
        }

        private readonly List<NoDelayedQueultem> _actions = new();
        private readonly List<NoDelayedQueultem> _currentActions = new();
        private readonly List<DelayedQueultem> _delayeds = new();
        private readonly List<DelayedQueultem> _currentDelayeds = new();

        public static int maxThreads = 8;
        private static int numThreads;

        private Loom()
        {
            MonoMgr.Instance.AddUpdateListener(UpdateAction);
        }

        public static void QueueOnMainThread(Action<object> action)
        {
            QueueOnMainThread(action, null, 0f);
        }

        public static void QueueOnMainThread(Action<object> action, object param)
        {
            QueueOnMainThread(action, param, 0f);
        }

        public static void QueueOnMainThread(Action<object> vAction, object vParam, float vTime)
        {
            if (vTime != 0)
            {
                lock (Instance._delayeds)
                {
                    Instance._delayeds.Add(new DelayedQueultem { time = Time.time + vTime, action = vAction });
                }
            }
            else
            {
                lock (Instance._actions)
                {
                    Instance._actions.Add(new NoDelayedQueultem { action = vAction, param = vParam });
                }
            }
        }

        public static Thread RunAsync(Action action)
        {
            if (Instance == null) return null;

            while (numThreads >= maxThreads)
            {
                Thread.Sleep(1);
            }
            Interlocked.Increment(ref numThreads);
            ThreadPool.QueueUserWorkItem(RunAction, action);
            return null;
        }

        private static void RunAction(object action)
        {
            try
            {
                ((Action)action)();
            }
            catch { }
            finally { Interlocked.Decrement(ref numThreads); }
        }

        private void UpdateAction()
        {
            if (_actions.Count > 0)
            {
                lock (_actions)
                {
                    _currentActions.Clear();
                    _currentActions.AddRange(_actions);
                    _actions.Clear();
                }
                for (int i = 0; i < _currentActions.Count; i++)
                {
                    _currentActions[i].action(_currentActions[i].param);
                }
            }
            if (_delayeds.Count > 0)
            {
                lock (_delayeds)
                {
                    _currentDelayeds.Clear();
                    _currentDelayeds.AddRange(_delayeds.Where(d => d.time <= Time.time));
                    for (int i = 0; i < _currentDelayeds.Count; i++)
                    {
                        _delayeds.Remove(_currentDelayeds[i]);
                    }
                }
                for (int i = 0; i < _currentDelayeds.Count; i++)
                {
                    _currentDelayeds[i].action(_currentDelayeds[i].param);
                }
            }
        }
    }
}
