using System.Reflection;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace JLXB.Framework.BehaviorTree.Editor
{
    public class SharedStringField : SharedVariableField<SharedString, string>
    {
        private bool multiline;
        public SharedStringField(string label, VisualElement visualInput, Type objectType, FieldInfo fieldInfo) : base(label, visualInput, objectType, fieldInfo)
        {
            multiline = fieldInfo.GetCustomAttribute<MultilineAttribute>() != null;
        }
        protected override BaseField<string> CreateValueField()
        {
            TextField textField;
            textField = new TextField();
            if (multiline)
            {
                textField.multiline = true;
                textField.style.maxWidth = 250;
                textField.style.whiteSpace = WhiteSpace.Normal;
            }
            return textField;
        }
    }
}