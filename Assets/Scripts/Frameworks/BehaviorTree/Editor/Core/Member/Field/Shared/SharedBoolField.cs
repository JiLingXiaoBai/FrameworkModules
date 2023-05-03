using System;
using System.Reflection;
using UnityEngine.UIElements;

namespace JLXB.Framework.BehaviorTree.Editor
{
    public class SharedBoolField : SharedVariableField<SharedBool, bool>
    {
        public SharedBoolField(string label, VisualElement visualInput, Type objectType, FieldInfo fieldInfo) : base(label, visualInput, objectType, fieldInfo)
        { }

        protected override BaseField<bool> CreateValueField() => new Toggle("Value");
    }
}