using System.Collections;
using JLXB.Framework.LogSystem;
using UnityEngine;

public class TestLog : MonoBehaviour
{
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        LogSystem.Instance.Init();
        //Debug.Log("LogSystemStart");

        Log.Debug("111111");
        Debug.Log("LogSystemStart");

        // Log.Info("222222");

        // Log.Warn("333333");

        // Log.Error("444444");

        // Log.Fatal("555555");

        // Log.Debug("666666");
        StartCoroutine(nameof(ShowLog));
        //ShowLogStart1();
    }

    private void ShowLogStart1()
    {
        ShowLogStart2();
    }

    private void ShowLogStart2()
    {
        Log.Debug("111111");
    }

    private IEnumerator ShowLog()
    {
        Debug.Log("ShowLogStart");
        Log.Debug("111111");
        yield return null;
        Log.Info("222222");
        yield return null;
        Log.Warn("333333");
        yield return null;
        Log.Error("444444");
        yield return null;
        Log.Fatal("555555");
        yield return null;
        Log.Debug("666666");
        yield break;
    }
}
