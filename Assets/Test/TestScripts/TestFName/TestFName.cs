using System.Collections.Generic;
using UnityEngine;
using XBToolKit;

namespace Test.TestScripts.TestFName
{
    public class TestFName : MonoBehaviour
    {
        public Dictionary<FName, int> fNameDict = new Dictionary<FName, int>();

        void Start()
        {
            for (int i = 0; i < 100; i++)
            {
                var fName = FNamePool.GetFName("Test" + i);
                fNameDict.Add(fName, i);
            }
            
            if (fNameDict.TryAdd(FNamePool.GetFName("Test" + 0), 100))
            {
                Debug.Log("Add success");
            }
            else
            {
                Debug.Log("Add fail");
            }
        }
    }
}