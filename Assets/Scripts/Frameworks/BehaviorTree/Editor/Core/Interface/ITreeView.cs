using System.Collections.Generic;
using System;
namespace JLXB.Framework.BehaviorTree.Editor
{
    public interface ITreeView
    {
        /// <summary>
        /// 将选中结点加入Group并创建Block
        /// </summary>
        void SelectGroup(BTNode node);
        /// <summary>
        /// 取消Group
        /// </summary>
        void UnSelectGroup();
        /// <summary>
        /// 复制结点
        /// </summary>
        /// <param name="node"></param>
        BTNode DuplicateNode(BTNode node);
        /// <summary>
        /// 编辑器名称
        /// </summary>
        string treeEditorName { get; }
        /// <summary>
        /// 共享变量名称修改事件(手动触发)
        /// </summary>
        event Action<SharedVariable> OnPropertyNameChangeEvent;
        /// <summary>
        /// 共享变量名称编辑事件(自动触发)
        /// </summary>
        event Action<SharedVariable> OnPropertyNameEditingEvent;
        /// <summary>
        /// 编辑器内共享变量
        /// </summary>
        List<SharedVariable> ExposedProperties { get; }
        /// <summary>
        /// 是否在Restore中
        /// </summary>
        bool IsRestored { get; }
        /// <summary>
        /// 添加共享变量到黑板
        /// </summary>
        void AddPropertyToBlackBoard(SharedVariable variable);
    }
}