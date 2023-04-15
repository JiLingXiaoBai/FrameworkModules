using System.Collections.Generic;
using UnityEngine;
using JLXB.Framework;

public class TestMessage : MonoBehaviour
{
    [SerializeField] private int loopCount = 100000;

    void Start()
    {
        EventManager.Instance.Register<int>(MessageDefine.TEST_MESSAGE, TestMessage1);
    }

    private void OnDestroy()
    {
        EventManager.Instance.Remove<int>(MessageDefine.TEST_MESSAGE, TestMessage1);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            for (int i = 0; i < loopCount; i++)
            {
                EventManager.Instance.DispatchEvent<int>(MessageDefine.TEST_MESSAGE, 1);
                //EventManager.Instance.DispatchEvent<string>(MessageDefine.TEST_MESSAGE, "2");
            }
        }
    }

    void TestMessage0()
    {
        Debug.Log("void Test Message");
    }

    void TestMessage1(int data)
    {
        int i = data + 1;
        Debug.Log("int TestMessage data: " + data);
    }

    void TestMessage2(string data)
    {

        Debug.Log("string TestMessage data: " + data);
    }
}

