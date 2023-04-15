namespace JLXB.Framework
{
    public class Singleton<T> where T : class, new()
    {
        private static T _instance = null;
        private static readonly object padlock = new object();

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (padlock)
                    {
                        if (_instance == null)
                        {
                            _instance = new T();
                        }
                    }
                }
                return _instance;
            }
        }
    }
}
