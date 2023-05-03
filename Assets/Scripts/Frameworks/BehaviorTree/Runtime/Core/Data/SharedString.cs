
namespace JLXB.Framework.BehaviorTree
{
    [System.Serializable]
    public class SharedString : SharedVariable<string>
    {
        public SharedString() { }
        public SharedString(string value)
        {
            this.value = value;
        }
        public override object Clone()
        {
            return new SharedString() { Value = this.Value, Name = this.Name, IsShared = this.IsShared };
        }
    }
}