using System;
using System.Reflection;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace JLXB.Framework.BehaviorTree.Editor
{
    public class SharedIntField : SharedVariableField<SharedInt, int>
    {

        public SharedIntField(string label, VisualElement visualInput, Type objectType, FieldInfo fieldInfo) : base(label, visualInput, objectType, fieldInfo)
        {
        }
        protected override BaseField<int> CreateValueField() => new IntegerField();
    }
}