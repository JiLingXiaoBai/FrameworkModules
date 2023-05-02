
namespace JLXB.Framework.BehaviorTree
{
    [System.Serializable]
    public class SharedBool : SharedVariable<bool>
    {
        public override object Clone()
        {
            return new SharedBool() { Value = this.Value, Name = this.Name, IsShared = this.IsShared };
        }
    }
}