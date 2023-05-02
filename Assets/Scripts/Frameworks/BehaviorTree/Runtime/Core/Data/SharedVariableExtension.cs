
using UnityEngine;
namespace JLXB.Framework.BehaviorTree
{
    public static class SharedVariableExtension
    {
        /// <summary>
        /// 从行为树获取共享变量
        /// </summary>
        public static SharedVariable<T> GetValueFromTree<T>(this SharedVariable<T> variable, IBehaviorTree tree)
        {
            if (variable == null) return null;
            if (!variable.IsShared) return variable;
            var value = tree.GetSharedVariable<T>(variable.Name);
            if (value != null) variable.Bind(value);
            else Debug.LogWarning($"{variable.Name}并非有效共享变量");
            return variable;
        }
    }
}