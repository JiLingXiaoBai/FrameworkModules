using UnityEngine;
using XBToolKit;

public class TestLog : MonoBehaviour
{
    private void Awake()
    {
        GameLogger.DebugLogEnable = true;
        GameLogger.WarningLogEnable = true;
        GameLogger.ErrorLogEnable = true;
        GameLogger.LogToFile = true;
        GameLogger.LogFileCount = 5;
    }
    
    private void Start()
    {
        GameLogger.Open();
    }

    private void Update()
    {
        GameLogger.Debug("TestDebug");
        GameLogger.Error("TestError");
        GameLogger.Warning("TestWarning");
        Debug.Log("AAA");
        Debug.LogError("BBB");
        Debug.LogWarning("CCC");
    }

    private void OnDestroy()
    {
        GameLogger.Close();
    }
}