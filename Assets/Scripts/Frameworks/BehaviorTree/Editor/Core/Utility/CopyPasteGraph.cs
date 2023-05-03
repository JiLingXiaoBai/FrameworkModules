using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
namespace JLXB.Framework.BehaviorTree.Editor
{
    internal class CopyPasteGraph
    {
        private readonly BehaviorTreeView sourceView;
        private List<ISelectable> copyElements;
        private Dictionary<Port, Port> portCopyDict;
        private Dictionary<BTNode, BTNode> nodeCopyDict;
        private readonly List<ISelectable> selection;
        public CopyPasteGraph(BehaviorTreeView sourceView, List<ISelectable> selection)
        {
            this.sourceView = sourceView;
            this.selection = selection;
            copyElements = new List<ISelectable>();
            portCopyDict = new Dictionary<Port, Port>();
            nodeCopyDict = new Dictionary<BTNode, BTNode>();
            CopyNodes();
            CopyEdges();
            CopyGroupBlocks();
        }
        public List<ISelectable> GetCopyElements() => copyElements;
        private void CopyNodes()
        {
            foreach (var select in selection)
            {
                if (select is not BTNode) continue;
                //先复制所有子结点
                BTNode selectNode = select as BTNode;
                var node = sourceView.DuplicateNode(selectNode);
                copyElements.Add(node);
                nodeCopyDict.Add(selectNode, node);
                if (selectNode.GetBehavior().IsSubclassOf(typeof(Action)))
                {
                    var actionNode = selectNode as ActionNode;
                    portCopyDict.Add(actionNode.Parent, (node as ActionNode).Parent);
                }
                if (selectNode.GetBehavior().IsSubclassOf(typeof(Composite)))
                {
                    var compositeNode = selectNode as CompositeNode;
                    var copy = node as CompositeNode;
                    int count = compositeNode.ChildPorts.Count - copy.ChildPorts.Count;
                    for (int i = 0; i < count; i++)
                    {
                        copy.AddChild();
                    }
                    for (int i = 0; i < compositeNode.ChildPorts.Count; i++)
                    {
                        portCopyDict.Add(compositeNode.ChildPorts[i], copy.ChildPorts[i]);
                    }
                    portCopyDict.Add(compositeNode.Parent, copy.Parent);
                }
                if (selectNode.GetBehavior().IsSubclassOf(typeof(Condition)))
                {
                    var conditionalNode = selectNode as ConditionalNode;
                    portCopyDict.Add(conditionalNode.Child, (node as ConditionalNode).Child);
                    portCopyDict.Add(conditionalNode.Parent, (node as ConditionalNode).Parent);

                }
                if (selectNode.GetBehavior().IsSubclassOf(typeof(Decorator)))
                {
                    var decoratorNode = node as DecoratorNode;
                    portCopyDict.Add(decoratorNode.Child, (node as DecoratorNode).Child);
                    portCopyDict.Add(decoratorNode.Parent, (node as DecoratorNode).Parent);
                }
            }
        }
        private void CopyEdges()
        {
            foreach (var select in selection)
            {
                if (select is not Edge) continue;
                var edge = select as Edge;
                if (!portCopyDict.ContainsKey(edge.input) || !portCopyDict.ContainsKey(edge.output)) continue;
                var newEdge = BehaviorTreeView.ConnectPorts(portCopyDict[edge.output], portCopyDict[edge.input]);
                sourceView.AddElement(newEdge);
                copyElements.Add(newEdge);
            }
        }
        private void CopyGroupBlocks()
        {
            foreach (var select in selection)
            {
                if (select is not GroupBlock) continue;
                var selectBlock = select as GroupBlock;
                var nodes = selectBlock.containedElements.Where(x => x is BTNode).Cast<BTNode>();
                Rect newRect = selectBlock.GetPosition();
                newRect.position += new Vector2(50, 50);
                var block = sourceView.CreateBlock(newRect);
                block.title = selectBlock.title;
                block.AddElements(nodes.Where(x => nodeCopyDict.ContainsKey(x)).Select(x => nodeCopyDict[x]));
            }
        }
    }
}
