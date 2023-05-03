using UnityEngine;
namespace JLXB.Framework.BehaviorTree.Extend
{
    [EditorInfo("Action:根据status设置Animator的Bool元素")]
    [EditorLabel("Animator:SetBool")]
    [EditorGroup("Animator")]
    public class AnimatorSetBool : AnimatorAction
    {
        [SerializeField]
        private string parameter;
        [SerializeField]
        private SharedBool status;
        private int parameterHash;
        public override void Start()
        {
            parameterHash = Animator.StringToHash(parameter);
            InitVariable(status);
        }
        protected override Status OnUpdate()
        {
            _Animator.SetBool(parameterHash, status.Value);
            return Status.Success;
        }
    }
}