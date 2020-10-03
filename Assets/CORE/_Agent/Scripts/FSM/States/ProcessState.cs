// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare47
{
	[CreateAssetMenu(fileName = "Process State", menuName = "AI/States/Process State", order = 50)]
	public class ProcessState : BaseState
    {
		#region Fields / Properties
		//[HorizontalLine(1, order = 0), Section("ProcessState", order = 1)]

		#endregion

		#region Constructor
		public ProcessState() : base() => StateType = StateType.Process;
		#endregion

		#region Methods
		private void Process()
		{
			if (controller.HasToMove)
				stateMachine.GoToState(this, StateType.Move);
			else
				stateMachine.GoToState(this, StateType.Idle);
		}

		public override void OnEnterState(FiniteStateMachine _stateMachine)
		{
			base.OnEnterState(_stateMachine);
			Process();
		}

		public override void OnExitState()
		{
		}
		#endregion
	}
}
