using UnityEngine;
namespace JLXB.Framework.BehaviorTree.Extend
{
    [EditorInfo("Action:触发Animator的Trigger元素")]
    [EditorLabel("Animator:SetTrigger")]
    [EditorGroup("Animator")]
    public class AnimatorSetTrigger : AnimatorAction
    {
        [SerializeField]
        private string parameter;
        private int parameterHash;
        [SerializeField]
        private bool resetLastTrigger = true;
        public override void Start()
        {
            parameterHash = Animator.StringToHash(parameter);
        }
        protected override Status OnUpdate()
        {
            if (resetLastTrigger) _Animator.ResetTrigger(parameterHash);
            _Animator.SetTrigger(parameterHash);
            return Status.Success;
        }
    }
}