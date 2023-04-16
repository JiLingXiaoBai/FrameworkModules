using System.Collections.Generic;
using UnityEngine.Events;

namespace JLXB.Framework.Event
{
    public interface IEventData
    {

    }

    public class EventData : IEventData
    {
        public UnityAction eventActions;
        public EventData(UnityAction action)
        {
            eventActions += action;
        }
    }

    public class EventData<T> : IEventData
    {
        public UnityAction<T> eventActions;
        public EventData(UnityAction<T> action)
        {
            eventActions += action;
        }
    }

    public class EventData<T0, T1> : IEventData
    {
        public UnityAction<T0, T1> eventActions;
        public EventData(UnityAction<T0, T1> action)
        {
            eventActions += action;
        }
    }

    public class EventData<T0, T1, T2> : IEventData
    {
        public UnityAction<T0, T1, T2> eventActions;
        public EventData(UnityAction<T0, T1, T2> action)
        {
            eventActions += action;
        }
    }

    public class EventData<T0, T1, T2, T3> : IEventData
    {
        public UnityAction<T0, T1, T2, T3> eventActions;
        public EventData(UnityAction<T0, T1, T2, T3> action)
        {
            eventActions += action;
        }
    }

}
