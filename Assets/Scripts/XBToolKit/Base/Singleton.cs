using System;
using System.Reflection;
using System.Linq;

namespace XBToolKit
{
    public abstract class Singleton<T> where T : class
    {
        private static readonly Lazy<T> LazyInstance = new(() =>
            {
                var ctorList = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (ctorList.Count() != 1)
                    throw new InvalidOperationException($"Type {typeof(T)} must have exactly one constructor.");
                var ctor = ctorList.SingleOrDefault(c => !c.GetParameters().Any() && c.IsPrivate);
                return ctor == null
                    ? throw new InvalidOperationException(
                        $"The constructor for {typeof(T)} must be private and take no parameters.")
                    : (T)ctor.Invoke(null);
            }, true
        );

        public static T Instance => LazyInstance.Value;
    }
}