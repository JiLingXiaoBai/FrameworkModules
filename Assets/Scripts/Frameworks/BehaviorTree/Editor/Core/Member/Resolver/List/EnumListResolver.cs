using System;
using System.Reflection;
using System.Collections.Generic;

namespace JLXB.Framework.BehaviorTree.Editor
{
    public class EnumListResolver<T> : FieldResolver<EnumListField<T>, List<T>> where T : Enum
    {
        protected readonly IFieldResolver childResolver;
        public EnumListResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
            childResolver = new EnumResolver(fieldInfo);
        }
        protected override EnumListField<T> CreateEditorField(FieldInfo fieldInfo)
        {
            return new EnumListField<T>(fieldInfo.Name, null, () => childResolver.CreateField(), () => default(T));
        }
        public static bool IsAcceptable(Type infoType, FieldInfo info) =>
        FieldResolverFactory.IsList(infoType) && infoType.GenericTypeArguments.Length > 0 && infoType.GenericTypeArguments[0].IsEnum;

    }
}