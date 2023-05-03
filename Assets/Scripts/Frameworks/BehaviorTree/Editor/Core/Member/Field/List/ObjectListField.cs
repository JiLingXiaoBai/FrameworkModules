using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;
using Object = UnityEngine.Object;

namespace JLXB.Framework.BehaviorTree.Editor
{
    public class ObjectListField<T> : ListField<T> where T : Object
    {
        public ObjectListField(string label, VisualElement visualInput, Func<VisualElement> elementCreator, Func<object> valueCreator) : base(label, visualInput, elementCreator, valueCreator)
        {
        }
        protected override ListView CreateListView()
        {
            Action<VisualElement, int> bindItem = (e, i) =>
            {
                (e as ObjectField).value = value[i];
                (e as ObjectField).RegisterValueChangedCallback((x) => value[i] = (T)x.newValue);
            };
            Func<VisualElement> makeItem = () =>
            {
                var field = elementCreator.Invoke();
                (field as ObjectField).label = string.Empty;
                (field as ObjectField).objectType = typeof(T);
                return field;
            };
            const int itemHeight = 20;
            var view = new ListView(value, itemHeight, makeItem, bindItem);
            return view;
        }

    }
}