using UnityEngine;
using JLXB.Framework.Event;

public class TestEvent : MonoBehaviour
{
    [SerializeField] private int loopCount = 100000;

    void Start()
    {
        EventCenter.Register<int>(EventConst.TEST_MESSAGE, TestMessage1);
    }

    private void OnDestroy()
    {
        EventCenter.Remove<int>(EventConst.TEST_MESSAGE, TestMessage1);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            for (int i = 0; i < loopCount; i++)
            {
                EventCenter.DispatchEvent<int>(EventConst.TEST_MESSAGE, 1);
                //EventCenter.DispatchEvent<string>(EventConst.TEST_MESSAGE, "2");
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

