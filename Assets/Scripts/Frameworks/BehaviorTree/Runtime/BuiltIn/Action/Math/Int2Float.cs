using UnityEngine;
namespace JLXB.Framework.BehaviorTree.Extend
{
    [EditorInfo("Action:Int类型转为Float类型")]
    [EditorLabel("Math:Int2Float")]
    [EditorGroup("Math")]
    public class Int2Float : Action
    {
        [SerializeField]
        private SharedInt value;
        [SerializeField, ForceShared]
        private SharedFloat newValue;
        public override void Awake()
        {
            InitVariable(value);
            InitVariable(newValue);
        }
        protected override Status OnUpdate()
        {
            newValue.Value = (float)value.Value;
            return Status.Success;
        }
    }
}
