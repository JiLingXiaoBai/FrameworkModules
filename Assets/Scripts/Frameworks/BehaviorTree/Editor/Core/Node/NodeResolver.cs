using System;
using UnityEngine.UIElements;

namespace JLXB.Framework.BehaviorTree.Editor
{
    public class NodeResolver
    {
        private StyleSheet styleSheetCache;
        public BTNode CreateNodeInstance(Type type, ITreeView treeView)
        {
            BTNode node;
            if (type.IsSubclassOf(typeof(Composite)))
            {
                node = new CompositeNode();
            }
            else if (type.IsSubclassOf(typeof(Condition)))
            {
                node = new ConditionalNode();
            }
            else if (type.IsSubclassOf(typeof(Decorator)))
            {
                node = new DecoratorNode();
            }
            else if (type == typeof(Root))
            {
                node = new RootNode();
            }
            else
            {
                node = new ActionNode();
            }
            node.SetBehavior(type, treeView);
            if (styleSheetCache == null) styleSheetCache = BehaviorTreeSetting.GetNodeStyle(treeView.treeEditorName);
            node.styleSheets.Add(styleSheetCache);
            return node;
        }
    }
}