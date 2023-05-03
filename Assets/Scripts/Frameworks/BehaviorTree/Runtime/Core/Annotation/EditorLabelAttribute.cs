using System;

namespace JLXB.Framework.BehaviorTree
{
    /// <summary>
    /// 对行为结点在编辑器中的名称进行替换,也可以对字段名称进行替换
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class EditorLabelAttribute : Attribute
    {
        private readonly string mTitle;
        public string Title
        {
            get => this.mTitle;
        }

        public EditorLabelAttribute(string tite)
        {
            this.mTitle = tite;
        }
    }
}