using System.Collections;
using UnityEngine.Internal;
using UnityEngine;
using UnityEngine.Events;

namespace Tools
{
    public class MonoMgr : MonoSingleton<MonoMgr>
    {
        private event UnityAction UpdateEvent;
        private event UnityAction DestroyEvent;
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

        public void AddUpdateListener(UnityAction func)
        {
            UpdateEvent += func;
        }
        public void RemoveUpdateListener(UnityAction func)
        {
            UpdateEvent -= func;
        }

        public void AddDestroyListener(UnityAction func)
        {
            DestroyEvent += func;
        }

        public void RemoveDestroyListener(UnityAction func)
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
