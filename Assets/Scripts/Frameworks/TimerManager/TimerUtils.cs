using UnityEngine.Events;
using UnityEngine;

namespace JLXB.Framework.Timer
{
    public class TimerUtils
    {
        /// <summary>
        /// 执行一次的定时器
        /// </summary>
        /// <param name="delay">延迟时间</param>
        /// <param name="onComplete">延迟时间后调用的回调</param>
        /// <param name="onFrameUpdate">每帧调用的回调</param>
        /// <param name="useRealTime">是否使用真实时间,即不受 TimeScale 和 pause 等影响</param>
        /// <param name="autoDestroyOwner">此组件销毁后,自行取消此timer</param>
        /// <returns></returns>
        public static Timer TimerOnce(float delay, UnityAction onComplete, UnityAction<float> onFrameUpdate = null, bool useRealTime = false, MonoBehaviour autoDestroyOwner = null)
        {
            return Timer.Register(delay, onComplete, onFrameUpdate, false, useRealTime, autoDestroyOwner);
        }

        /// <summary>
        /// 循环执行的定时器
        /// </summary>
        /// <param name="duration">执行间隔</param>
        /// <param name="onStepUpdate">每次执行调用的回调</param>
        /// <param name="loopCount">循环次数,非正整数表示永远执行</param>
        /// <param name="onComplete">所有循环次数执行后,最后调用一次的回调,若 loopCount 为非正整数则永远不会调用</param>
        /// <param name="onFrameUpdate">每帧调用的回调</param>
        /// <param name="useRealTime">是否使用真实时间,即不受 TimeScale 和 pause 等影响</param>
        /// <param name="autoDestroyOwner">此组件销毁后,自行取消此timer</param>
        /// <returns></returns>
        public static Timer TimerLoop(float duration, UnityAction onStepUpdate, int loopCount = 0, UnityAction onComplete = null,
        UnityAction<float> onFrameUpdate = null, bool useRealTime = false, MonoBehaviour autoDestroyOwner = null)
        {
            Timer timer = null;
            UnityAction action = null;

            if (loopCount <= 0)
            {
                action = onStepUpdate;
            }
            else
            {
                action = () =>
                {
                    onStepUpdate.Invoke();
                    if (--loopCount <= 0)
                    {
                        timer.Cancel();
                        onComplete?.Invoke();
                    }
                };
            }

            timer = Timer.Register(
                    duration: duration,
                    onComplete: action,
                    onFrameUpdate: onFrameUpdate,
                    isLooped: true,
                    useRealTime: useRealTime,
                    autoDestroyOwner: autoDestroyOwner
                );

            return timer;
        }

    }
}
