namespace JLXB.Framework.BehaviorTree.Extend
{
    [EditorInfo("Decorator:子结点返回Success则反转为Failure,为Failure则反转为Success,返回Running则保持Running")]
    [EditorLabel("Invertor反转")]
    public class Invertor : Decorator
    {

        protected override Status OnDecorate(Status childStatus)
        {
            if (childStatus == Status.Success)
                return Status.Failure;
            else if (childStatus == Status.Failure)
                return Status.Success;
            else
                return childStatus;
        }
        protected override bool OnDecorate(bool childCanUpdate)
        {
            return !childCanUpdate;
        }
    }
}