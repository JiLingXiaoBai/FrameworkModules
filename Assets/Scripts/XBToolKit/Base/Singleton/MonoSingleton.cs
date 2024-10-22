using UnityEngine;
using System.Reflection;
using System.Linq;
using System;

namespace XBToolKit
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance = null;

        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = FindFirstObjectByType<T>();
                if (_instance == null)
                {
                    GameObject obj = new(typeof(T).Name);
                    _instance = obj.AddComponent<T>();
                    DontDestroyOnLoad(obj);
                }
                else
                {
                    DontDestroyOnLoad(_instance.gameObject);
                }

                return _instance;
            }
        }

        protected MonoSingleton()
        {
            var constructorInfos =
                typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (constructorInfos.Count() != 1)
                throw new InvalidOperationException($"Type {typeof(T)} must have exactly one constructor.");
            var ctor = constructorInfos.SingleOrDefault(c => !c.GetParameters().Any() && c.IsPrivate) ??
                       throw new InvalidOperationException(
                           $"The constructor for {typeof(T)} must be private and take no parameters.");
        }
    }
}