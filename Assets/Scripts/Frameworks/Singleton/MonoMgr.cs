using System.Collections;
using UnityEngine.Internal;
using UnityEngine;
using UnityEngine.Events;

namespace JLXB.Framework
{
    public class MonoMgr : MonoSingleton<MonoMgr>
    {
        private event UnityAction _updateEvent;
        private MonoBehaviour _mono;

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

        public void AddUpdateListener(UnityAction func)
        {
            _updateEvent += func;
        }
        public void RemoveUpdateListener(UnityAction func)
        {
            _updateEvent -= func;
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
