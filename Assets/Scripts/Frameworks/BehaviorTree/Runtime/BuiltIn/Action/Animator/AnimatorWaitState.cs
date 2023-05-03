using UnityEngine;
namespace JLXB.Framework.BehaviorTree.Extend
{
    [EditorInfo("Action:等待Animator进入指定State")]
    [EditorLabel("Animator:WaitState")]
    [EditorGroup("Animator")]
    public class AnimatorWaitState : AnimatorAction
    {
        [SerializeField]
        private string stateName;
        [SerializeField]
        private int layer = -1;
        protected override Status OnUpdate()
        {
            AnimatorStateInfo stateInfo = _Animator.GetCurrentAnimatorStateInfo(layer);
            if (stateInfo.IsName(stateName))
                return Status.Success;
            else
                return Status.Running;
        }
    }
}