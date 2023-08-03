using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JLXB.Framework.Asset;
using JLXB.Framework.Timer;
using UnityEngine.UI;

public class TestAsset : MonoBehaviour
{
    public Button Btn_ChangeScene;
    // Start is called before the first frame update
    void Start()
    {
        Btn_ChangeScene.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            // AssetMgr.Instance.LoadAssetAsync<GameObject>("TestAssetCube", (obj) =>
            // {
            //     GameObject _testCube = GameObject.Instantiate(obj, Vector3.zero, Quaternion.identity);
            //     _testCube.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            //     TimerUtils.TimerOnce(3, () =>
            //     {
            //         AssetMgr.Instance.ReleaseAsset("TestAssetCube");
            //     });
            // }, false);

            // AssetMgr.Instance.LoadAssetAsync<GameObject>("TestAssetCube", (obj) =>
            // {
            //     TimerUtils.TimerOnce(3, () =>
            //     {
            //         GameObject _testCube = GameObject.Instantiate(obj, Vector3.zero, Quaternion.identity);
            //         _testCube.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            //     });
            // }, true);

            // AssetMgr.Instance.LoadSceneAsync("TestAssetScene", UnityEngine.SceneManagement.LoadSceneMode.Single, false,
            //     (progress) => { Debug.Log("Load scene progress" + progress); },
            //     (sceneInstance) =>
            //     {
            //         Btn_ChangeScene.onClick.AddListener(() => { sceneInstance.ActivateAsync(); });
            //         Btn_ChangeScene.gameObject.SetActive(true);
            //     });

            // AssetMgr.Instance.InstantiateAsync("TestAssetCube", (obj) =>
            // {
            //     go = obj;
            // });

            // TimerUtils.TimerOnce(3, () =>
            //     {
            //         AssetMgr.Instance.ReleaseInstance(ref go);
            //     });

            AssetMgr.Instance.LoadAssetsByLabel<GameObject>("Test",
            (item) => { GameObject _test = GameObject.Instantiate(item, Vector3.zero, Quaternion.identity); });

        }
    }
}
