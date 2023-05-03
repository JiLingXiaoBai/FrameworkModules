
namespace JLXB.Framework.BehaviorTree
{
    [System.Serializable]
    public class SharedFloat : SharedVariable<float>
    {
        public SharedFloat() { }
        public SharedFloat(float value)
        {
            this.value = value;
        }
        public override object Clone()
        {
            return new SharedFloat() { Value = this.Value, Name = this.Name, IsShared = this.IsShared };
        }
    }
}