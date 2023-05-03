using System;

namespace JLXB.Framework.BehaviorTree
{
    /// <summary>
    /// 行为结点在编辑器下拉菜单中进行分类,可以用'/'符号进行子分类
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class EditorGroupAttribute : Attribute
    {
        private readonly string mGroup;
        public string Group
        {
            get => this.mGroup;
        }

        public EditorGroupAttribute(string group)
        {
            this.mGroup = group;
        }
    }
}