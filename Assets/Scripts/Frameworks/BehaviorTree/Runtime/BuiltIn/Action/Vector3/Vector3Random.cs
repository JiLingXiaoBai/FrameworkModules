using UnityEngine;
using UnityEngine.Serialization;
namespace JLXB.Framework.BehaviorTree.Extend
{
    [EditorInfo("Action:随机获取Vector3值")]
    [EditorLabel("Vector3:Random")]
    [EditorGroup("Vector3")]
    public class Vector3Random : Action
    {
        private enum Operation
        {
            Absolutely,
            Relatively
        }

        [SerializeField]
        private Vector2 xRange = new Vector2(-5, 5);
        [SerializeField]
        private Vector2 yRange = new Vector2(-5, 5);
        [SerializeField]
        private Vector2 zRange = new Vector2(-5, 5);
        [SerializeField]
        private Operation operation;
        [SerializeField, ForceShared, FormerlySerializedAs("randomVector3")]
        private SharedVector3 storeResult;
        public override void Awake()
        {
            InitVariable(storeResult);
        }
        protected override Status OnUpdate()
        {
            Vector3 addVector3 = new Vector3(UnityEngine.Random.Range(xRange.x, xRange.y), UnityEngine.Random.Range(yRange.x, yRange.y), UnityEngine.Random.Range(zRange.x, zRange.y));
            storeResult.Value = (operation == Operation.Absolutely ? Vector3.zero : storeResult.Value) + addVector3;
            return Status.Success;
        }
    }
}