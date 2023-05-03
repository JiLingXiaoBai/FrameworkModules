using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;

namespace JLXB.Framework.BehaviorTree.Editor
{
    public class ObjectResolver : FieldResolver<ObjectField, Object>
    {
        public ObjectResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override ObjectField CreateEditorField(FieldInfo fieldInfo)
        {
            var editorField = new ObjectField(fieldInfo.Name);
            editorField.objectType = fieldInfo.FieldType;
            return editorField;
        }
    }
}