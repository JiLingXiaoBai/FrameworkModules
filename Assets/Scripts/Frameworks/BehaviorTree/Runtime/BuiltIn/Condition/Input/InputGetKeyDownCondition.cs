using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace JLXB.Framework.BehaviorTree.Extend
{
    [EditorInfo("Condition:当指定按键被按下时返回True,否则返回False")]
    [EditorLabel("Input:GetKeyDown")]
    [EditorGroup("Input")]
    public class InputGetKeyDownCondition : Condition
    {
        [SerializeField]
        private KeyCode keyToGet;
        protected override bool IsUpdatable()
        {
            return (Input.GetKeyDown(keyToGet));
        }
    }
}
