using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace JLXB.Framework.Config
{
    public class ConfigMgr : Singleton<ConfigMgr>
    {
        private ConfigMgr()
        {
        }

        private Dictionary<string, ScriptableObject> _configDict;

        private bool _initFlag;
        public bool ConfigLoadCompleted { get; private set; }

        public void Init()
        {
            if (_initFlag) return;
            _initFlag = true;
            _configDict = new Dictionary<string, ScriptableObject>();
            var handle =
                Addressables.LoadAssetsAsync<ScriptableObject>("ExcelConfigData",
                    so => { _configDict.TryAdd(so.name, so); });
            handle.Completed += (operationHandle) =>
            {
                if (operationHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    ConfigLoadCompleted = true;
                    Debug.Log("Excel Config Data Load Completed");
                }
                else
                {
                    ConfigLoadCompleted = false;
                    throw new Exception("Excel Config Data Load Error");
                }
            };
        }

        public IReadOnlyDictionary<TKey, TValue> GetConfig<TKey, TValue>(string configName)
            where TValue : ConfigDataBase
        {
            if (!ConfigLoadCompleted)
            {
                Debug.LogError("Excel config data has not been loaded");
                return null;
            }

            var hasConfig = _configDict.TryGetValue(configName, out var configObj);
            if (hasConfig)
            {
                return ((ConfigDataTable<TKey, TValue>)configObj).Config;
            }

            Debug.LogError($"There is no config data named {configName}");
            return null;
        }
    }
}