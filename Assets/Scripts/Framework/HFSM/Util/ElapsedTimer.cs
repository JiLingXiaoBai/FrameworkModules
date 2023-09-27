using UnityEngine;

namespace JLXB.Framework.FSM
{
	/// <summary>
	/// Default timer that calculates the elapsed time based on Time.time.
	/// </summary>
	public class ElapsedTimer
	{
		public float startTime;
		public float Elapsed => Time.time - startTime;

		public ElapsedTimer()
		{
			startTime = Time.time;
		}

		public void Reset()
		{
			startTime = Time.time;
		}

		public static bool operator >(ElapsedTimer elapsedTimer, float duration)
			=> elapsedTimer.Elapsed > duration;

		public static bool operator <(ElapsedTimer elapsedTimer, float duration)
			=> elapsedTimer.Elapsed < duration;

		public static bool operator >=(ElapsedTimer elapsedTimer, float duration)
			=> elapsedTimer.Elapsed >= duration;

		public static bool operator <=(ElapsedTimer elapsedTimer, float duration)
			=> elapsedTimer.Elapsed <= duration;
	}
}
