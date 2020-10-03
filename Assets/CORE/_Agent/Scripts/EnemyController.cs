// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare47
{
	public class EnemyController : MonoBehaviour
    {
		#region Fields / Properties
		[HorizontalLine(1, order = 0), Section("EnemyController", order = 1)]
		[SerializeField] private FiniteStateMachine stateMachine;
		[SerializeField, Required] private EnemyDetection detection = null; 

		[HorizontalLine(1, order = 0)]
		[SerializeField, ReadOnly] private Vector2 destination = Vector2.zero;
		[SerializeField] private Vector2[] patrolPath = new Vector2[] { };

		public EnemyDetection Detection => detection; 
		public bool HasToMove 
		{
			get
			{
				return destination != Vector2.zero || patrolPath.Length > 0; 
			}
		}
        #endregion
		
		#region Methods
		private void StartBehaviour()
		{
			stateMachine = stateMachine.Copy();
			stateMachine.StartFSM(this); 
		}
		#endregion
    }
}
