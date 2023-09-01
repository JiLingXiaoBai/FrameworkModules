
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JLXB.Framework.Config.Editor
{
    [Serializable]
    public class ColumnInfoCache : ScriptableObject
    {
        public Dictionary<string, List<ColumnInfo>> Cache;
    }
    
    /// <summary>
    /// 列信息容器
    /// </summary>
    [Serializable]
    public class ColumnInfo
    {
        public string name;
        public string description;
        public string type;
        public List<string> dataList;
    }
}