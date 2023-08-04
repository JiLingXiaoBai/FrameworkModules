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
        Log.Debug("123456");
        Log.Info("1212312");
        Log.Warn("45345");
        Log.Error("sdjfa;s");
        Log.Fatal("asdfjasf");
        Debug.Log("黑恶黑");
    }
}
