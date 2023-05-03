using System;
using System.Reflection;

namespace JLXB.Framework.BehaviorTree.Editor
{
    public class SharedVector3Resolver : FieldResolver<SharedVector3Field, SharedVector3>
    {
        public SharedVector3Resolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override void SetTree(ITreeView ownerTreeView)
        {
            editorField.InitField(ownerTreeView);
        }
        private SharedVector3Field editorField;
        protected override SharedVector3Field CreateEditorField(FieldInfo fieldInfo)
        {
            editorField = new SharedVector3Field(fieldInfo.Name, null, fieldInfo.FieldType, fieldInfo);
            return editorField;
        }
        public static bool IsAcceptable(Type infoType, FieldInfo info) => infoType == typeof(SharedVector3);
    }
}