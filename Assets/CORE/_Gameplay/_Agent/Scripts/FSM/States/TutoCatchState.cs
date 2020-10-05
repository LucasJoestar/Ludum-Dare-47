// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare47
{
	[CreateAssetMenu(fileName = "Catch State Tuto", menuName = "AI/States/Catch State Tuto", order = 50)]
	public class TutoCatchState : BaseState, ILateUpdate
	{
		#region Fields / Properties
		[HorizontalLine(1, order = 0), Section("TutoCatchState", order = 1)]
		[SerializeField] private Vector2 tutoDestination = Vector2.zero;
		private IPlayerBehaviour caughtTarget = null; 
		#endregion

		#region Constructor
		public TutoCatchState() : base() => StateType = StateType.Catch; 
        #endregion

        #region Methods
        public override void OnEnterState(FiniteStateMachine _stateMachine)
		{
			base.OnEnterState(_stateMachine);
			if(controller.Detection.Target != null)
			{
				caughtTarget = controller.Detection.Target;
				caughtTarget.Die();
				caughtTarget.Parent(controller.GrabTransform);
			}
			controller.NavAgent.SetDestination(tutoDestination); 
			UpdateManager.Instance.Register(this); 
		}

		public override void OnExitState()
		{
			UpdateManager.Instance.Unregister(this);
			caughtTarget.Unparent();
			if(controller.Detection.Target == caughtTarget)
			{
				controller.Detection.SetTarget(null, null);
				controller.ResetDestination();
			}
		}

		public void Update()
		{
			if (!stateMachine.IsActive)
			{
				stateMachine.GoToState(this, StateType.Process);
				return;
			}
			if (controller.NavAgent.IsMoving)
			{
				if(controller.Detection.CastDetection())
				{
					stateMachine.GoToState(this, StateType.Process);
				}
				return; 
			}
			stateMachine.GoToState(this, StateType.Idle); 
		}
		#endregion

	}
}
