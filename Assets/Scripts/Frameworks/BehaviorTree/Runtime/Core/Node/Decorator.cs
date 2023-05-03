using UnityEngine;


namespace JLXB.Framework.BehaviorTree
{
    /// <summary>
    /// 装饰器结点行为
    /// </summary>
    public class Decorator : NodeBehavior
    {
        [SerializeReference]
        private NodeBehavior child;

        public NodeBehavior Child
        {
            get => child;
#if UNITY_EDITOR
            set => child = value;
#endif
        }
        private bool isRunning = false;
        protected sealed override void OnRun()
        {
            child?.Run(gameObject, tree);
        }

        public sealed override void Awake()
        {
            OnAwake();
            child?.Awake();
        }

        protected virtual void OnAwake() { }

        public sealed override void Start()
        {
            OnStart();
            child?.Start();
        }

        protected virtual void OnStart() { }

        protected override Status OnUpdate()
        {
            var status = child.Update();
            return OnDecorate(status);
        }

        /// <summary>
        /// 装饰子结点返回值
        /// </summary>
        protected virtual Status OnDecorate(Status childStatus)
        {
            return childStatus;
        }

        /// <summary>
        /// 装饰子判断结点(Condition)的CanUpdate返回值
        /// </summary>
        protected virtual bool OnDecorate(bool childCanUpdate)
        {
            return childCanUpdate;
        }

        public override bool CanUpdate()
        {
            return OnDecorate(child.CanUpdate());
        }


        public sealed override void PreUpdate()
        {
            child?.PreUpdate();
        }
        public sealed override void PostUpdate()
        {
            child?.PostUpdate();
        }
        public override void Abort()
        {
            if (isRunning)
            {
                isRunning = false;
                child?.Abort();
            }
        }
    }
}