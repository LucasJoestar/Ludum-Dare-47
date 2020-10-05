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
		[HorizontalLine(1, order = 0), Section("CatchState", order = 1)]
		[SerializeField, Range(1.0f, 2.0f)] private float CatchCooldown = 1.0f; 
		#endregion

		#region Constructor
		public CatchState() : base() => StateType = StateType.Catch;
		#endregion

		#region Methods
		public override void OnEnterState(FiniteStateMachine _stateMachine)
		{
			base.OnEnterState(_stateMachine);
			timer = 0; 
			controller.SetCatchAnimation();
			controller.Detection.Target.Die(); 
			UpdateManager.Instance.Register(this);
		}

		public override void OnExitState()
		{
			UpdateManager.Instance.Unregister(this);
			controller.Detection.SetTarget(null, null);
			controller.ReturnToOriginalPosition();
		}

		// ------------------------------ // 

		float timer = 0; 
		void ILateUpdate.Update()
		{
			if (!stateMachine.IsActive)
			{
				stateMachine.GoToState(this, StateType.Process);
				return;
			}
			if (controller.IsInAnimation)
			{
				return; 
			}
			if(timer < CatchCooldown)
			{
				timer += Time.deltaTime;
				return; 
			}
			stateMachine.GoToState(this, StateType.Process);
		}

		#endregion
	}
}
