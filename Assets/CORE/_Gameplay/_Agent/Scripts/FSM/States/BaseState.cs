// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare47
{
	public abstract class BaseState : ScriptableObject
    {
		#region Fields / Properties
		protected FiniteStateMachine stateMachine = null; 
		protected EnemyController controller = null;
		private bool isInitialized = false;

		public StateType StateType { get; protected set; }
		 
		#endregion

		#region Methods
		public virtual void OnEnterState(FiniteStateMachine _stateMachine)
		{
			if(!isInitialized)
			{
				stateMachine = _stateMachine;
				controller = _stateMachine.Controller; 
			}
		}

		public abstract void OnExitState();
		#endregion
	}

	public enum StateType
	{
		Process,
		Idle,
		Move,
		Catch, 
	}
}
