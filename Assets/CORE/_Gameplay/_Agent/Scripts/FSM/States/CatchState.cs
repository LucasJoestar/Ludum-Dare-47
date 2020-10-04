// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare47
{
	[CreateAssetMenu(fileName = "Catch State", menuName = "AI/States/Catch State", order = 50)]
	public class CatchState : BaseState, ILateUpdate
    {
		#region Fields / Properties
		//[HorizontalLine(1, order = 0), Section("CatchState", order = 1)]

		#endregion

		#region Constructor
		public CatchState() : base() => StateType = StateType.Catch;
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
			if(controller.IsInAnimation)
			{
				return; 
			}
			controller.ReturnToOriginalPosition(); 
			stateMachine.GoToState(this, StateType.Process);
		}

		#endregion
	}
}
