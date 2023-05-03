using System;
using System.Reflection;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
namespace JLXB.Framework.BehaviorTree.Editor
{
    public class SharedFloatField : SharedVariableField<SharedFloat, float>
    {

        public SharedFloatField(string label, VisualElement visualInput, Type objectType, FieldInfo fieldInfo) : base(label, visualInput, objectType, fieldInfo)
        {

        }
        protected override BaseField<float> CreateValueField() => new FloatField();
    }
}