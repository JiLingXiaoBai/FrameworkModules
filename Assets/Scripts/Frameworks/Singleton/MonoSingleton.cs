using UnityEngine;
using System.Reflection;
using System.Linq;
using System;

namespace JLXB.Framework
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance = null;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject(typeof(T).Name);
                        _instance = obj.AddComponent<T>();
                        DontDestroyOnLoad(obj);
                    }
                }

                return _instance;
            }
        }

        public MonoSingleton()
        {
            var ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (ctors.Count() != 1)
                throw new InvalidOperationException(String.Format("Type {0} must have exactly one constructor.", typeof(T)));
            var ctor = ctors.SingleOrDefault(c => !c.GetParameters().Any() && c.IsPrivate);
            if (ctor == null)
                throw new InvalidOperationException(String.Format("The constructor for {0} must be private and take no parameters.", typeof(T)));
        }
    }
}
