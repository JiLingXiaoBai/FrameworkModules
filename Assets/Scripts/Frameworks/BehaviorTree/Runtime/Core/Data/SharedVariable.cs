using System;
using UnityEngine;

namespace JLXB.Framework.BehaviorTree
{
    [System.Serializable]
    public abstract class SharedVariable : ICloneable
    {
        public SharedVariable()
        {

        }

        [SerializeField]
        private bool isShared;

        /// <summary>
		/// 是否共享
		/// </summary>
        public bool IsShared
        {
            get => isShared;
            set => isShared = value;
        }

        [SerializeField]
        private string mName;

        public string Name
        {
            get => mName;
            set => mName = value;
        }

        public abstract object GetValue();
        public abstract void SetValue(object value);
        public abstract object Clone();
    }


    [System.Serializable]
    public abstract class SharedVariable<T> : SharedVariable
    {
        [SerializeField]
        private T value;

        public T Value
        {
            get => (this.Getter == null) ? this.value : this.Getter();
            set
            {
                if (this.Setter != null)
                    this.Setter(value);
                else
                    this.value = value;
            }
        }

        private Func<T> Getter;
        private Action<T> Setter;

        public sealed override object GetValue()
        {
            return this.Value;
        }

        public sealed override void SetValue(object value)
        {
            if (this.Setter != null)
            {
                this.Setter((T)((object)value));
            }
            else if (value is IConvertible)
            {
                this.value = (T)((object)Convert.ChangeType(value, typeof(T)));
            }
            else
            {
                this.value = (T)((object)value);
            }
        }

        public void Bind(SharedVariable<T> other)
        {
            this.Getter = () => other.Value;
            Setter = (arg) => other.Value = arg;
        }
    }
}