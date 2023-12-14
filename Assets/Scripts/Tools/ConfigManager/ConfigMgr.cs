using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Tools.Config
{
    public class ConfigMgr : Singleton<ConfigMgr>
    {
        private ConfigMgr()
        {
            Init();
        }

        private Dictionary<string, ScriptableObject> _configDict;

        private bool _configLoadCompleted;

        private const string TypeEndStr = "+ConfigData";

        private void Init()
        {
            _configDict = new Dictionary<string, ScriptableObject>();
            var handle =
                Addressables.LoadAssetsAsync<ScriptableObject>("ExcelConfigData",
                    so => { _configDict.TryAdd(so.name, so); });
            handle.Completed += (operationHandle) =>
            {
                if (operationHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    _configLoadCompleted = true;
                    Debug.Log("Excel Config Data Load Completed");
                }
                else
                {
                    _configLoadCompleted = false;
                    throw new Exception("Excel Config Data Load Error");
                }
            };
            handle.WaitForCompletion();
        }

        public IReadOnlyDictionary<TKey, TValue> GetConfig<TKey, TValue>()
            where TValue : ConfigDataBase
        {
            if (!_configLoadCompleted)
            {
                Debug.LogError("Excel config data has not been loaded");
                return null;
            }
            var configName = typeof(TValue).ToString()[..^TypeEndStr.Length];
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