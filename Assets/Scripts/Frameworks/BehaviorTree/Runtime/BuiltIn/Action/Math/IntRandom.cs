using UnityEngine;
namespace JLXB.Framework.BehaviorTree.Extend
{
    [EditorInfo("Action:Int类型随机值")]
    [EditorLabel("Math:IntRandom")]
    [EditorGroup("Math")]
    public class IntRandom : Action
    {
        private enum Operation
        {
            Absolutely,
            Relatively
        }
        [SerializeField]
        private Vector2Int range = new Vector2Int(-5, 5);
        [SerializeField]
        private Operation operation;
        [SerializeField, ForceShared]
        private SharedInt randomInt;
        public override void Awake()
        {
            InitVariable(randomInt);
        }
        protected override Status OnUpdate()
        {
            int random = UnityEngine.Random.Range(range.x, range.y);
            randomInt.Value = (operation == Operation.Absolutely ? 0 : randomInt.Value) + random;
            return Status.Success;
        }
    }
}