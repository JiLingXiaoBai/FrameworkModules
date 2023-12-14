using System.Collections.Generic;
using UnityEngine;

namespace Tools.Config
{
    [System.Serializable]
    public class ConfigDataBase
    {
    }

    public class ConfigDataTable<TKey, TValue> : ScriptableObject, ISerializationCallbackReceiver
        where TValue : ConfigDataBase
    {
        private Dictionary<TKey, TValue> _config;

        [SerializeField] private TKey[] keys;

        [SerializeField] private TValue[] values;

        public void OnBeforeSerialize()
        {
            if (_config == null) return;
            keys = new TKey[_config.Count];
            values = new TValue[_config.Count];

            var index = 0;
            foreach (var item in _config)
            {
                keys[index] = item.Key;
                values[index] = item.Value;
                index++;
            }
        }

        public void OnAfterDeserialize()
        {
            if (keys == null) return;
            _config = new Dictionary<TKey, TValue>();
            var count = keys.Length;
            for (var i = 0; i < count; i++)
            {
                _config[keys[i]] = values[i];
            }

            keys = null;
            values = null;
        }

        public IReadOnlyDictionary<TKey, TValue> Config => _config;

        public int Count => _config.Count;
    }
}