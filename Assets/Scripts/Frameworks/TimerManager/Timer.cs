using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{

    //定时器数据类
    public class TimerTask
    {
        /// <summary>
        /// TimerTask的唯一标识
        /// </summary>
        public ulong ID;
        /// <summary>
        /// TimerTask的触发时间间隔
        /// </summary>
        public float lifeCycle;
        /// <summary>
        /// TimerTask的到期时间,即触发的时间加上设定的触发时间间隔
        /// </summary>
        public float expirationTime;
        /// <summary>
        /// TimerTask的重复触发次数
        /// </summary>
        public long times;
        /// <summary>
        /// TimerTask的回调函数
        /// </summary>
        public UnityAction func;

        //你可以通过此方法来获取定时器的运行进度  0 ~ 1  1.0表示即将要调用func
        //你可以通过 GetTimer( id ) 获取当前Task的Clone体 
        public float progress
        {
            get
            {
                return 1.0f - Mathf.Clamp01((expirationTime - Time.time) / lifeCycle);
            }
        }
        public TimerTask Clone()
        {
            var timerTask = new TimerTask();
            timerTask.ID = ID;
            timerTask.expirationTime = expirationTime;
            timerTask.lifeCycle = lifeCycle;
            timerTask.times = times;
            timerTask.func = func;
            return timerTask;
        }
        public void Destory()
        {
            freeTaskCls.Enqueue(this);
        }
        public void Reset()
        {
            expirationTime = Time.time + lifeCycle;
        }
    }


    #region Member property

    protected static List<TimerTask> activeTaskCls = new List<TimerTask>();//激活中的TimerTask对象
    protected static Queue<TimerTask> freeTaskCls = new Queue<TimerTask>();//闲置TimerTask对象
    protected static HashSet<UnityAction> lateChannel = new HashSet<UnityAction>();//确保callLate调用的唯一性
    protected static ulong idOffset = 1000; //timer的唯一标识
    protected static bool inited = false; //避免重复创建 防呆设计
    #endregion


    #region public methods

    //每帧结束时执行回调 : 当前帧内的多次调用仅在当前帧结束的时候执行一次
    public static void CallerLate(UnityAction func)
    {
        if (!lateChannel.Contains(func))
        {
            lateChannel.Add(func);
            SetTimeout(0f, func);
        }
    }


    //delay秒后 执行一次回调
    public static ulong SetTimeout(float delay, UnityAction func)
    {
        return SetInterval(delay, func, false, 1);
    }

    /// <summary>
    /// 周期性定时器 间隔一段时间调用一次
    /// </summary>
    /// <param name="interval"> 间隔时长: 秒</param>
    /// <param name="func"> 调用的方法回调 </param>
    /// <param name="immediate"> 是否立即执行一次 </param>
    /// <param name="times"> 调用的次数: 默认永久循环 当值<=0时会一直更新调用 当值>0时 循环指定次数后 停止调用 </param>
    /// <returns></returns>
    public static ulong SetInterval(float interval, UnityAction func, bool immediate = false, int times = 0)
    {
        //从free池中 获取一个闲置的TimerTask对象
        var timer = GetFreeTimerTask();
        timer.lifeCycle = interval;
        timer.Reset();
        timer.func = func;
        timer.times = times;
        timer.ID = ++idOffset;

        //尝试初始化
        Init();

        //立即执行一次
        if (immediate)
        {
            --timer.times;
            func?.Invoke();
            if (timer.times == 0)
            {

                timer.Destory();
            }
            else
            {
                //添加到激活池中
                activeTaskCls.Add(timer);
            }
        }
        else
        {
            //添加到激活池中
            activeTaskCls.Add(timer);
        }

        return idOffset;
    }

    #endregion


    #region Get Timer methods

    /// <summary>
    /// 通过Tag获取定时器对象
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public static TimerTask GetTimer(ulong ID)
    {
        return activeTaskCls.Find((TimerTask t) =>
         {
             return t.ID == ID;
         })?.Clone();
    }

    /// <summary>
    /// 通过Tag获取定时器对象
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static TimerTask GetTimer(UnityAction func)
    {
        return activeTaskCls.Find((TimerTask t) =>
         {
             return t.func == func;
         })?.Clone();
    }
    #endregion


    #region Clean Timer methods

    /// <summary>
    /// 通过ID 清理定时器
    /// </summary>
    /// <param name="ID">定时器标签</param>
    /// <returns></returns>
    public static void ClearTimer(ulong ID)
    {
        int index = activeTaskCls.FindIndex((TimerTask t) =>
         {
             return t.ID == ID;
         });

        if (index != -1)
        {
            var t = activeTaskCls[index];
            if (lateChannel.Count != 0 && lateChannel.Contains(t.func))
            {
                lateChannel.Remove(t.func);
            }
            activeTaskCls.RemoveAt(index);
            freeTaskCls.Enqueue(t);
        }
    }

    /// <summary>
    /// 通过方法 清理定时器
    /// </summary>
    /// <param name="func">处理方法</param>
    /// <returns></returns>
    public static void ClearTimer(UnityAction func)
    {
        var allMatchTask = activeTaskCls.FindAll(t => t.func == func);

        allMatchTask?.ForEach(task =>
        {
            if (lateChannel.Count != 0 && lateChannel.Contains(task.func))
            {
                lateChannel.Remove(task.func);
            }
            activeTaskCls.Remove(task);
            freeTaskCls.Enqueue(task);
        });
    }

    /// <summary>
    /// 移除 类实例target的所有成员方法定时器
    /// </summary>
    /// <param name="func">处理方法</param>
    /// <returns></returns>
    public static void ClearTimer(object target)
    {
        //var allMatchTask = m_activeTaskCls.FindAll( t => t.func.Target == target );
        var clsName = target.GetType().FullName;
        var allMatchTask = activeTaskCls.FindAll(t =>
        {
            if (null != t.func && null != t.func.Target)
            {
                var fullname = t.func.Target.GetType().FullName;
                var currentClsNameClip = fullname.Split('+');
                if (currentClsNameClip.Length > 0)
                {
                    if (currentClsNameClip[0] == clsName)
                    {
                        return true;
                    }
                }
            }
            return false;
        });

        allMatchTask?.ForEach(task =>
        {
            if (lateChannel.Count != 0 && lateChannel.Contains(task.func))
            {
                lateChannel.Remove(task.func);
            }
            activeTaskCls.Remove(task);
            freeTaskCls.Enqueue(task);
        });
    }

    /// <summary>
    /// 清理所有定时器
    /// </summary>
    public static void ClearTimers()
    {
        lateChannel.Clear();
        activeTaskCls.ForEach(timer => freeTaskCls.Enqueue(timer));
        activeTaskCls.Clear();
    }

    #endregion


    #region System methods

    //Update更新之前
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(TimeElapse());
    }

    //定时器调度
    private IEnumerator TimeElapse()
    {
        TimerTask t = null;
        while (true)
        {
            if (activeTaskCls.Count > 0)
            {
                float time = Time.time;
                for (int i = 0; i < activeTaskCls.Count; ++i)
                {
                    t = activeTaskCls[i];

                    if (t.expirationTime <= time)
                    {
                        if (t.times == 1)
                        {
                            activeTaskCls.RemoveAt(i--);
                            t.Destory();

                            if (lateChannel.Count != 0 && lateChannel.Contains(t.func))
                            {
                                lateChannel.Remove(t.func);
                            }
                        }

                        t.Reset();
                        --t.times;
                        t.func();
                    }
                }
            }
            yield return 0;
        }
    }

    //初始化
    protected static void Init()
    {
        if (!inited)
        {
            inited = true;
            var inst = new GameObject("TimerNode");
            inst.AddComponent<Timer>();
        }
    }

    //获取闲置定时器
    protected static TimerTask GetFreeTimerTask()
    {
        if (freeTaskCls.Count > 0)
        {
            return freeTaskCls.Dequeue();
        }
        return new TimerTask();
    }

    #endregion

}



