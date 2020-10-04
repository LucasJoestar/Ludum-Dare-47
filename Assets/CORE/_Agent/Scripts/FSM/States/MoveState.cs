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
		//[HorizontalLine(1, order = 0), Section("Move Settings", order = 1)]
		private int patrolIndex = 0;
		private bool isInPatrol = false; 
		#endregion

		#region Constructor
		public MoveState() : base() => StateType = StateType.Move;


		#endregion

		#region Methods

		private void ReachNextPatrolPoint()
		{
			controller.NavAgent.SetDestination(controller.PatrolPath[patrolIndex]);
			patrolIndex++;
			patrolIndex = patrolIndex >= controller.PatrolPath.Length ? 0 : patrolIndex;
		}

		// ------------------------------ // 

		public override void OnEnterState(FiniteStateMachine _stateMachine)
		{
			base.OnEnterState(_stateMachine);
			patrolIndex = 0;
			isInPatrol = false; 
			// if target!=null --> Chase the target (set the chase speed)
			if(controller.Detection.Target != null)
			{
				//controller.NavAgent.SetDestination(controller.Detection.Target);
				Debug.Log("Implement Method to get Interface position");
			}
			// else if destination != Vector2.zero --> Go to the destination (set the chase speed)
			else if(controller.Destination != Vector2.zero)
			{
				controller.NavAgent.SetDestination(controller.Destination); 
			}
			else if(controller.PatrolPath.Length > 0)
			{
				isInPatrol = true;
				ReachNextPatrolPoint();
			}
			else
			{
				stateMachine.GoToState(this, StateType.Process);
				return; 
			}
			controller.NavAgent.SetSpeedValue(isInPatrol);
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
			if(controller.NavAgent.IsMoving)
			{
				return; 
			}
			if(isInPatrol)
			{
				ReachNextPatrolPoint(); 
			}
			else
			{
				stateMachine.GoToState(this, StateType.Process);
			}
			// make the agent move here
		}
		#endregion
	}
}
