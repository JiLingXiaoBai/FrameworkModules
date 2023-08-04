using System.Collections;
using System.Collections.Generic;
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
    }
}
