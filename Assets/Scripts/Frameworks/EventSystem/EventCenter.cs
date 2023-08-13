using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace JLXB.Framework.EventCenter
{
    public static class EventCenter
    {
        private interface IEventData
        {
        }

        private class EventData : IEventData
        {
            public UnityAction EventActions;

            public EventData(UnityAction action)
            {
                EventActions += action;
            }
        }

        private class EventData<T> : IEventData
        {
            public UnityAction<T> EventActions;

            public EventData(UnityAction<T> action)
            {
                EventActions += action;
            }
        }

        private class EventData<T0, T1> : IEventData
        {
            public UnityAction<T0, T1> EventActions;

            public EventData(UnityAction<T0, T1> action)
            {
                EventActions += action;
            }
        }

        private class EventData<T0, T1, T2> : IEventData
        {
            public UnityAction<T0, T1, T2> EventActions;

            public EventData(UnityAction<T0, T1, T2> action)
            {
                EventActions += action;
            }
        }

        private class EventData<T0, T1, T2, T3> : IEventData
        {
            public UnityAction<T0, T1, T2, T3> EventActions;

            public EventData(UnityAction<T0, T1, T2, T3> action)
            {
                EventActions += action;
            }
        }

        private static readonly Dictionary<string, IEventData> MEventTable = new();


        private static void OnRegistering(string key, IEventData eventData)
        {
            MEventTable.TryAdd(key, null);
            var data = MEventTable[key];
            if (data != null && data.GetType() != eventData.GetType())
            {
                throw new Exception(
                    $"尝试为事件 {key} 添加不同类型的委托，当前事件所对应的委托是 {data.GetType()}，要添加的委托类型为 {eventData.GetType()}");
            }
        }

        private static void OnRemoving(string key, IEventData eventData)
        {
            if (MEventTable.TryGetValue(key, out var data))
            {
                if (data == null)
                {
                    throw new Exception($"移除监听错误：事件 {key} 没有对应的委托");
                }

                if (data.GetType() != eventData.GetType())
                {
                    throw new Exception(
                        $"移除监听错误：尝试为事件 {key} 移除不同类型的委托，当前委托类型为 {data.GetType()}，要移除的委托类型为 {eventData.GetType()}");
                }
            }
            else
            {
                throw new Exception($"移除监听错误：没有事件码 {key}");
            }
        }

        private static void OnRemoved(string key)
        {
            if (MEventTable[key] == null)
            {
                MEventTable.Remove(key);
            }
        }

        //no parameters
        public static void Register(string key, UnityAction action)
        {
            OnRegistering(key, new EventData(action));

            if (MEventTable[key] == null)
            {
                MEventTable[key] = new EventData(action);
            }
            else
            {
                ((EventData)MEventTable[key]).EventActions += action;
            }
        }

        //single parameter
        public static void Register<T>(string key, UnityAction<T> action)
        {
            OnRegistering(key, new EventData<T>(action));

            if (MEventTable[key] == null)
            {
                MEventTable[key] = new EventData<T>(action);
            }
            else
            {
                ((EventData<T>)MEventTable[key]).EventActions += action;
            }
        }

        //two parameters
        public static void Register<T0, T1>(string key, UnityAction<T0, T1> action)
        {
            OnRegistering(key, new EventData<T0, T1>(action));

            if (MEventTable[key] == null)
            {
                MEventTable[key] = new EventData<T0, T1>(action);
            }
            else
            {
                ((EventData<T0, T1>)MEventTable[key]).EventActions += action;
            }
        }

        //three parameters
        public static void Register<T0, T1, T2>(string key, UnityAction<T0, T1, T2> action)
        {
            OnRegistering(key, new EventData<T0, T1, T2>(action));
            if (MEventTable[key] == null)
            {
                MEventTable[key] = new EventData<T0, T1, T2>(action);
            }
            else
            {
                ((EventData<T0, T1, T2>)MEventTable[key]).EventActions += action;
            }
        }

        //four parameters
        public static void Register<T0, T1, T2, T3>(string key, UnityAction<T0, T1, T2, T3> action)
        {
            OnRegistering(key, new EventData<T0, T1, T2, T3>(action));
            if (MEventTable[key] == null)
            {
                MEventTable[key] = new EventData<T0, T1, T2, T3>(action);
            }
            else
            {
                ((EventData<T0, T1, T2, T3>)MEventTable[key]).EventActions += action;
            }
        }


        //no parameters
        public static void Remove(string key, UnityAction action)
        {
            OnRemoving(key, new EventData(action));
            ((EventData)MEventTable[key]).EventActions -= action;
            OnRemoved(key);
        }

        //single parameter
        public static void Remove<T>(string key, UnityAction<T> action)
        {
            OnRemoving(key, new EventData<T>(action));
            ((EventData<T>)MEventTable[key]).EventActions -= action;
            OnRemoved(key);
        }

        //two parameters
        public static void Remove<T0, T1>(string key, UnityAction<T0, T1> action)
        {
            OnRemoving(key, new EventData<T0, T1>(action));
            ((EventData<T0, T1>)MEventTable[key]).EventActions -= action;
            OnRemoved(key);
        }

        //three parameters
        public static void Remove<T0, T1, T2>(string key, UnityAction<T0, T1, T2> action)
        {
            OnRemoving(key, new EventData<T0, T1, T2>(action));
            ((EventData<T0, T1, T2>)MEventTable[key]).EventActions -= action;
            OnRemoved(key);
        }

        //four parameters
        public static void Remove<T0, T1, T2, T3>(string key, UnityAction<T0, T1, T2, T3> action)
        {
            OnRemoving(key, new EventData<T0, T1, T2, T3>(action));
            ((EventData<T0, T1, T2, T3>)MEventTable[key]).EventActions -= action;
            OnRemoved(key);
        }

        public static void Clear()
        {
            MEventTable.Clear();
        }


        //no parameters
        public static void DispatchEvent(string key)
        {
            if (!MEventTable.TryGetValue(key, out var data)) return;
            if (data is EventData eventData)
            {
                eventData.EventActions.Invoke();
            }
            else
            {
                throw new Exception($"广播事件错误：事件 {key} 对应委托具有不同的类型");
            }
        }

        //single parameters
        public static void DispatchEvent<T>(string key, T arg)
        {
            if (!MEventTable.TryGetValue(key, out var data)) return;
            if (data is EventData<T> eventData)
            {
                eventData.EventActions.Invoke(arg);
            }
            else
            {
                throw new Exception($"广播事件错误：事件 {key} 对应委托具有不同的类型");
            }
        }

        //two parameters
        public static void DispatchEvent<T0, T1>(string key, T0 arg0, T1 arg1)
        {
            if (!MEventTable.TryGetValue(key, out var data)) return;
            if (data is EventData<T0, T1> eventData)
            {
                eventData.EventActions.Invoke(arg0, arg1);
            }
            else
            {
                throw new Exception($"广播事件错误：事件 {key} 对应委托具有不同的类型");
            }
        }

        //three parameters
        public static void DispatchEvent<T0, T1, T2>(string key, T0 arg0, T1 arg1, T2 arg2)
        {
            if (!MEventTable.TryGetValue(key, out var data)) return;
            if (data is EventData<T0, T1, T2> eventData)
            {
                eventData.EventActions.Invoke(arg0, arg1, arg2);
            }
            else
            {
                throw new Exception($"广播事件错误：事件 {key} 对应委托具有不同的类型");
            }
        }

        //four parameters
        public static void DispatchEvent<T0, T1, T2, T3>(string key, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            if (!MEventTable.TryGetValue(key, out var data)) return;
            if (data is EventData<T0, T1, T2, T3> eventData)
            {
                eventData.EventActions.Invoke(arg0, arg1, arg2, arg3);
            }
            else
            {
                throw new Exception($"广播事件错误：事件 {key} 对应委托具有不同的类型");
            }
        }
    }
}