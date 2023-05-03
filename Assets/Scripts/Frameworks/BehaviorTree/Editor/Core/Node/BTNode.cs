using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace JLXB.Framework.BehaviorTree.Editor
{
    public abstract class BTNode : Node
    {
        private string guid;
        public string GUID => guid;
        protected NodeBehavior NodeBehavior { set; get; }
        private Type dirtyNodeBehaviorType;

        public Port Parent { private set; get; }
        private readonly VisualElement container;
        private readonly TextField description;
        public string Description => description.value;
        private readonly FieldResolverFactory fieldResolverFactory;
        public bool Copiable { get; private set; }
        public readonly List<IFieldResolver> resolvers = new List<IFieldResolver>();
        public Action<BTNode> onSelectAction;
        protected ITreeView mapTreeView;
        protected bool noValidate;
        public override void OnSelected()
        {
            base.OnSelected();
            onSelectAction?.Invoke(this);
        }

        protected BTNode()
        {
            fieldResolverFactory = FieldResolverFactory.Instance;
            container = new VisualElement();
            description = new TextField();
            guid = Guid.NewGuid().ToString();
            Initialize();
        }

        private void Initialize()
        {
            AddDescription();
            mainContainer.Add(this.container);
            AddParent();
        }

        protected virtual void AddDescription()
        {
            description.RegisterCallback<FocusInEvent>(evt => { Input.imeCompositionMode = IMECompositionMode.On; });
            description.RegisterCallback<FocusOutEvent>(evt => { Input.imeCompositionMode = IMECompositionMode.Auto; });
            mainContainer.Add(description);
        }
        public void Restore(NodeBehavior behavior)
        {
            NodeBehavior = behavior;
            resolvers.ForEach(e => e.Restore(NodeBehavior));
            NodeBehavior.NotifyEditor = MarkAsExecuted;
            description.value = NodeBehavior.Description;
            guid = string.IsNullOrEmpty(behavior.GUID) ? Guid.NewGuid().ToString() : behavior.GUID;
            OnRestore();
        }
        public void CopyFrom(BTNode copyNode)
        {
            for (int i = 0; i < copyNode.resolvers.Count; i++)
            {
                resolvers[i].Copy(copyNode.resolvers[i]);
            }
            description.value = copyNode.Description;
            NodeBehavior = Activator.CreateInstance(copyNode.GetBehavior()) as NodeBehavior;
            NodeBehavior.NotifyEditor = MarkAsExecuted;
            guid = Guid.NewGuid().ToString();
        }

        protected virtual void OnRestore()
        {

        }

        public NodeBehavior ReplaceBehavior()
        {
            this.NodeBehavior = Activator.CreateInstance(GetBehavior()) as NodeBehavior;
            return NodeBehavior;
        }

        protected virtual void AddParent()
        {
            Parent = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(Port));
            Parent.portName = "Parent";
            inputContainer.Add(Parent);
        }

        protected Port CreateChildPort()
        {
            var port = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Port));
            port.portName = "Child";
            return port;
        }

        public Type GetBehavior()
        {
            return dirtyNodeBehaviorType;
        }

        public void Commit(Stack<BTNode> stack)
        {
            OnCommit(stack);
            resolvers.ForEach(r => r.Commit(NodeBehavior));
            NodeBehavior.Description = this.description.value;
            NodeBehavior.GraphPosition = GetPosition();
            NodeBehavior.NotifyEditor = MarkAsExecuted;
            NodeBehavior.GUID = this.GUID;
        }
        protected abstract void OnCommit(Stack<BTNode> stack);

        public bool Validate(Stack<BTNode> stack)
        {
            var valid = GetBehavior() != null && OnValidate(stack);
            if (valid)
            {
                style.backgroundColor = new StyleColor(StyleKeyword.Null);
            }
            else
            {
                style.backgroundColor = Color.red;
            }
            return valid;
        }

        protected abstract bool OnValidate(Stack<BTNode> stack);
        /// <summary>
        ///  核心:设置结点行为类型
        /// </summary>
        /// <param name="nodeBehavior"></param>
        public void SetBehavior(System.Type nodeBehavior, ITreeView ownerTreeView = null)
        {
            if (ownerTreeView != null) this.mapTreeView = ownerTreeView;
            if (dirtyNodeBehaviorType != null)
            {
                dirtyNodeBehaviorType = null;
                container.Clear();
                resolvers.Clear();
            }
            dirtyNodeBehaviorType = nodeBehavior;

            nodeBehavior
                .GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(field => field.GetCustomAttribute<HideInEditorWindow>() == null)//根据Atrribute判断是否需要隐藏
                .Concat(GetAllFields(nodeBehavior))//Concat合并列表
                .Where(field => field.IsInitOnly == false)
                .ToList().ForEach((p) =>
                {
                    var fieldResolver = fieldResolverFactory.Create(p);//工厂创建暴露引用
                    var defaultValue = Activator.CreateInstance(nodeBehavior) as NodeBehavior;
                    fieldResolver.Restore(defaultValue);
                    container.Add(fieldResolver.GetEditorField(mapTreeView));
                    resolvers.Add(fieldResolver);
                });
            var label = nodeBehavior.GetCustomAttribute(typeof(EditorLabelAttribute), false) as EditorLabelAttribute;
            title = label?.Title ?? nodeBehavior.Name;
            Copiable = nodeBehavior.GetCustomAttribute(typeof(CopyDisableAttribute), false) == null;
            noValidate = nodeBehavior.GetCustomAttribute(typeof(NoValidateAttribute), false) != null;
        }

        private static IEnumerable<FieldInfo> GetAllFields(Type t)
        {
            if (t == null)
                return Enumerable.Empty<FieldInfo>();

            return t.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(field => field.GetCustomAttribute<SerializeField>() != null)
                .Where(field => field.GetCustomAttribute<HideInEditorWindow>() == null).Concat(GetAllFields(t.BaseType));//Concat合并列表
        }

        private void MarkAsExecuted(Status status)
        {
            switch (status)
            {
                case Status.Failure:
                    {
                        style.backgroundColor = Color.red;
                        break;
                    }
                case Status.Running:
                    {
                        style.backgroundColor = Color.yellow;
                        break;
                    }
                case Status.Success:
                    {
                        style.backgroundColor = Color.green;
                        break;
                    }
            }
        }

        public void ClearStyle()
        {
            style.backgroundColor = new StyleColor(StyleKeyword.Null);
            OnClearStyle();
        }

        protected abstract void OnClearStyle();
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.MenuItems().Add(new BehaviorTreeDropdownMenuAction("Duplicate", (a) =>
            {
                mapTreeView.DuplicateNode(this);
            }));
            evt.menu.MenuItems().Add(new BehaviorTreeDropdownMenuAction("Select Group", (a) =>
            {
                mapTreeView.SelectGroup(this);
                return;
            }));
            evt.menu.MenuItems().Add(new BehaviorTreeDropdownMenuAction("UnSelect Group", (a) =>
            {
                mapTreeView.UnSelectGroup();
                return;
            }));
        }

    }
}
