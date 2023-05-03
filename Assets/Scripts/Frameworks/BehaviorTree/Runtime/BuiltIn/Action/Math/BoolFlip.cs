using UnityEngine;
namespace JLXB.Framework.BehaviorTree.Extend
{
    [EditorInfo("Action:Bool类型反转")]
    [EditorLabel("Math:BoolFlip")]
    [EditorGroup("Math")]
    public class BoolFlip : Action
    {
        [SerializeField, ForceShared]
        private SharedBool boolToFlip;
        public override void Awake()
        {
            InitVariable(boolToFlip);
        }
        protected override Status OnUpdate()
        {
            boolToFlip.Value = !boolToFlip.Value;
            return Status.Success;
        }
    }
}