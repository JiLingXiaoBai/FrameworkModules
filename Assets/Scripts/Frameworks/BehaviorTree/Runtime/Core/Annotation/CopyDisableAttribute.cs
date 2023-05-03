using System;

namespace JLXB.Framework.BehaviorTree
{
    /// <summary>
    /// 禁止结点在编辑器内复制
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class CopyDisableAttribute : Attribute
    {

    }
}