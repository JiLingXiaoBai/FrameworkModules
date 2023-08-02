using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace JLXB.Framework.Asset
{
    public class AssetMgr : Singleton<AssetMgr>
    {
        public void LoadAssetAsync<T>(string name, UnityAction<T> callback, bool autoRelease = true) where T : Object
        {
            var handle = Addressables.LoadAssetAsync<T>(name);
            handle.Completed += (handle) =>
            {
                OnLoadDone(handle, callback);
                if (autoRelease)
                {
                    Addressables.Release(handle.Result);
                }
            };
        }

        private void OnLoadDone<T>(AsyncOperationHandle<T> handle, UnityAction<T> callback)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded && callback != null)
            {
                callback(handle.Result);
            }
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
    }
}
