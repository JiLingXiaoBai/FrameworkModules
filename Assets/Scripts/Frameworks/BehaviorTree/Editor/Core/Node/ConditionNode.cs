using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;

namespace JLXB.Framework.BehaviorTree.Editor
{
    public class ConditionalNode : BTNode
    {
        private Port childPort;

        public Port Child => childPort;

        private BTNode cache;

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.MenuItems().Add(new BehaviorTreeDropdownMenuAction("Change Behavior", (a) =>
            {
                var provider = ScriptableObject.CreateInstance<ConditionSearchWindowProvider>();
                provider.Init(this, BehaviorTreeSetting.GetMask(mapTreeView.treeEditorName));
                SearchWindow.Open(new SearchWindowContext(a.eventInfo.localMousePosition), provider);
            }));
            base.BuildContextualMenu(evt);
        }

        public ConditionalNode()
        {
            childPort = CreateChildPort();
            outputContainer.Add(childPort);
        }

        protected override bool OnValidate(Stack<BTNode> stack)
        {
            if (!childPort.connected)
            {
                return true;
            }
            stack.Push(childPort.connections.First().input.node as BTNode);
            return true;
        }

        protected override void OnCommit(Stack<BTNode> stack)
        {
            if (!childPort.connected)
            {
                (NodeBehavior as Condition).Child = null;
                cache = null;
                return;
            }
            var child = childPort.connections.First().input.node as BTNode;
            (NodeBehavior as Condition).Child = child.ReplaceBehavior();
            stack.Push(child);
            cache = child;
        }

        protected override void OnClearStyle()
        {
            cache?.ClearStyle();
        }
    }
}