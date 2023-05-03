using UnityEngine;
namespace JLXB.Framework.BehaviorTree.Extend
{
    [EditorInfo("Action:String类型赋值")]
    [EditorLabel("String:Set")]
    [EditorGroup("String")]
    public class SetString : Action
    {
        [SerializeField]
        private SharedString value;
        [SerializeField, ForceShared]
        private SharedString storeResult;
        public override void Awake()
        {
            InitVariable(value);
            InitVariable(storeResult);
        }
        protected override Status OnUpdate()
        {
            storeResult.Value = value.Value;
            return Status.Success;
        }
    }
}
