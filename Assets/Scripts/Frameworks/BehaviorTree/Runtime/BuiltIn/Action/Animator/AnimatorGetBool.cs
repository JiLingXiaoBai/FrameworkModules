using UnityEngine;
namespace JLXB.Framework.BehaviorTree.Extend
{
    [EditorInfo("Action:获取Animator的Bool元素")]
    [EditorLabel("Animator:GetBool")]
    [EditorGroup("Animator")]
    public class AnimatorGetBool : AnimatorAction
    {
        [SerializeField]
        private string parameter;
        [SerializeField]
        private SharedBool storeResult;
        private int parameterHash;
        public override void Start()
        {
            parameterHash = Animator.StringToHash(parameter);
            InitVariable(storeResult);
        }
        protected override Status OnUpdate()
        {
            storeResult.Value = _Animator.GetBool(parameterHash);
            return Status.Success;
        }
    }
}