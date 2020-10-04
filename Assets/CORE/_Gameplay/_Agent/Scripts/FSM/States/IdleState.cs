// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare47
{
	[CreateAssetMenu(fileName = "Idle State", menuName = "AI/States/Idle State", order = 50)]
	public class IdleState : BaseState, ILateUpdate
    {
		#region Fields / Properties
		//[HorizontalLine(1, order = 0), Section("IdleState", order = 1)]

		#endregion

		#region Constructor
		public IdleState() : base() => StateType = StateType.Idle;
		#endregion

		#region Methods
		public override void OnEnterState(FiniteStateMachine _stateMachine)
		{
			base.OnEnterState(_stateMachine);
			UpdateManager.Instance.Register(this);
		}

		public override void OnExitState()
		{
			UpdateManager.Instance.Unregister(this);
		}

		// ------------------------------ // 

		void ILateUpdate.Update()
		{
			if(controller.HasToMove || controller.Detection.CastDetection())
			{
				stateMachine.GoToState(this, StateType.Process);
			}
		}

		#endregion
	}
}
