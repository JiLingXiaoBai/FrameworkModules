using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System.Reflection;
using System;
namespace JLXB.Framework.BehaviorTree.Editor
{
    public class BehaviorTreeView : GraphView, ITreeView
    {
        private readonly struct EdgePair
        {
            public readonly NodeBehavior NodeBehavior;
            public readonly Port ParentOutputPort;

            public EdgePair(NodeBehavior nodeBehavior, Port parentOutputPort)
            {
                NodeBehavior = nodeBehavior;
                ParentOutputPort = parentOutputPort;
            }
        }
        public Blackboard _blackboard;
        protected readonly IBehaviorTree behaviorTree;
        protected RootNode root;
        private List<SharedVariable> exposedProperties = new List<SharedVariable>();
        public List<SharedVariable> ExposedProperties => exposedProperties;
        private FieldResolverFactory fieldResolverFactory = FieldResolverFactory.Instance;
        protected NodeSearchWindowProvider provider;
        public event System.Action<SharedVariable> OnPropertyNameChangeEvent;
        public event System.Action<SharedVariable> OnPropertyNameEditingEvent;
        public bool AutoSave
        {
            get => behaviorTree.AutoSave;
            set => behaviorTree.AutoSave = value;
        }
        public string SavePath
        {
            get => behaviorTree.SavePath;
            set => behaviorTree.SavePath = value;
        }
        /// <summary>
        /// 是否可以保存为SO,如果可以会在ToolBar中提供按钮
        /// </summary>
        public virtual bool CanSaveToSO => behaviorTree is BehaviorTree;
        public virtual string treeEditorName => "BehaviorTree";
        private readonly NodeResolver nodeResolver = new NodeResolver();
        /// <summary>
        /// 结点选择委托
        /// </summary>
        public System.Action<BTNode> onSelectAction;
        private readonly EditorWindow _window;
        public bool IsRestored { get; private set; }
        private readonly BehaviorNodeConverter converter = new BehaviorNodeConverter();
        private readonly DragDropManipulator dragDropManipulator;
        /// <summary>
        /// 黑板
        /// </summary>
        /// <returns></returns>
        public BehaviorTreeView(IBehaviorTree bt, EditorWindow editor)
        {
            _window = editor;
            behaviorTree = bt;
            style.flexGrow = 1;
            style.flexShrink = 1;
            styleSheets.Add(BehaviorTreeSetting.GetGraphStyle(treeEditorName));
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            Insert(0, new GridBackground());

            var contentDragger = new ContentDragger();
            //鼠标中键移动
            contentDragger.activators.Add(new ManipulatorActivationFilter()
            {
                button = MouseButton.MiddleMouse,
            });
            // 添加选框
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new FreehandSelector());
            this.AddManipulator(contentDragger);
            dragDropManipulator = new DragDropManipulator(this);
            dragDropManipulator.OnDragOverEvent += CopyFromObject;
            this.AddManipulator(dragDropManipulator);
            provider = ScriptableObject.CreateInstance<NodeSearchWindowProvider>();
            provider.Initialize(this, editor, BehaviorTreeSetting.GetMask(treeEditorName));

            nodeCreationRequest += context =>
            {
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), provider);
            };
            serializeGraphElements += CopyOperation;
            canPasteSerializedData += (data) => true;
            unserializeAndPaste += OnPaste;
        }
        private string CopyOperation(IEnumerable<GraphElement> elements)
        {
            ClearSelection();
            foreach (GraphElement n in elements)
            {
                if (n is BTNode && !(n as BTNode).Copiable) continue;
                AddToSelection(n);
            }
            return "Copy Nodes";
        }
        /// <summary>
        /// 粘贴结点
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        private void OnPaste(string a, string b)
        {
            List<ISelectable> copyElements = new CopyPasteGraph(this, selection).GetCopyElements();
            ClearSelection();
            //再次选择
            copyElements.ForEach(node =>
            {
                node.Select(this, true);
            });
        }
        public BTNode DuplicateNode(BTNode node)
        {
            var newNode = nodeResolver.CreateNodeInstance(node.GetBehavior(), this) as BTNode;
            Rect newRect = node.GetPosition();
            newRect.position += new Vector2(50, 50);
            newNode.SetPosition(newRect);
            newNode.onSelectAction = onSelectAction;
            AddElement(newNode);
            newNode.CopyFrom(node);
            return newNode;
        }
        public GroupBlock CreateBlock(Rect rect, GroupBlockData blockData = null)
        {
            if (blockData == null) blockData = new GroupBlockData();
            var group = new GroupBlock
            {
                autoUpdateGeometry = true,
                title = blockData.Title
            };
            AddElement(group);
            group.SetPosition(rect);
            return group;
        }
        internal void NotifyEditSharedVariable(SharedVariable variable)
        {
            OnPropertyNameEditingEvent?.Invoke(variable);
        }
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
            var remainTargets = evt.menu.MenuItems().FindAll(e =>
            {
                switch (e)
                {
                    case BehaviorTreeDropdownMenuAction _: return true;
                    case DropdownMenuAction a: return a.name == "Create Node" || a.name == "Delete";
                    default: return false;
                }
            });
            //Remove needless default actions .
            evt.menu.MenuItems().Clear();
            remainTargets.ForEach(evt.menu.MenuItems().Add);
        }
        public void AddPropertyToBlackBoard(SharedVariable variable)
        {
            var localPropertyValue = variable.GetValue();
            if (String.IsNullOrEmpty(variable.Name)) variable.Name = variable.GetType().Name;
            var localPropertyName = variable.Name;
            int index = 1;
            while (ExposedProperties.Any(x => x.Name == localPropertyName))
            {
                localPropertyName = $"{variable.Name}{index}";
                index++;
            }
            variable.Name = localPropertyName;
            ExposedProperties.Add(variable);
            var container = new VisualElement();
            var field = new BlackboardField { text = localPropertyName, typeText = variable.GetType().Name };
            field.capabilities &= ~Capabilities.Deletable;
            field.capabilities &= ~Capabilities.Movable;
            container.Add(field);
            FieldInfo info = variable.GetType().GetField("value", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            var fieldResolver = fieldResolverFactory.Create(info);
            var valueField = fieldResolver.GetEditorField(ExposedProperties, variable);
            var sa = new BlackboardRow(field, valueField);
            sa.AddManipulator(new ContextualMenuManipulator((evt) => BuildBlackboardMenu(evt, container, variable)));
            container.Add(sa);
            _blackboard.Add(container);
        }
        private void BuildBlackboardMenu(ContextualMenuPopulateEvent evt, VisualElement element, SharedVariable variable)
        {
            evt.menu.MenuItems().Add(new BehaviorTreeDropdownMenuAction("Delate Variable", (a) =>
            {
                int index = _blackboard.contentContainer.IndexOf(element);
                ExposedProperties.RemoveAt(index - 1);
                _blackboard.Remove(element);
                return;
            }));
            evt.menu.MenuItems().Add(new BehaviorTreeDropdownMenuAction("Update All Reference Node", (a) =>
            {
                OnPropertyNameChangeEvent?.Invoke(variable);
            }));
        }
        public override List<Port> GetCompatiblePorts(Port startAnchor, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            foreach (var port in ports.ToList())
            {
                if (startAnchor.node == port.node ||
                    startAnchor.direction == port.direction ||
                    startAnchor.portType != port.portType)
                {
                    continue;
                }

                compatiblePorts.Add(port);
            }
            return compatiblePorts;
        }
        private void CopyFromObject(UnityEngine.Object data, Vector3 mousePosition)
        {
            if (data is GameObject)
            {
                if ((data as GameObject).TryGetComponent<IBehaviorTree>(out IBehaviorTree tree))
                {
                    _window.ShowNotification(new GUIContent("GameObject Dropped Successfully!"));
                    CopyFromOtherTree(tree, mousePosition);
                    return;
                }
                _window.ShowNotification(new GUIContent("Invalid Drag GameObject!"));
                return;
            }
            if (data is not IBehaviorTree)
            {
                _window.ShowNotification(new GUIContent("Invalid Drag Data!"));
                return;
            }
            _window.ShowNotification(new GUIContent("Data Dropped Successfully!"));
            CopyFromOtherTree(data as IBehaviorTree, mousePosition);
        }
        internal void CopyFromOtherTree(IBehaviorTree otherTree, Vector2 mousePosition)
        {
            var localMousePosition = contentViewContainer.WorldToLocal(mousePosition) - new Vector2(400, 300);
            IsRestored = true;
            IEnumerable<BTNode> nodes;
            RootNode rootNode;
            foreach (var variable in otherTree.SharedVariables)
            {
                AddPropertyToBlackBoard(variable.Clone() as SharedVariable);//Clone新的共享变量
            }
            (rootNode, nodes) = converter.ConvertToNode(otherTree, this, localMousePosition);
            foreach (var node in nodes) node.onSelectAction = onSelectAction;
            var edge = rootNode.Child.connections.First();
            RemoveElement(edge);
            RemoveElement(rootNode);
            foreach (var nodeBlockData in otherTree.BlockDatas)
            {
                CreateBlock(new Rect(nodeBlockData.Position, new Vector2(100, 100)), nodeBlockData)
                .AddElements(nodes.Where(x => nodeBlockData.ChildNodes.Contains(x.GUID)));
            }
            IsRestored = false;
        }
        /// <summary>
        /// 重铸行为树
        /// </summary>
        internal void Restore()
        {
            IBehaviorTree tree = behaviorTree.ExternalBehaviorTree ?? behaviorTree;
            foreach (var variable in tree.SharedVariables)
            {
                AddPropertyToBlackBoard(variable.Clone() as SharedVariable);//Clone新的共享变量
            }
            IsRestored = true;
            IEnumerable<BTNode> nodes;
            (this.root, nodes) = converter.ConvertToNode(tree, this, Vector2.zero);
            foreach (var node in nodes) node.onSelectAction = onSelectAction;
            foreach (var nodeBlockData in tree.BlockDatas)
            {
                CreateBlock(new Rect(nodeBlockData.Position, new Vector2(100, 100)), nodeBlockData)
                .AddElements(nodes.Where(x => nodeBlockData.ChildNodes.Contains(x.GUID)));
            }
            IsRestored = false;
        }
        void ITreeView.SelectGroup(BTNode node)
        {
            var block = CreateBlock(new Rect(node.transform.position, new Vector2(100, 100)));
            foreach (var select in selection)
            {
                if (select is not BTNode || select is RootNode) continue;
                block.AddElement(select as BTNode);
            }
        }
        void ITreeView.UnSelectGroup()
        {
            foreach (var select in selection)
            {
                if (select is not BTNode) continue;
                var node = select as BTNode;
                var block = graphElements.ToList().Where(x => x is GroupBlock)?.Cast<GroupBlock>()?.FirstOrDefault(x => x.ContainsElement(node));
                if (block != null) block.RemoveElement(node);
            }
        }

        internal bool Save(bool autoSave = false)
        {
            if (Application.isPlaying) return false;
            if (Validate())
            {
                Commit();
                if (autoSave) Debug.Log($"<color=#3aff48>{treeEditorName}</color>[{behaviorTree._Object.name}]自动保存成功{System.DateTime.Now.ToString()}");
                AssetDatabase.SaveAssets();
                return true;
            }
            if (autoSave) Debug.Log($"<color=#ff2f2f>{treeEditorName}</color>[{behaviorTree._Object.name}]自动保存失败{System.DateTime.Now.ToString()}");
            return false;
        }

        protected virtual bool Validate()
        {
            //validate nodes by DFS.
            var stack = new Stack<BTNode>();
            stack.Push(root);
            while (stack.Count > 0)
            {
                var node = stack.Pop();
                if (!node.Validate(stack))
                {
                    return false;
                }
            }
            return true;
        }
        private void Commit()
        {
            Commit(behaviorTree);
        }
        internal void Commit(IBehaviorTree behaviorTree)
        {
            var stack = new Stack<BTNode>();
            stack.Push(root);

            // save new components
            while (stack.Count > 0)
            {
                var node = stack.Pop();
                node.Commit(stack);
            }

            root.PostCommit(behaviorTree);
            behaviorTree.SharedVariables.Clear();
            foreach (var sharedVariable in ExposedProperties)
            {
                behaviorTree.SharedVariables.Add(sharedVariable);
            }
            List<GroupBlock> NodeBlocks = graphElements.ToList().Where(x => x is GroupBlock).Cast<GroupBlock>().ToList();
            behaviorTree.BlockDatas.Clear();
            foreach (var block in NodeBlocks)
            {
                var nodes = block.containedElements.Where(x => x is BTNode).Cast<BTNode>().Select(x => x.GUID)
                    .ToList();

                behaviorTree.BlockDatas.Add(new GroupBlockData
                {
                    ChildNodes = nodes,
                    Title = block.title,
                    Position = block.GetPosition().position
                });
            }
            // notify to unity editor that the tree is changed.
            EditorUtility.SetDirty(behaviorTree._Object);
        }
        internal static Edge ConnectPorts(Port output, Port input)
        {
            var tempEdge = new Edge
            {
                output = output,
                input = input
            };
            tempEdge.input.Connect(tempEdge);
            tempEdge.output.Connect(tempEdge);
            return tempEdge;
        }
    }
}