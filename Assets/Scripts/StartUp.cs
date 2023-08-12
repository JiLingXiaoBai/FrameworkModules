using UnityEngine;
using JLXB.Framework.LogSystem;
using JLXB.Framework.Asset;
using UnityEngine.SceneManagement;
public class StartUp : MonoBehaviour
{
    private void Awake()
    {
        LogSystem.Instance.Init();
        Log.Info("StartUp");
        AudioMgr.Instance.Init();
    }

    private void Start()
    {
        AssetMgr.Instance.LoadSceneAsync("SampleScene", LoadSceneMode.Single, true);
    }
}
