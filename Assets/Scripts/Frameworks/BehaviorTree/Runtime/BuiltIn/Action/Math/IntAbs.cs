using UnityEngine;
namespace JLXB.Framework.BehaviorTree.Extend
{
    [EditorInfo("Action:Int类型取绝对值")]
    [EditorLabel("Math:IntAbs")]
    [EditorGroup("Math")]
    public class IntAbs : Action
    {
        [SerializeField, ForceShared]
        private SharedInt value;
        public override void Awake()
        {
            InitVariable(value);
        }
        protected override Status OnUpdate()
        {
            value.Value = Mathf.Abs(value.Value);
            return Status.Success;
        }
    }
}
