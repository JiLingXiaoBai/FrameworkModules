using System;
using System.Reflection;
using UnityEngine.UIElements;
using UnityEngine;
using System.Collections.Generic;

namespace JLXB.Framework.BehaviorTree.Editor
{
    sealed class Ordered : Attribute
    {
        public int Order = 100;
    }

    public abstract class FieldResolver<T, K> : IFieldResolver where T : BaseField<K>
    {
        private readonly FieldInfo fieldInfo;
        private T editorField;

        protected FieldResolver(FieldInfo fieldInfo)
        {
            this.fieldInfo = fieldInfo;
            SetEditorField();
        }

        private void SetEditorField()
        {
            this.editorField = this.CreateEditorField(this.fieldInfo);
            //修改标签
            EditorLabelAttribute label = this.fieldInfo.GetCustomAttribute<EditorLabelAttribute>();
            if (label != null) this.editorField.label = label.Title;
            TooltipAttribute tooltip = this.fieldInfo.GetCustomAttribute<TooltipAttribute>();
            if (tooltip != null) this.editorField.tooltip = tooltip.tooltip;
        }

        protected abstract T CreateEditorField(FieldInfo fieldInfo);
        protected virtual void SetTree(ITreeView ownerTreeView) { }


        #region Implementation of interface
        public object Value => editorField.value;
        public VisualElement CreateField() => CreateEditorField(this.fieldInfo);
        public VisualElement GetEditorField(ITreeView ownerTreeView)
        {
            SetTree(ownerTreeView);
            return this.editorField;
        }

        /// <summary>
        /// 不安全的方法,外界注入回调操作的列表和共享变量,不保证转换成功
        /// </summary>
        /// <param name="ExposedProperties"></param>
        /// <param name="variable"></param>
        public VisualElement GetEditorField(List<SharedVariable> ExposedProperties, SharedVariable variable)
        {
            this.editorField.RegisterValueChangedCallback(evt =>
            {
                var index = ExposedProperties.FindIndex(x => x.Name == variable.Name);
                ExposedProperties[index].SetValue(evt.newValue);
            });
            this.editorField.value = (K)variable.GetValue();
            return this.editorField;
        }

        public void Commit(NodeBehavior behavior)
        {
            fieldInfo.SetValue(behavior, editorField.value);
        }

        public void Copy(IFieldResolver resolver)
        {
            if (resolver is not FieldResolver<T, K>) return;
            editorField.value = (K)resolver.Value;
        }

        public void Restore(NodeBehavior behavior)
        {
            editorField.value = (K)fieldInfo.GetValue(behavior);
        }
        #endregion
    }
}