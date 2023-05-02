using System.Collections.Generic;
using UnityEngine;
namespace JLXB.Framework.BehaviorTree
{
    public interface IBehaviorTree
    {
        public Object _Object { get; }
        public Root Root
        {
            get;
#if UNITY_EDITOR
            set;
#endif
        }

        public List<SharedVariable> SharedVariables
        {
            get;
#if UNITY_EDITOR
            set;
#endif
        }

#if UNITY_EDITOR
        /// <summary>
        /// Editor Only
        /// </summary>
        public string SavePath { get; set; }
        /// <summary>
        /// Editor Only
        /// </summary>
        public bool AutoSave { get; set; }
        /// <summary>
        /// Editor Only
        /// </summary>
        // public BehaviorTreeSO ExternalBehaviorTree { get; }
        /// <summary>
        /// Editor Only
        /// </summary>
        public List<GroupBlockData> BlockDatas { get; set; }
#endif
        /// <summary>
        /// 获取共享变量
        /// Get SharedVariable
        /// </summary>
        /// <param name="name">Variable Name</param>
        /// <typeparam name="T">Variabe Type</typeparam>
        SharedVariable<T> GetSharedVariable<T>(string name);
    }
}