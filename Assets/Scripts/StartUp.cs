using UnityEngine;
using JLXB.Framework.LogSystem;
using JLXB.Framework.Asset;
using JLXB.Framework.Audio;
using JLXB.Framework.Config;
using UnityEngine.SceneManagement;
public class StartUp : MonoBehaviour
{
    private void Awake()
    {
        LogSystem.Instance.Init();
        Log.Info("StartUp");
        AudioMgr.Instance.Init();
        ConfigMgr.Instance.Init();
    }

    private void Start()
    {
        AssetMgr.Instance.LoadSceneAsync("SampleScene", LoadSceneMode.Single, false, null , sceneInstance =>
        {
            if (ConfigMgr.Instance.ConfigLoadCompleted)
            {
                sceneInstance.ActivateAsync();
            }
        });
    }
}
