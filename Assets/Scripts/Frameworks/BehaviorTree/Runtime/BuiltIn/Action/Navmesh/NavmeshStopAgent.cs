using UnityEngine;
using UnityEngine.AI;
namespace JLXB.Framework.BehaviorTree.Extend
{
    [EditorInfo("Action:根据isStopped停止NavmeshAgent")]
    [EditorLabel("Navmesh:StopAgent")]
    [EditorGroup("Navmesh")]
    public class NavmeshStopAgent : Action
    {
        [SerializeField, Tooltip("如不填写,则从绑定物体中获取")]
        private NavMeshAgent agent;
        [SerializeField, EditorLabel("是否停止")]
        private SharedBool isStopped;
        protected override Status OnUpdate()
        {
            if (agent != null && agent.isStopped != isStopped.Value)
            {
                agent.isStopped = isStopped.Value;
            }
            return Status.Success;
        }
        public override void Awake()
        {
            if (agent == null) agent = gameObject.GetComponent<NavMeshAgent>();
            InitVariable(isStopped);
        }

    }
}
