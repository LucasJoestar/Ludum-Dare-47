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
		#endregion

		#region Constructor
		public TutoCatchState() : base() => StateType = StateType.Catch; 
        #endregion

        #region Methods
        public override void OnEnterState(FiniteStateMachine _stateMachine)
		{
			base.OnEnterState(_stateMachine);
			controller.Detection.Target.Die(); 
			controller.Detection.Target.Parent(controller.transform);
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
				return; 
			}
			stateMachine.GoToState(this, StateType.Idle); 
		}
		#endregion

	}
}
