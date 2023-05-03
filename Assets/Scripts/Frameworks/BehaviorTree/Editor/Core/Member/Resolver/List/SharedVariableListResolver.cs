using System;
using System.Collections.Generic;
using System.Reflection;

namespace JLXB.Framework.BehaviorTree.Editor
{
    public class SharedVariableListResolver<T> : FieldResolver<SharedVariableListField<T>, List<T>>, IChildResolver where T : SharedVariable, new()
    {
        private readonly IFieldResolver childResolver;
        public SharedVariableListResolver(FieldInfo fieldInfo, IFieldResolver resolver) : base(fieldInfo)
        {
            childResolver = resolver;
        }
        SharedVariableListField<T> editorField;
        protected override void SetTree(ITreeView ownerTreeView)
        {
            editorField.InitField(ownerTreeView);
        }
        protected override SharedVariableListField<T> CreateEditorField(FieldInfo fieldInfo)
        {
            editorField = new SharedVariableListField<T>(fieldInfo.Name, null, () => childResolver.CreateField(), () => new T());
            return editorField;
        }
        public static bool IsAcceptable(Type infoType, FieldInfo info) =>
        FieldResolverFactory.IsList(infoType) &&
        infoType.GenericTypeArguments.Length > 0 &&
        infoType.GenericTypeArguments[0].IsSubclassOf(typeof(SharedVariable));

    }
}