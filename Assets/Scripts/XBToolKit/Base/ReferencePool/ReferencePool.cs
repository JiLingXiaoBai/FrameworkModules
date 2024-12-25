using System;
using System.Collections.Generic;

namespace XBToolKit
{
    public static partial class ReferencePool
    {
        private static readonly Dictionary<Type, ReferenceCollection> ReferenceCollections = new();

        public static void ClearAll()
        {
            lock (ReferenceCollections)
            {
                foreach (var referenceCollection in ReferenceCollections)
                {
                    referenceCollection.Value.RemoveAllReferences();
                }
                ReferenceCollections.Clear();
            }
        }

        public static void CheckTypeCount()
        {
            lock (ReferenceCollections)
            {
                foreach (var referenceCollection in ReferenceCollections)
                {
                    UnityEngine.Debug.Log("Type: " + referenceCollection.Key + " Count: " +
                                          referenceCollection.Value.Count);
                }
            }
        }


        public static T Acquire<T>() where T : class, IReference, new()
        {
            return GetReferenceCollection(typeof(T)).AcquireReference() as T;
        }

        public static void Release<T>(ref T reference) where T : class, IReference, new()
        {
            GetReferenceCollection(reference.GetType()).ReleaseReference(reference);
            reference = null;
        }

        public static IReference Acquire(Type referenceType)
        {
            InternalCheckReferenceType(referenceType);
            return GetReferenceCollection(referenceType).AcquireReference();
        }

        public static void Release(ref IReference reference)
        {
            if (reference == null)
            {
                throw new Exception("Reference is invalid.");
            }
            var referenceType = reference.GetType();
            InternalCheckReferenceType(referenceType);
            GetReferenceCollection(referenceType).ReleaseReference(reference);
            reference = null;
        }

        public static void Add<T>(int count) where T : class, IReference, new()
        {
            GetReferenceCollection(typeof(T)).AddReference(count);
        }

        public static void Add(Type referenceType, int count)
        {
            InternalCheckReferenceType(referenceType);
            GetReferenceCollection(referenceType).AddReference(count);
        }

        public static void Remove<T>(int count) where T : class, IReference
        {
            GetReferenceCollection(typeof(T)).RemoveReference(count);
        }

        public static void Remove(Type referenceType, int count)
        {
            InternalCheckReferenceType(referenceType);
            GetReferenceCollection(referenceType).RemoveReference(count);
        }

        public static void RemoveAll<T>() where T : class, IReference
        {
            GetReferenceCollection(typeof(T)).RemoveAllReferences();
        }

        public static void RemoveAll(Type referenceType)
        {
            InternalCheckReferenceType(referenceType);
            GetReferenceCollection(referenceType).RemoveAllReferences();
        }

        private static void InternalCheckReferenceType(Type referenceType)
        {
#if UNITY_EDITOR
            if (referenceType == null)
                throw new Exception("ReferenceType is invalid.");
            if (!referenceType.IsClass || referenceType.IsAbstract)
                throw new Exception("Reference Type is not a non-abstract class type.");
            if (!typeof(IReference).IsAssignableFrom(referenceType))
                throw new Exception($"Reference Type '{referenceType.FullName}' is invalid.");
#endif
        }

        private static ReferenceCollection GetReferenceCollection(Type referenceType)
        {
            if (referenceType == null)
                throw new Exception("ReferenceType is invalid.");

            ReferenceCollection referenceCollection;
            lock (ReferenceCollections)
            {
                if (ReferenceCollections.TryGetValue(referenceType, out referenceCollection))
                    return referenceCollection;
                referenceCollection = new ReferenceCollection(referenceType);
                ReferenceCollections.Add(referenceType, referenceCollection);
            }
            return referenceCollection;
        }
    }
}