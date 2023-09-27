
namespace JLXB.Framework.FSM
{
	public interface ITransitionListener
	{
		void BeforeTransition();
		void AfterTransition();
	}
}
