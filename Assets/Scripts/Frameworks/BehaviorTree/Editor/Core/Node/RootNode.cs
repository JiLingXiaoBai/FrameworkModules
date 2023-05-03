using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace JLXB.Framework.BehaviorTree.Editor
{
    public sealed class RootNode : BTNode
    {
        public readonly Port Child;

        private BTNode cache;

        public RootNode()
        {
            SetBehavior(typeof(Root));
            title = "Root";
            Child = CreateChildPort();
            outputContainer.Add(Child);
            capabilities &= ~Capabilities.Copiable;
            capabilities &= ~Capabilities.Deletable;//不可删除
            capabilities &= ~Capabilities.Movable;//不可删除
            RefreshExpandedState();
            RefreshPorts();//更新链接
        }

        protected override void AddParent()
        {
        }

        protected override void AddDescription()
        {
        }

        protected override void OnRestore()
        {
            (NodeBehavior as Root).UpdateEditor = ClearStyle;
        }

        protected override bool OnValidate(Stack<BTNode> stack)
        {
            if (!Child.connected)
            {
                return true;
            }
            stack.Push(Child.connections.First().input.node as BTNode);
            return true;
        }
        protected override void OnCommit(Stack<BTNode> stack)
        {

            var newRoot = new Root();
            BTNode child = null;
            if (Child.connected)
            {
                child = Child.connections.First().input.node as BTNode;
                newRoot.Child = child.ReplaceBehavior();
                stack.Push(child);

            }
            newRoot.UpdateEditor = ClearStyle;
            NodeBehavior = newRoot;
            cache = child;
        }

        public void PostCommit(IBehaviorTree tree)
        {
            tree.Root = (NodeBehavior as Root);
        }
        protected override void OnClearStyle()
        {
            cache?.ClearStyle();
        }
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
        }
    }
}