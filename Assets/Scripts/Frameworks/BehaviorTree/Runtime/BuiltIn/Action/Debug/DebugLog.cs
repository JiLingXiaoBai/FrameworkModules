using UnityEngine;
namespace JLXB.Framework.BehaviorTree.Extend
{
    [EditorInfo("Action:Log一段文字")]
    [EditorLabel("Debug:Log")]
    [EditorGroup("Debug")]
    public class DebugLog : Action
    {
        [SerializeField]
        private SharedString logText;
        public override void Awake()
        {
            InitVariable(logText);
        }
        protected override Status OnUpdate()
        {
            Debug.Log(logText.Value, gameObject);
            return Status.Success;
        }
    }
}
