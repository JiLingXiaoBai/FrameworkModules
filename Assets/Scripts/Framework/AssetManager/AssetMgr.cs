using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace JLXB.Framework.Asset
{
    public class AssetMgr : Singleton<AssetMgr>
    {
        private AssetMgr() { }
        private readonly Dictionary<string, AsyncOperationHandle> _caches = new();

        public void LoadAssetAsync<T>(string key, UnityAction<T> callback, bool autoRelease = true) where T : UnityEngine.Object
        {
            if (_caches.ContainsKey(key))
            {
                var handle = _caches[key];
                if (handle.IsDone)
                {
                    callback?.Invoke(_caches[key].Convert<T>().Result);
                }
                else
                {
                    handle.Completed += (operationHandle) =>
                    {
                        if (operationHandle.Status == AsyncOperationStatus.Succeeded)
                        {
                            callback?.Invoke(_caches[key].Convert<T>().Result);
                            if (autoRelease)
                                ReleaseAsset(key);
                        }
                        else
                        {
                            throw new Exception($"加载资源失败 key = {key}");
                        }
                    };
                }
            }
            else
            {
                var handle = Addressables.LoadAssetAsync<T>(key);
                handle.Completed += (operationHandle) =>
                {
                    if (operationHandle.Status == AsyncOperationStatus.Succeeded)
                    {
                        callback?.Invoke(operationHandle.Result);
                        if (autoRelease)
                            ReleaseAsset(key);
                    }
                    else
                    {
                        throw new Exception($"加载资源失败 key = {key}");
                    }

                };
                _caches.Add(key, handle);
            }
        }

        public void LoadAssetsByLabel<T>(string label, Action<T> callback, bool autoRelease = true) where T : UnityEngine.Object
        {
            if (_caches.ContainsKey(label))
            {
                var handle = _caches[label];
                if (handle.IsDone)
                {
                    var list = _caches[label].Convert<IList<T>>().Result;
                    foreach (var item in list)
                    {
                        callback?.Invoke(item);
                    }
                }
                else
                {
                    handle.Completed += (operationHandle) =>
                    {
                        if (operationHandle.Status == AsyncOperationStatus.Succeeded)
                        {
                            callback?.Invoke(_caches[label].Convert<T>().Result);
                            if (autoRelease)
                                ReleaseAsset(label);
                        }
                        else
                        {
                            throw new Exception($"加载资源失败 label = {label}");
                        }
                    };
                }
            }
            else
            {
                var handle = Addressables.LoadAssetsAsync(label, callback);
                handle.Completed += (operationHandle) =>
                {
                    if (operationHandle.Status == AsyncOperationStatus.Succeeded)
                    {
                        if (autoRelease)
                            ReleaseAsset(label);
                    }
                    else
                    {
                        throw new Exception($"加载资源失败 label = {label}");
                    }

                };
                _caches.Add(label, handle);
            }
        }

        public void ReleaseAsset(string key)
        {
            if (!_caches.ContainsKey(key)) return;
            Addressables.Release(_caches[key]);
            _caches.Remove(key);
        }

        public void LoadSceneAsync(string sceneName, LoadSceneMode loadMode, bool activateOnLoad, UnityAction<float> loadAction = null, UnityAction<SceneInstance> finishAction = null)
        {
            var coroutine = WaitForLoading(sceneName, loadMode, activateOnLoad, loadAction, finishAction);
            MonoMgr.Instance.StartCoroutine(coroutine);
        }

        private IEnumerator WaitForLoading(string sceneName, LoadSceneMode loadMode, bool activateOnLoad, UnityAction<float> loadAction, UnityAction<SceneInstance> finishAction)
        {
            var handle = Addressables.LoadSceneAsync(sceneName, loadMode, activateOnLoad);
            _caches.Add(sceneName, handle);

            while (handle.Status == AsyncOperationStatus.None)
            {
                //进度条显示等等
                loadAction?.Invoke(handle.PercentComplete);
                yield return null;
            }
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                finishAction?.Invoke(handle.Result);
            }
            else
            {
                throw new Exception($"加载场景失败，sceneName = {sceneName}");
            }
        }

        public void UnloadSceneAsync(string sceneName)
        {
            if (!_caches.ContainsKey(sceneName)) return;
            Addressables.UnloadSceneAsync(_caches[sceneName]);
            _caches.Remove(sceneName);
        }

        public void InstantiateAsync(string key, UnityAction<GameObject> callback = null)
        {
            var handle = Addressables.InstantiateAsync(key);
            handle.Completed += (operationHandle) =>
            {
                if (operationHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    callback?.Invoke(operationHandle.Result);
                }
            };
        }

        public void InstantiateAsync(string key, Vector3 position, Quaternion rotation, Transform parent = null, UnityAction<GameObject> callback = null)
        {
            var handle = Addressables.InstantiateAsync(key, position, rotation, parent);
            handle.Completed += (operationHandle) =>
            {
                if (operationHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    callback?.Invoke(operationHandle.Result);
                }
            };
        }

        /// <summary>
        /// 从内存中释放预制件并销毁其在当前活动场景中的实例。
        /// </summary>
        /// <param name="obj">要释放的预制件的游戏对象引用。</param>
        public void ReleaseInstance(ref GameObject obj)
        {
            if (obj != null)
                Addressables.ReleaseInstance(obj);
        }
    }
}
