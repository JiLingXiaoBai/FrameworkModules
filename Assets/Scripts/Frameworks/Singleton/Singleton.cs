using System;
using System.Reflection;
using System.Linq;

namespace JLXB.Framework
{
    public abstract class Singleton<T> where T : class
    {
        private static readonly Lazy<T> _instance = new(() =>
            {
                var ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (ctors.Count() != 1)
                    throw new InvalidOperationException(String.Format("Type {0} must have exactly one constructor.", typeof(T)));
                var ctor = ctors.SingleOrDefault(c => !c.GetParameters().Any() && c.IsPrivate);
                return ctor == null
                    ? throw new InvalidOperationException(String.Format("The constructor for {0} must be private and take no parameters.", typeof(T)))
                    : (T)ctor.Invoke(null);
            }, true
        );

        public static T Instance
        {
            get
            {
                return _instance.Value;
            }
        }
    }
}
