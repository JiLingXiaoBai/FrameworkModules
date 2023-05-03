using System.Collections.Generic;
using UnityEngine.UIElements;

namespace JLXB.Framework.BehaviorTree.Editor
{
    public interface IFieldResolver
    {
        /// <summary>
        /// 获取ValueField同时绑定行为树视图
        /// </summary>
        /// <param name="ownerTreeView"></param>
        /// <returns></returns>
        VisualElement GetEditorField(ITreeView ownerTreeView);
        /// <summary>
        /// 获取ValueField同时绑定共享变量
        /// </summary>
        /// <param name="ExposedProperties"></param>
        /// <param name="variable"></param>
        /// <returns></returns>
        VisualElement GetEditorField(List<SharedVariable> ExposedProperties, SharedVariable variable);
        /// <summary>
        /// 只创建ValueField,不进行任何绑定
        /// </summary>
        /// <returns></returns>
        public VisualElement CreateField();
        void Restore(NodeBehavior behavior);
        void Commit(NodeBehavior behavior);
        void Copy(IFieldResolver resolver);
        object Value { get; }
    }
}