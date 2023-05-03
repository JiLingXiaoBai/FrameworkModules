using System;
using System.Reflection;

namespace JLXB.Framework.BehaviorTree.Editor
{
    public class SharedIntResolver : FieldResolver<SharedIntField, SharedInt>
    {
        public SharedIntResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override void SetTree(ITreeView ownerTreeView)
        {
            editorField.InitField(ownerTreeView);
        }
        private SharedIntField editorField;
        protected override SharedIntField CreateEditorField(FieldInfo fieldInfo)
        {
            editorField = new SharedIntField(fieldInfo.Name, null, fieldInfo.FieldType, fieldInfo);
            return editorField;
        }
        public static bool IsAcceptable(Type infoType, FieldInfo info) => infoType == typeof(SharedInt);

    }
}