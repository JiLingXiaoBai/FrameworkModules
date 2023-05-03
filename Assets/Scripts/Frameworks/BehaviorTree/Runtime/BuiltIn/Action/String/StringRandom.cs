using System.Collections.Generic;
using UnityEngine;
namespace JLXB.Framework.BehaviorTree.Extend
{
    [EditorInfo("Action:随机获取String值")]
    [EditorLabel("String:Random")]
    [EditorGroup("String")]
    public class StringRandom : Action
    {
        [SerializeField]
        private List<string> randomStrings;
        [SerializeField, ForceShared]
        private SharedString storeResult;
        public override void Awake()
        {
            InitVariable(storeResult);
        }
        protected override Status OnUpdate()
        {
            storeResult.Value = randomStrings[UnityEngine.Random.Range(0, randomStrings.Count)];
            return Status.Success;
        }
    }
}
