using UnityEngine;
namespace JLXB.Framework.BehaviorTree.Extend
{
    [EditorInfo("Condition:获取Animator的Bool元素,如果和status一致返回True,否则返回False")]
    [EditorLabel("Animator:BoolCondition")]
    [EditorGroup("Animator")]
    public class AnimatorBoolCondition : AnimatorCondition
    {
        [SerializeField]
        private string parameter;
        [SerializeField]
        private bool status;
        [SerializeField]
        private SharedBool storeResult;
        private int parameterHash;
        protected override void OnStart()
        {
            parameterHash = Animator.StringToHash(parameter);
            InitVariable(storeResult);
        }
        protected override bool IsUpdatable()
        {
            storeResult.Value = animator.GetBool(parameterHash);
            return (storeResult.Value == status);
        }
    }
}