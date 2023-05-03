using System;

namespace JLXB.Framework.BehaviorTree
{
    /// <summary>
    /// 在编辑器中描述结点行为
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class EditorInfoAttribute : Attribute
    {
        private readonly string mDescription;
        public string Description
        {
            get => this.mDescription;
        }

        public EditorInfoAttribute(string description)
        {
            this.mDescription = description;
        }
    }
}

