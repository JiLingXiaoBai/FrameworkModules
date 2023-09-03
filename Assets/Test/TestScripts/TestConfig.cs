using System;
using JLXB.Framework.Config;
using UnityEngine;

public class TestConfig : MonoBehaviour
{
    private void Start()
    {
        var quality = ConfigMgr.Instance.GetConfig<int, Quality.ConfigData>("Quality");
        
        Debug.Log(quality[4].color);
        
        foreach (var item in quality)
        {
            Debug.Log(item.Value.id + " " + item.Value.name + " " + item.Value.color + " " + item.Value.description);
        }
    }
}