using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JLXB.Framework.BehaviorTree;
public class TestBehaviorTree : MonoBehaviour
{
    private BehaviorTree tree;
    void Start()
    {
        tree = GetComponent<BehaviorTree>();
    }

    // Update is called once per frame
    void Update()
    {
        tree.Tick();
    }
}
