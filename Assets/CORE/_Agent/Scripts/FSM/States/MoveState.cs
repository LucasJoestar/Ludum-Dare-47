// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare47
{
	[CreateAssetMenu(fileName = "Move State", menuName = "AI/States/Move State", order = 50)]
	public class MoveState : BaseState, ILateUpdate
    {
		#region Fields / Properties
		//[HorizontalLine(1, order = 0), Section("MoveToState", order = 1)]
		private int patrolIndex = 0; 
		#endregion

		#region Constructor
		public MoveState() : base() => StateType = StateType.Move;


		#endregion

		#region Methods
		public override void OnEnterState(FiniteStateMachine _stateMachine)
		{
			base.OnEnterState(_stateMachine);
			patrolIndex = 0; 
			// if target!=null --> Chase the target (set the chase speed)
			// else if destination != Vector2.zero --> Go to the destination (set the chase speed)
			// else if patrolPath.length > 0 --> go through the patrol path
			UpdateManager.Instance.Register(this);
		}

		public override void OnExitState()
		{
			UpdateManager.Instance.Unregister(this);
		}

		// ------------------------------ // 

		void ILateUpdate.Update()
		{
			if(controller.Detection.CastDetection())
			{
				// stop the agent here
				stateMachine.GoToState(this, StateType.Process);
				return; 
			}
			// make the agent move here
		}
		#endregion
	}
}
