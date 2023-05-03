
namespace JLXB.Framework.BehaviorTree
{
    [System.Serializable]
    public class SharedInt : SharedVariable<int>
    {
        public SharedInt() { }
        public SharedInt(int value)
        {
            this.value = value;
        }
        public override object Clone()
        {
            return new SharedInt() { Value = this.Value, Name = this.Name, IsShared = this.IsShared };
        }
    }
}