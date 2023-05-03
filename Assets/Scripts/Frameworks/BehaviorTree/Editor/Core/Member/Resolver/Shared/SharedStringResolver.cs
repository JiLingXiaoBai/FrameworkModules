using System;
using System.Reflection;
namespace JLXB.Framework.BehaviorTree.Editor
{
    public class SharedStringResolver : FieldResolver<SharedStringField, SharedString>
    {
        public SharedStringResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override void SetTree(ITreeView ownerTreeView)
        {
            editorField.InitField(ownerTreeView);
        }
        private SharedStringField editorField;
        protected override SharedStringField CreateEditorField(FieldInfo fieldInfo)
        {
            editorField = new SharedStringField(fieldInfo.Name, null, fieldInfo.FieldType, fieldInfo);
            return editorField;
        }
        public static bool IsAcceptable(Type infoType, FieldInfo info) => infoType == typeof(SharedString);

    }
}