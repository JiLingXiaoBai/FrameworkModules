
namespace JLXB.Framework.BehaviorTree
{
    [System.Serializable]
    public class SharedFloat : SharedVariable<float>
    {
        public override object Clone()
        {
            return new SharedFloat() { Value = this.Value, Name = this.Name, IsShared = this.IsShared };
        }
    }
}