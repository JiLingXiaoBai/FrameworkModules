using System.Collections;
using UnityEngine.Internal;
using UnityEngine;
using UnityEngine.Events;

namespace JLXB.Framework
{
    public class MonoMgr : MonoSingleton<MonoMgr>
    {
        private event UnityAction _updateEvent;
        private event UnityAction _destroyEvent;
        private MonoBehaviour _mono;
        private MonoMgr() { }
        private void Awake()
        {
            _mono = this;
        }

        void Update()
        {
            if (_updateEvent != null)
            {
                _updateEvent();
            }
        }

        void OnDestroy()
        {
            if (_destroyEvent != null)
            {
                _destroyEvent();
            }
            _destroyEvent = null;
            _updateEvent = null;
        }

        public void AddUpdateListener(UnityAction func)
        {
            _updateEvent += func;
        }
        public void RemoveUpdateListener(UnityAction func)
        {
            _updateEvent -= func;
        }

        public void AddDestroyListener(UnityAction func)
        {
            _destroyEvent += func;
        }

        public void RemoveDestroyListener(UnityAction func)
        {
            _destroyEvent -= func;
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
