using UnityEngine;
using UnityEngine.AI;
namespace JLXB.Framework.BehaviorTree.Extend
{
    [EditorInfo("Action:设置NavmeshAgent的目的地")]
    [EditorLabel("Navmesh:SetDestination")]
    [EditorGroup("Navmesh")]
    public class NavmeshSetDestination : Action
    {
        [SerializeField, Tooltip("如不填写,则从绑定物体中获取")]
        private NavMeshAgent agent;
        [SerializeField]
        private SharedVector3 destination;
        protected override Status OnUpdate()
        {
            if (agent != null)
            {
                agent.destination = destination.Value;
            }
            return Status.Success;
        }
        public override void Awake()
        {
            if (agent == null) agent = gameObject.GetComponent<NavMeshAgent>();
            InitVariable(destination);
        }

    }
}
