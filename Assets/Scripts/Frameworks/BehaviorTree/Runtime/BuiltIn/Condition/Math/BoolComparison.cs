using UnityEngine;
namespace JLXB.Framework.BehaviorTree.Extend
{
    [EditorInfo("Condition:比较Bool值,如果满足条件返回True,否则返回False")]
    [EditorLabel("Math:BoolComparison")]
    [EditorGroup("Math")]
    public class BoolComparison : Condition
    {
        private enum Operation
        {
            EqualTo,
            NotEqualTo,
        }
        [SerializeField]
        private SharedBool bool1;
        [SerializeField]
        private SharedBool bool2;
        [SerializeField]
        private Operation operation;
        protected override void OnStart()
        {
            InitVariable(bool1);
            InitVariable(bool2);
        }
        protected override bool IsUpdatable()
        {
            switch (operation)
            {
                case Operation.EqualTo:
                    return bool1.Value == bool2.Value;
                case Operation.NotEqualTo:
                    return bool1.Value != bool2.Value;

            }
            return true;
        }
    }
}