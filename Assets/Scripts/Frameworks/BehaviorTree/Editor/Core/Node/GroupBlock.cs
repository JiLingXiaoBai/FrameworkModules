using UnityEditor.Experimental.GraphView;

namespace JLXB.Framework.BehaviorTree.Editor
{
    public class GroupBlock : Group
    {
        public GroupBlock()
        {
            capabilities |= Capabilities.Ascendable;
        }
    }
}