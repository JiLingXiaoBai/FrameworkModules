using System;
using System.Reflection;

namespace JLXB.Framework.BehaviorTree.Editor
{
    public class SharedFloatResolver : FieldResolver<SharedFloatField, SharedFloat>
    {
        public SharedFloatResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override void SetTree(ITreeView ownerTreeView)
        {
            editorField.InitField(ownerTreeView);
        }
        private SharedFloatField editorField;
        protected override SharedFloatField CreateEditorField(FieldInfo fieldInfo)
        {
            editorField = new SharedFloatField(fieldInfo.Name, null, fieldInfo.FieldType, fieldInfo);
            return editorField;
        }
        public static bool IsAcceptable(Type infoType, FieldInfo info) => infoType == typeof(SharedFloat);

    }
}