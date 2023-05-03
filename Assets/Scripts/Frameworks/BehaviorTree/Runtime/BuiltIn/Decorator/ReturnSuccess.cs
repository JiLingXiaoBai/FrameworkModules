namespace JLXB.Framework.BehaviorTree.Extend
{
    [EditorInfo("Decorator:无论子结点返回值,始终返回Success")]
    [EditorLabel("ReturnSuccess返回成功")]
    public class ReturnSuccess : Decorator
    {
        protected override Status OnDecorate(Status childStatus)
        {
            return Status.Success;
        }
    }
}