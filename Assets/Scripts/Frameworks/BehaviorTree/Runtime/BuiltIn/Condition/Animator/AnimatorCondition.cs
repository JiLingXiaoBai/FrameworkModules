using UnityEngine;
namespace JLXB.Framework.BehaviorTree.Extend
{

    public abstract class AnimatorCondition : Condition
    {
        protected Animator animator;
        protected override void OnAwake()
        {
            animator = gameObject.GetComponent<Animator>();
        }
    }
}