using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace JLXB.Framework.BehaviorTree.Editor
{
    public class CompositeNode : BTNode
    {
        public readonly List<Port> ChildPorts = new List<Port>();

        private List<BTNode> cache = new List<BTNode>();
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.MenuItems().Add(new BehaviorTreeDropdownMenuAction("Change Behavior", (a) =>
            {
                var provider = ScriptableObject.CreateInstance<CompositeSearchWindowProvider>();
                provider.Init(this, BehaviorTreeSetting.GetMask(mapTreeView.treeEditorName));
                SearchWindow.Open(new SearchWindowContext(a.eventInfo.localMousePosition), provider);
            }));
            evt.menu.MenuItems().Add(new BehaviorTreeDropdownMenuAction("Add Child", (a) => AddChild()));
            evt.menu.MenuItems().Add(new BehaviorTreeDropdownMenuAction("Remove Unnecessary Children", (a) => RemoveUnnecessaryChildren()));
            base.BuildContextualMenu(evt);
        }

        public CompositeNode()
        {
            AddChild();
        }

        public void AddChild()
        {
            var child = CreateChildPort();
            ChildPorts.Add(child);
            outputContainer.Add(child);
        }

        private void RemoveUnnecessaryChildren()
        {
            var unnecessary = ChildPorts.Where(p => !p.connected).ToList();
            unnecessary.ForEach(e =>
            {
                ChildPorts.Remove(e);
                outputContainer.Remove(e);
            });
        }

        protected override bool OnValidate(Stack<BTNode> stack)
        {
            if (ChildPorts.Count <= 0 && !noValidate) return false;
            foreach (var port in ChildPorts)
            {
                if (!port.connected)
                {
                    if (noValidate) continue;
                    style.backgroundColor = Color.red;
                    return false;
                }
                stack.Push(port.connections.First().input.node as BTNode);
            }
            style.backgroundColor = new StyleColor(StyleKeyword.Null);
            return true;
        }

        protected override void OnCommit(Stack<BTNode> stack)
        {
            cache.Clear();
            foreach (var port in ChildPorts)
            {
                if (port.connections.Count() == 0) continue;
                var child = port.connections.First().input.node as BTNode;
                (NodeBehavior as Composite).AddChild(child.ReplaceBehavior());
                stack.Push(child);
                cache.Add(child);
            }
        }

        protected override void OnClearStyle()
        {
            foreach (var node in cache)
            {
                node.ClearStyle();
            }
        }
    }
}