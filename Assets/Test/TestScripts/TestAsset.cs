using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JLXB.Framework.Asset;
using JLXB.Framework.Timer;

public class TestAsset : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            AssetMgr.Instance.LoadAssetAsync<GameObject>("TestAssetCube", (obj) =>
            {
                GameObject _testCube = GameObject.Instantiate(obj, Vector3.zero, Quaternion.identity);
                _testCube.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                TimerUtils.TimerOnce(3, () =>
                {
                    AssetMgr.Instance.ReleaseAsset(ref obj);
                });
            }, false);
        }
    }
}
