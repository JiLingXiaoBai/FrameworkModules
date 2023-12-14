
using System;
using System.Collections.Generic;

namespace Tools.Config.Editor
{
    public sealed class ColumnInfoCache : ConfigDataTable<string, ColumnInfoCache.ConfigData>
    {
        [Serializable]
        public class ConfigData : ConfigDataBase
        {
            public List<ColumnInfo> cacheInfo;
            public ConfigData(List<ColumnInfo> cacheInfo)
            {
                this.cacheInfo = cacheInfo;
            }
        }
        
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