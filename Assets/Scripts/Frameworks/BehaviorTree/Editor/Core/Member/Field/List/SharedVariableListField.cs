using System;
using UnityEngine.UIElements;

namespace JLXB.Framework.BehaviorTree.Editor
{
    public class SharedVariableListField<T> : ListField<T>, IInitField where T : SharedVariable
    {
        private ITreeView treeView;
        public event Action<ITreeView> OnTreeViewInitEvent;
        public SharedVariableListField(string label, VisualElement visualInput, Func<VisualElement> elementCreator, Func<object> valueCreator) : base(label, visualInput, elementCreator, valueCreator)
        {

        }
        public void InitField(ITreeView treeView)
        {
            this.treeView = treeView;
            OnTreeViewInitEvent?.Invoke(treeView);
        }
        protected override ListView CreateListView()
        {
            Action<VisualElement, int> bindItem = (e, i) =>
            {
                (e as BaseField<T>).value = value[i];
                (e as BaseField<T>).RegisterValueChangedCallback((x) => value[i] = (T)x.newValue);
            };
            Func<VisualElement> makeItem = () =>
            {
                var field = elementCreator.Invoke();
                (field as IFoldout).foldout.text = string.Empty;
                (field as IFoldout).foldout.value = true;
                (field as BaseField<T>).label = string.Empty;
                if (treeView != null) (field as IInitField).InitField(treeView);
                OnTreeViewInitEvent += (view) => { (field as IInitField).InitField(view); };
                return field;
            };
            var view = new ListView(value, 60, makeItem, bindItem);
            return view;
        }

    }
}