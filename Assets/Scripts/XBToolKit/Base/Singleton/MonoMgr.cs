using System;
using System.Collections;
using UnityEngine.Internal;
using UnityEngine;

namespace XBToolKit
{
    public class MonoMgr : MonoSingleton<MonoMgr>
    {
        private event Action UpdateEvent;
        private event Action DestroyEvent;
        private MonoBehaviour _mono;
        private MonoMgr() { }
        private void Awake()
        {
            _mono = this;
        }

        private void Update()
        {
            UpdateEvent?.Invoke();
        }

        private void OnDestroy()
        {
            DestroyEvent?.Invoke();
            DestroyEvent = null;
            UpdateEvent = null;
        }

        public void AddUpdateListener(Action func)
        {
            UpdateEvent += func;
        }
        public void RemoveUpdateListener(Action func)
        {
            UpdateEvent -= func;
        }

        public void AddDestroyListener(Action func)
        {
            DestroyEvent += func;
        }

        public void RemoveDestroyListener(Action func)
        {
            DestroyEvent -= func;
        }

        public new Coroutine StartCoroutine(IEnumerator routine)
        {
            return _mono.StartCoroutine(routine);
        }

        public new Coroutine StartCoroutine(string methodName, [DefaultValue("null")] object value)
        {
            return _mono.StartCoroutine(methodName, value);
        }
        public new Coroutine StartCoroutine(string methodName)
        {
            return _mono.StartCoroutine(methodName);
        }
    }
}
