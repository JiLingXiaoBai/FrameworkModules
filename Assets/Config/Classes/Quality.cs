using JLXB.Framework.Config;

public class Quality : ConfigDataTable<int, Quality.ConfigData>
{
    [System.Serializable]
    public class ConfigData : ConfigDataBase
    {
		/// <summary>
		/// 编号
		/// </summary>
		public int id;
		/// <summary>
		/// 名称
		/// </summary>
		public string name;
		/// <summary>
		/// 颜色
		/// </summary>
		public string color;
		/// <summary>
		/// 说明
		/// </summary>
		public string description;

    }
}