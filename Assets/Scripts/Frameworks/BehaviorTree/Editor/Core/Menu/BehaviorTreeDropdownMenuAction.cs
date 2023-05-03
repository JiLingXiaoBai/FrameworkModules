using System.Collections;
using System;
using UnityEngine.UIElements;

namespace JLXB.Framework.BehaviorTree.Editor
{
    public class BehaviorTreeDropdownMenuAction : DropdownMenuAction
    {
        public BehaviorTreeDropdownMenuAction(
            string actionName,
            Action<DropdownMenuAction> actionCallback,
            Func<DropdownMenuAction, Status> actionStatusCallback,
            object userData = null
        ) : base(actionName, actionCallback, actionStatusCallback, userData)
        {
        }

        public BehaviorTreeDropdownMenuAction(
            string actionName,
            Action<DropdownMenuAction> actionCallback
        ) : this(actionName, actionCallback, (e) => DropdownMenuAction.Status.Normal, null)
        {
        }
    }
}