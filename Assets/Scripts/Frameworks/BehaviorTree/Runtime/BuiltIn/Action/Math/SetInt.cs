using UnityEngine;
namespace JLXB.Framework.BehaviorTree.Extend
{
    [EditorInfo("Action:Int类型赋值")]
    [EditorLabel("Math:SetInt")]
    [EditorGroup("Math")]
    public class SetInt : Action
    {
        [SerializeField]
        private int setValue;
        [SerializeField, ForceShared]
        private SharedInt intToSet;

        public override void Awake()
        {
            InitVariable(intToSet);
        }
        protected override Status OnUpdate()
        {
            intToSet.Value = setValue;
            return Status.Success;
        }
    }
}