using System;
using System.Reflection;
using UnityEditor.UIElements;
namespace JLXB.Framework.BehaviorTree.Editor
{
    [Ordered]
    public class LayerResolver : FieldResolver<LayerField, int>
    {
        public LayerResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override LayerField CreateEditorField(FieldInfo fieldInfo)
        {
            return new LayerField(fieldInfo.Name);
        }
        public static bool IsAcceptable(Type infoType, FieldInfo info) => infoType == typeof(int) && info.GetCustomAttribute<Layer>() != null;
    }
}