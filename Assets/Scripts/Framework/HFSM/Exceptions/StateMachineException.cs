using System;

namespace JLXB.Framework.FSM.Exceptions
{
	[Serializable]
	public class StateMachineException : Exception
	{
		public StateMachineException(string message) : base(message) { }
	}
}
