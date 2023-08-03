using UnityEngine;
using JLXB.Framework.Event;

public class TestEvent : MonoBehaviour
{
    [SerializeField] private int loopCount = 100000;

    void Start()
    {
        EventCenter.Register<int>(EventConst.TEST_MESSAGE, TestMessage1);
        EventCenter.Register<(int, string)>(EventConst.TEST_MESSAGE_TUPLE, TestMessageTuple);
    }

    private void OnDestroy()
    {
        EventCenter.Remove<int>(EventConst.TEST_MESSAGE, TestMessage1);
        EventCenter.Remove<(int, string)>(EventConst.TEST_MESSAGE_TUPLE, TestMessageTuple);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            for (int i = 0; i < loopCount; i++)
            {
                EventCenter.DispatchEvent<int>(EventConst.TEST_MESSAGE, 1);
                EventCenter.DispatchEvent<(int, string)>(EventConst.TEST_MESSAGE_TUPLE, (3, "lalala"));
            }
        }
    }

    void TestMessageTuple((int, string) data)
    {
        for (int i = 0; i < data.Item1; i++)
        {
            Debug.Log("tuple Test Message" + data.Item2);
        }
    }

    void TestMessage0()
    {
        Debug.Log("void Test Message");
    }

    void TestMessage1(int data)
    {
        Debug.Log("int TestMessage data: " + data);
    }

    void TestMessage2(string data)
    {
        Debug.Log("string TestMessage data: " + data);
    }
}

