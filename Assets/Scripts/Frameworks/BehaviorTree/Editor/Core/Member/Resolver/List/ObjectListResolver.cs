using System.Collections.Generic;
using System.Reflection;
using Object = UnityEngine.Object;

namespace JLXB.Framework.BehaviorTree.Editor
{
    public class ObjectListResolver<T> : FieldResolver<ObjectListField<T>, List<T>> where T : Object
    {
        protected readonly IFieldResolver childResolver;
        public ObjectListResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
            childResolver = new ObjectResolver(fieldInfo);
        }
        protected override ObjectListField<T> CreateEditorField(FieldInfo fieldInfo)
        {
            return new ObjectListField<T>(fieldInfo.Name, null, () => childResolver.CreateField(), () => null);
        }
    }
}