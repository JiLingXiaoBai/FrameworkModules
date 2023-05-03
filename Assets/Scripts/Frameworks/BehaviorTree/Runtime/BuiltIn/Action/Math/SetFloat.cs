using UnityEngine;
namespace JLXB.Framework.BehaviorTree.Extend
{
    [EditorInfo("Action:Float类型赋值")]
    [EditorLabel("Math:SetFloat")]
    [EditorGroup("Math")]
    public class SetFloat : Action
    {
        [SerializeField]
        private float setValue;
        [SerializeField, ForceShared]
        private SharedFloat floatToSet;
        public override void Awake()
        {
            InitVariable(floatToSet);
        }
        protected override Status OnUpdate()
        {
            floatToSet.Value = setValue;
            return Status.Success;
        }
    }
}