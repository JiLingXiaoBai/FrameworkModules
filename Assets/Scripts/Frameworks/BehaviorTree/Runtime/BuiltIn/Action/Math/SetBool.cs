using UnityEngine;
namespace JLXB.Framework.BehaviorTree.Extend
{
    [EditorInfo("Action:Bool类型赋值")]
    [EditorLabel("Math:SetBool")]
    [EditorGroup("Math")]
    public class SetBool : Action
    {
        [SerializeField]
        private bool setValue;
        [SerializeField, ForceShared]
        private SharedBool boolToSet;
        public override void Awake()
        {
            InitVariable(boolToSet);
        }
        protected override Status OnUpdate()
        {
            boolToSet.Value = setValue;
            return Status.Success;
        }
    }
}