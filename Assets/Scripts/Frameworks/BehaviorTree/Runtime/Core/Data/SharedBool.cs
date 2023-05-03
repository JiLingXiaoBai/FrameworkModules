
namespace JLXB.Framework.BehaviorTree
{
    [System.Serializable]
    public class SharedBool : SharedVariable<bool>
    {
        public SharedBool() { }
        public SharedBool(bool value)
        {
            this.value = value;
        }
        public override object Clone()
        {
            return new SharedBool() { Value = this.Value, Name = this.Name, IsShared = this.IsShared };
        }
    }
}