using System.Reflection;
using System;
namespace JLXB.Framework.BehaviorTree.Editor
{
    public class SharedBoolResolver : FieldResolver<SharedBoolField, SharedBool>
    {
        public SharedBoolResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override void SetTree(ITreeView ownerTreeView)
        {
            editorField.InitField(ownerTreeView);
        }
        private SharedBoolField editorField;
        protected override SharedBoolField CreateEditorField(FieldInfo fieldInfo)
        {
            editorField = new SharedBoolField(fieldInfo.Name, null, fieldInfo.FieldType, fieldInfo);
            return editorField;
        }
        public static bool IsAcceptable(Type infoType, FieldInfo info) => infoType == typeof(SharedBool);

    }
}