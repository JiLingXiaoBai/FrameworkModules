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
        public void LoadAssetAsync<T>(string key, UnityAction<T> callback, bool autoRelease = true) where T : UnityEngine.Object
        {
            var handle = Addressables.LoadAssetAsync<T>(key);
            handle.Completed += (handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    callback?.Invoke(handle.Result);
                }

                if (autoRelease)
                {
                    Addressables.Release(handle);
                }
            };
        }

        public void LoadAssetsByLabel<T>(string label, Action<T> callback = null) where T : UnityEngine.Object
        {
            Addressables.LoadAssetsAsync<T>(label, callback).Completed += (handle) =>
            {
                Addressables.Release(handle);
            };
        }

        /// <summary>
        /// If autoRelease is false, use this to release asset manually
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void ReleaseAsset<T>(ref T obj) where T : class
        {
            if (obj != null)
                Addressables.Release(obj);
        }

        public void LoadSceneAsync(string sceneName, LoadSceneMode loadMode, UnityAction loadAction = null, UnityAction finishAction = null, bool autoRelease = true)
        {
            var coroutine = WaitForLoading(sceneName, loadMode, loadAction, finishAction, autoRelease);
            MonoMgr.Instance.StartCoroutine(coroutine);
        }

        private IEnumerator WaitForLoading(string sceneName, LoadSceneMode loadMode, UnityAction loadAction, UnityAction finishAction, bool autoRelease)
        {
            var handle = Addressables.LoadSceneAsync(sceneName, loadMode);

            while (handle.Status == AsyncOperationStatus.None)
            {
                //进度条显示等等
                loadAction?.Invoke();
                yield return null;
            }

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                finishAction?.Invoke();
            }

            if (autoRelease)
            {
                Addressables.UnloadSceneAsync(handle);
            }

            yield return null;
        }

        /// <summary>
        /// If autoRelease is false, use this to release scene manually
        /// </summary>
        public void UnloadSceneAsync(SceneInstance scene)
        {
            Addressables.UnloadSceneAsync(scene);
        }

        public void InstantiateAsync(string key, UnityAction<GameObject> callback = null)
        {
            var handle = Addressables.InstantiateAsync(key);
            handle.Completed += (handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    callback?.Invoke(handle.Result);
                }
            };
        }

        public void InstantiateAsync(string key, Vector3 position, Quaternion rotation, Transform parent = null, UnityAction<GameObject> callback = null)
        {
            var handle = Addressables.InstantiateAsync(key, position, rotation, parent);
            handle.Completed += (handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    callback?.Invoke(handle.Result);
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
