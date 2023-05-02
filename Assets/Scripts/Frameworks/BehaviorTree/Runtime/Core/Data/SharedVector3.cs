
using UnityEngine;
namespace JLXB.Framework.BehaviorTree
{
    [System.Serializable]
    public class SharedVector3 : SharedVariable<Vector3>
    {
        public override object Clone()
        {
            return new SharedVector3() { Value = this.Value, Name = this.Name, IsShared = this.IsShared };
        }
    }
}