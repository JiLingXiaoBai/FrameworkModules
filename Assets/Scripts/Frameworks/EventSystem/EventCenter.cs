using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace JLXB.Framework.Event
{
    public sealed class EventCenter : Singleton<EventCenter>
    {
        private Dictionary<string, IEventData> m_EventTable;

        public EventCenter()
        {
            m_EventTable = new Dictionary<string, IEventData>();
        }

        private void OnRegistering(string key, IEventData eventData)
        {
            if (!m_EventTable.ContainsKey(key))
            {
                m_EventTable.Add(key, null);
            }
            IEventData data = m_EventTable[key];
            if (data != null && data.GetType() != eventData.GetType())
            {
                throw new Exception(string.Format("尝试为事件 {0} 添加不同类型的委托，当前事件所对应的委托是 {1}，要添加的委托类型为 {2}", key, data.GetType(), eventData.GetType()));
            }
        }

        private void OnRemoving(string key, IEventData eventData)
        {
            if (m_EventTable.ContainsKey(key))
            {
                IEventData data = m_EventTable[key];
                if (data == null)
                {
                    throw new Exception(string.Format("移除监听错误：事件 {0} 没有对应的委托", key));
                }
                else if (data.GetType() != eventData.GetType())
                {
                    throw new Exception(string.Format("移除监听错误：尝试为事件 {0} 移除不同类型的委托，当前委托类型为 {1}，要移除的委托类型为 {2}", key, data.GetType(), eventData.GetType()));
                }
            }
            else
            {
                throw new Exception(string.Format("移除监听错误：没有事件码 {0}", key));
            }
        }

        private void OnRemoved(string key)
        {
            if (m_EventTable[key] == null)
            {
                m_EventTable.Remove(key);
            }
        }

        //no parameters
        public void Register(string key, UnityAction action)
        {
            OnRegistering(key, new EventData(action));

            if (m_EventTable[key] == null)
            {
                m_EventTable[key] = new EventData(action);
            }
            else
            {
                (m_EventTable[key] as EventData).eventActions += action;
            }
        }

        //single parameter
        public void Register<T>(string key, UnityAction<T> action)
        {
            OnRegistering(key, new EventData<T>(action));

            if (m_EventTable[key] == null)
            {
                m_EventTable[key] = new EventData<T>(action);
            }
            else
            {
                (m_EventTable[key] as EventData<T>).eventActions += action;
            }
        }

        //two parameters
        public void Register<T0, T1>(string key, UnityAction<T0, T1> action)
        {
            OnRegistering(key, new EventData<T0, T1>(action));

            if (m_EventTable[key] == null)
            {
                m_EventTable[key] = new EventData<T0, T1>(action);
            }
            else
            {
                (m_EventTable[key] as EventData<T0, T1>).eventActions += action;
            }
        }

        //three parameters
        public void Register<T0, T1, T2>(string key, UnityAction<T0, T1, T2> action)
        {
            OnRegistering(key, new EventData<T0, T1, T2>(action));
            if (m_EventTable[key] == null)
            {
                m_EventTable[key] = new EventData<T0, T1, T2>(action);
            }
            else
            {
                (m_EventTable[key] as EventData<T0, T1, T2>).eventActions += action;
            }
        }

        //four parameters
        public void Register<T0, T1, T2, T3>(string key, UnityAction<T0, T1, T2, T3> action)
        {
            OnRegistering(key, new EventData<T0, T1, T2, T3>(action));
            if (m_EventTable[key] == null)
            {
                m_EventTable[key] = new EventData<T0, T1, T2, T3>(action);
            }
            else
            {
                (m_EventTable[key] as EventData<T0, T1, T2, T3>).eventActions += action;
            }
        }



        //no parameters
        public void Remove(string key, UnityAction action)
        {
            OnRemoving(key, new EventData(action));
            (m_EventTable[key] as EventData).eventActions -= action;
            OnRemoved(key);
        }

        //single parameter
        public void Remove<T>(string key, UnityAction<T> action)
        {
            OnRemoving(key, new EventData<T>(action));
            (m_EventTable[key] as EventData<T>).eventActions -= action;
            OnRemoved(key);
        }

        //two parameters
        public void Remove<T0, T1>(string key, UnityAction<T0, T1> action)
        {
            OnRemoving(key, new EventData<T0, T1>(action));
            (m_EventTable[key] as EventData<T0, T1>).eventActions -= action;
            OnRemoved(key);
        }

        //three parameters
        public void Remove<T0, T1, T2>(string key, UnityAction<T0, T1, T2> action)
        {
            OnRemoving(key, new EventData<T0, T1, T2>(action));
            (m_EventTable[key] as EventData<T0, T1, T2>).eventActions -= action;
            OnRemoved(key);
        }

        //four parameters
        public void Remove<T0, T1, T2, T3>(string key, UnityAction<T0, T1, T2, T3> action)
        {
            OnRemoving(key, new EventData<T0, T1, T2, T3>(action));
            (m_EventTable[key] as EventData<T0, T1, T2, T3>).eventActions -= action;
            OnRemoved(key);
        }

        public void Clear()
        {
            m_EventTable.Clear();
        }


        //no parameters
        public void DispatchEvent(string key)
        {

            if (m_EventTable.TryGetValue(key, out IEventData data))
            {
                if (data is EventData eventData)
                {
                    eventData.eventActions.Invoke();
                }
                else
                {
                    throw new Exception(string.Format("广播事件错误：事件 {0} 对应委托具有不同的类型", key));
                }
            }
        }

        //single parameters
        public void DispatchEvent<T>(string key, T arg)
        {

            if (m_EventTable.TryGetValue(key, out IEventData data))
            {
                if (data is EventData<T> eventData)
                {
                    eventData.eventActions.Invoke(arg);
                }
                else
                {
                    throw new Exception(string.Format("广播事件错误：事件 {0} 对应委托具有不同的类型", key));
                }
            }
        }

        //two parameters
        public void DispatchEvent<T0, T1>(string key, T0 arg0, T1 arg1)
        {

            if (m_EventTable.TryGetValue(key, out IEventData data))
            {

                if (data is EventData<T0, T1> eventData)
                {
                    eventData.eventActions.Invoke(arg0, arg1);
                }
                else
                {
                    throw new Exception(string.Format("广播事件错误：事件 {0} 对应委托具有不同的类型", key));
                }
            }
        }

        //three parameters
        public void DispatchEvent<T0, T1, T2>(string key, T0 arg0, T1 arg1, T2 arg2)
        {

            if (m_EventTable.TryGetValue(key, out IEventData data))
            {
                if (data is EventData<T0, T1, T2> eventData)
                {
                    eventData.eventActions.Invoke(arg0, arg1, arg2);
                }
                else
                {
                    throw new Exception(string.Format("广播事件错误：事件 {0} 对应委托具有不同的类型", key));
                }
            }
        }

        //four parameters
        public void DispatchEvent<T0, T1, T2, T3>(string key, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {

            if (m_EventTable.TryGetValue(key, out IEventData data))
            {
                if (data is EventData<T0, T1, T2, T3> eventData)
                {
                    eventData.eventActions.Invoke(arg0, arg1, arg2, arg3);
                }
                else
                {
                    throw new Exception(string.Format("广播事件错误：事件 {0} 对应委托具有不同的类型", key));
                }
            }
        }

    }
}
