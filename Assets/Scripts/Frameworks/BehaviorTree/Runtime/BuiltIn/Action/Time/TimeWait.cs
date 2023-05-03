using UnityEngine;
namespace JLXB.Framework.BehaviorTree.Extend
{
    [EditorInfo("Action:计时器,等待一段时间,期间返回Running,结束返回Success,Abort打断后复原计数")]
    [EditorLabel("Time:Wait")]
    public class TimeWait : Action
    {
        [SerializeField]
        private SharedFloat waitTime;
        private float timer;
        public override void Awake()
        {
            InitVariable(waitTime);
        }
        protected override Status OnUpdate()
        {
            AddTimer();
            if (IsAlready())
            {
                ClearTimer();
                return Status.Success;
            }
            else
                return Status.Running;
        }
        void AddTimer()
        {
            timer += Time.deltaTime;
        }
        void ClearTimer()
        {
            timer = 0;
        }
        bool IsAlready() => timer > waitTime.Value;
        public override void Abort()
        {
            ClearTimer();
        }
    }
}