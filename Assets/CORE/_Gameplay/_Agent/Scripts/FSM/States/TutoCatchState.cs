// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare47
{
	public class TutoCatchState : BaseState, ILateUpdate
	{
		#region Fields / Properties
		[HorizontalLine(1, order = 0), Section("TutoCatchState", order = 1)]
		[SerializeField] private Vector2 tutoDestination = Vector2.zero; 
		#endregion

		#region Methods
		public override void OnEnterState(FiniteStateMachine _stateMachine)
		{
			base.OnEnterState(_stateMachine);
			controller.NavAgent.SetDestination(tutoDestination); 
			UpdateManager.Instance.Register(this); 
		}

		public override void OnExitState()
		{
			UpdateManager.Instance.Unregister(this); 
		}

		public void Update()
		{
			if(controller.NavAgent.IsMoving)
			{
				controller.Detection.CastDetection();
				return; 
			}
			stateMachine.GoToState(this, StateType.Idle); 
		}
		#endregion

	}
}
