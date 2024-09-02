using System;
using System.Collections.Generic;

namespace XBToolKit
{
    public static partial class ReferencePool
    {
        private sealed class ReferenceCollection
        {
            private readonly Queue<IReference> _references;
            private readonly Type _referenceType;

            public ReferenceCollection(Type referenceType)
            {
                _references = new Queue<IReference>();
                _referenceType = referenceType;
            }

            public IReference AcquireReference()
            {
                lock (_references)
                {
                    if (_references.Count > 0)
                    {
                        return _references.Dequeue();
                    }
                }
                return (IReference)Activator.CreateInstance(_referenceType);
            }

            public void ReleaseReference(IReference reference)
            {
                reference.Clear();
                lock (_references)
                {
                    if (_references.Contains(reference))
                    {
                        throw new Exception("The reference has been released.");
                    }
                    _references.Enqueue(reference);
                }
            }


            public void AddReference(int count)
            {
                lock (_references)
                {
                    while (count-- > 0)
                    {
                        _references.Enqueue((IReference)Activator.CreateInstance(_referenceType));
                    }
                }
            }

            public void RemoveReference(int count)
            {
                lock (_references)
                {
                    if (count > _references.Count)
                    {
                        count = _references.Count;
                    }
                    while (count-- > 0)
                    {
                        _references.Dequeue();
                    }
                }
            }

            public void RemoveAllReferences()
            {
                lock (_references)
                {
                    _references.Clear();
                }
            }
        }
    }
}