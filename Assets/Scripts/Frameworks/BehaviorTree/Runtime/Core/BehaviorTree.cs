using System.Collections.Generic;
using UnityEngine;

namespace JLXB.Framework.BehaviorTree
{

    public enum UpdateType
    {
        [InspectorName("自动模式")]
        Auto,
        [InspectorName("手动模式")]
        Manual
    }

    /// <summary>
    /// Behavior Tree Component
    /// Awake, Start and Update using UnityEngine's life cycle
    /// </summary>
    [DisallowMultipleComponent]
    public class BehaviorTree : MonoBehaviour, IBehaviorTree
    {
        [SerializeReference, HideInInspector]
        private Root root = new Root();
        [SerializeReference, HideInInspector]
        private List<SharedVariable> sharedVariables = new List<SharedVariable>();
        [SerializeField, HideInInspector]
        private List<GroupBlockData> blockDatas = new List<GroupBlockData>();
        [SerializeField, Tooltip("切换成UpdateType.Manual使用手动更新并且调用BehaviorTree.Tick()")]
        private UpdateType updateType;
        [SerializeField, Tooltip("使用外部行为树替换组件内行为树,保存时会覆盖组件内行为树")]
        private BehaviorTreeSO externalBehaviorTree;
        [SerializeField, HideInInspector]
        private bool autoSave;
        [SerializeField, HideInInspector]
        private string savePath = "Assets";

        #region Implementation of interface
        public Object _Object => gameObject;

        public Root Root
        {
            get => root;
#if UNITY_EDITOR
            set => root = value;
#endif
        }

        public List<SharedVariable> SharedVariables
        {
            get => sharedVariables;
#if UNITY_EDITOR
            set => sharedVariables = value;
#endif
        }

#if UNITY_EDITOR
        public string SavePath
        {
            get => savePath;
            set => savePath = value;
        }
        public bool AutoSave
        {
            get => autoSave;
            set => autoSave = value;
        }

        public BehaviorTreeSO ExternalBehaviorTree => externalBehaviorTree;

        public List<GroupBlockData> BlockDatas { get => blockDatas; set => blockDatas = value; }
#endif
        public SharedVariable<T> GetSharedVariable<T>(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError($"共享变量名称不能为空", this);
                return null;
            }
            foreach (var variable in sharedVariables)
            {
                if (variable.Name.Equals(name))
                {
                    if (variable is SharedVariable<T>) return variable as SharedVariable<T>;
                    else Debug.LogError($"{name}名称变量不是{typeof(T).Name}类型", this);
                    return null;
                }
            }
            Debug.LogError($"没有找到共享变量:{name}", this);
            return null;
        }

        #endregion

        private void Awake()
        {
            root.Run(gameObject, this);
            root.Awake();
        }

        private void Start()
        {
            root.Start();
        }

        private void Update()
        {
            if (updateType == UpdateType.Auto) Tick();
        }

        public void Tick()
        {
            root.PreUpdate();
            root.Update();
            root.PostUpdate();
        }
    }
}