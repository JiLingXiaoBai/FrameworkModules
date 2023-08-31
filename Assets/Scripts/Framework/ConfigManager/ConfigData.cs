using System.Collections.Generic;
using UnityEngine;

namespace JLXB.Framework.Config
{
    [System.Serializable]
    public class ConfigDataBase
    {
        
    }

    public class ConfigDataTable<TKey,TData> : ScriptableObject where TData : ConfigDataBase
    {
        [SerializeField]
        public Dictionary<TKey, TData> config;

        public int Count => config.Count;
    }
}