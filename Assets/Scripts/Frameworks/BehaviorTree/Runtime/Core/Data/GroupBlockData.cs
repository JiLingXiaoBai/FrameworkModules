using System.Collections.Generic;
using UnityEngine;

namespace JLXB.Framework.BehaviorTree
{
    [System.Serializable]
    public class GroupBlockData
    {
        public List<string> ChildNodes = new List<string>();
        public Vector2 Position;
        public string Title = "New Block";
    }
}