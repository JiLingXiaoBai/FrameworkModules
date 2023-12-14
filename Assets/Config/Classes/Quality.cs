using Tools.Config;

public sealed class Quality : ConfigDataTable<string, Quality.ConfigData>
{
    [System.Serializable]
    public class ConfigData : ConfigDataBase
    {
		/// <summary>
		/// 编号
		/// </summary>
		public string id;
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
		/// <summary>
		/// test_float
		/// </summary>
		public float test1;
		/// <summary>
		/// test_double
		/// </summary>
		public double test2;
		/// <summary>
		/// test_bool
		/// </summary>
		public bool test3;
		/// <summary>
		/// test_intArr
		/// </summary>
		public int[] test4;
		/// <summary>
		/// test_stringArr
		/// </summary>
		public string[] test5;
		/// <summary>
		/// test_floatArr
		/// </summary>
		public float[] test6;
		/// <summary>
		/// test_doubleArr
		/// </summary>
		public double[] test7;
		/// <summary>
		/// test_boolArr
		/// </summary>
		public bool[] test8;

    }
}