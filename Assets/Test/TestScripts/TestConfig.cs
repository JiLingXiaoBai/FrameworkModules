using XBToolKit.Config;
using UnityEngine;

public class TestConfig : MonoBehaviour
{
    private void Start()
    {
        var quality = ConfigMgr.Instance.GetConfig<string, Quality.ConfigData>();
        
        Debug.Log(quality["four"].color);
        
        foreach (var item in quality)
        {
            Debug.Log(item.Value.id + " " + item.Value.name + " " + item.Value.color + " " + item.Value.description);
        }
    }
}