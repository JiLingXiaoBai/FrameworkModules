using System;
using System.Reflection;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine;
namespace JLXB.Framework.BehaviorTree.Editor
{
    public class SharedVector3Field : SharedVariableField<SharedVector3, Vector3>
    {
        public SharedVector3Field(string label, VisualElement visualInput, Type objectType, FieldInfo fieldInfo) : base(label, visualInput, objectType, fieldInfo)
        {
        }
        protected override BaseField<Vector3> CreateValueField() => new Vector3Field();
    }
}