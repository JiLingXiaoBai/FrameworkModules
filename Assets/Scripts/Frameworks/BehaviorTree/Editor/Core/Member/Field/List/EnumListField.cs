using UnityEngine.UIElements;
using System;

namespace JLXB.Framework.BehaviorTree.Editor
{
    public class EnumListField<T> : ListField<T> where T : Enum
    {
        public EnumListField(string label, VisualElement visualInput, Func<VisualElement> elementCreator, Func<object> valueCreator) : base(label, visualInput, elementCreator, valueCreator)
        {

        }
        protected override ListView CreateListView()
        {
            Action<VisualElement, int> bindItem = (e, i) =>
            {
                (e as EnumField).value = value[i];
                (e as EnumField).RegisterValueChangedCallback((x) => value[i] = (T)x.newValue);
            };
            Func<VisualElement> makeItem = () =>
            {
                var field = elementCreator.Invoke();
                (field as EnumField).label = string.Empty;
                return field;
            };
            const int itemHeight = 20;
            var view = new ListView(value, itemHeight, makeItem, bindItem);
            return view;
        }

    }
}