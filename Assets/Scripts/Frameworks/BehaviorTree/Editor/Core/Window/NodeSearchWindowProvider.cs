using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
namespace JLXB.Framework.BehaviorTree.Editor
{
    public class NodeSearchWindowProvider : ScriptableObject, ISearchWindowProvider
    {
        private BehaviorTreeView graphView;
        private EditorWindow graphEditor;
        private readonly NodeResolver nodeResolver = new NodeResolver();
        private Texture2D _indentationIcon;
        private string[] showGroups;
        private string[] notShowGroups;
        public void Initialize(BehaviorTreeView graphView, EditorWindow graphEditor, (string[], string[]) mask)
        {
            this.graphView = graphView;
            this.graphEditor = graphEditor;
            this.showGroups = mask.Item1;
            this.notShowGroups = mask.Item2;
            _indentationIcon = new Texture2D(1, 1);
            _indentationIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            _indentationIcon.Apply();
        }
        static readonly Type[] _Types = { typeof(Action), typeof(Condition), typeof(Composite), typeof(Decorator) };
        List<SearchTreeEntry> ISearchWindowProvider.CreateSearchTree(SearchWindowContext context)
        {
            var entries = new List<SearchTreeEntry>();
            entries.Add(new SearchTreeGroupEntry(new GUIContent("Create Node"), 0));
            List<Type> nodeTypes = SearchUtility.FindSubClassTypes(_Types);
            var groups = nodeTypes.GroupsByEditorGroup(); ;
            nodeTypes = nodeTypes.Except(groups.SelectMany(x => x)).ToList();
            groups = groups.SelectGroup(showGroups).ExceptGroup(notShowGroups);
            foreach (var _type in _Types)
            {
                entries.Add(new SearchTreeGroupEntry(new GUIContent($"Select {_type.Name}"), 1));
                var group = groups.SelectFather(_type);
                foreach (var subGroup in group)
                {
                    entries.AddAllEntries(subGroup, _indentationIcon, 2);
                }
                var left = nodeTypes.Where(x => x.IsSubclassOf(_type));
                foreach (Type type in left)
                {
                    entries.AddEntry(type, 2, _indentationIcon);
                }
            }
            entries.Add(new SearchTreeEntry(new GUIContent("Create Group Block", _indentationIcon)) { level = 1, userData = typeof(GroupBlock) });
            return entries;
        }
        bool ISearchWindowProvider.OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            var worldMousePosition = this.graphEditor.rootVisualElement.ChangeCoordinatesTo(this.graphEditor.rootVisualElement.parent, context.screenMousePosition - this.graphEditor.position.position);
            var localMousePosition = this.graphView.contentViewContainer.WorldToLocal(worldMousePosition);
            Rect newRect = new Rect(localMousePosition, new Vector2(100, 100));
            var type = searchTreeEntry.userData as Type;
            if (type == typeof(GroupBlock))
            {
                graphView.CreateBlock(newRect);
                return true;
            }
            var node = this.nodeResolver.CreateNodeInstance(type, graphView);
            node.SetPosition(newRect);
            this.graphView.AddElement(node);
            node.onSelectAction = graphView.onSelectAction;
            return true;
        }
    }
    public class CertainNodeSearchWindowProvider<T> : ScriptableObject, ISearchWindowProvider where T : NodeBehavior
    {
        private BTNode node;
        private Texture2D _indentationIcon;
        private string[] showGroups;
        private string[] notShowGroups;
        public void Init(BTNode node, (string[], string[]) mask)
        {
            this.node = node;
            this.showGroups = mask.Item1;
            this.notShowGroups = mask.Item2;
            _indentationIcon = new Texture2D(1, 1);
            _indentationIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            _indentationIcon.Apply();
        }
        List<SearchTreeEntry> ISearchWindowProvider.CreateSearchTree(SearchWindowContext context)
        {
            var entries = new List<SearchTreeEntry>();
            Dictionary<string, List<Type>> attributeDict = new Dictionary<string, List<Type>>();

            entries.Add(new SearchTreeGroupEntry(new GUIContent($"Select {typeof(T).Name}"), 0));
            List<Type> nodeTypes = SearchUtility.FindSubClassTypes(typeof(T));
            var groups = nodeTypes.GroupsByEditorGroup();//按EditorGroup进行分类
            nodeTypes = nodeTypes.Except(groups.SelectMany(x => x)).ToList();//去除被分类的部分
            groups = groups.SelectGroup(showGroups).ExceptGroup(notShowGroups);
            foreach (var group in groups)
            {
                entries.AddAllEntries(group, _indentationIcon, 1);
            }
            foreach (Type type in nodeTypes)
            {
                entries.AddEntry(type, 1, _indentationIcon);
            }
            return entries;
        }

        bool ISearchWindowProvider.OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            var type = searchTreeEntry.userData as System.Type;
            this.node.SetBehavior(type);
            return true;
        }
    }
}
