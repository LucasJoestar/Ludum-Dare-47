// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;
using LudumDare47.Navigation;

namespace LudumDare47
{
	public class EnemyController : MonoBehaviour, IResetable
    {
		public static readonly int Moving_Anim = Animator.StringToHash("IsMoving");
		public static readonly int Catch_Anim = Animator.StringToHash("Catch");

		#region Fields / Properties
		[HorizontalLine(1, order = 0), Section("EnemyController", order = 1)]
		[SerializeField] private FiniteStateMachine stateMachine;
		[SerializeField, Required] private EnemyDetection detection = null;
		[SerializeField, Required] private NavigationAgent navAgent = null;
		[SerializeField, Required] private Animator animator = null;

		[HorizontalLine(1, order = 0)]
		[SerializeField] private Vector2 initialDestination = Vector2.zero;
		[SerializeField] private Vector2 destination = Vector2.zero;
		[SerializeField] private Vector2[] patrolPath = new Vector2[] { };

		[HorizontalLine(1, order = 0)]
		[SerializeField, Range(.1f, 10.0f)] private float interactionRange = 3.0f;
		[SerializeField] private Transform grabTransfom = null;
		public Transform GrabTransform => grabTransfom; 

		public EnemyDetection Detection => detection;
		public NavigationAgent NavAgent => navAgent;
		public Vector2 Destination => destination; 
		public Vector2[] PatrolPath => patrolPath;

		public float InteractionRange => interactionRange;

		public bool HasToChase 
		{
			get
			{
				return detection.TargetTransform != null || destination != Vector2.zero; 
			}
		}
		 public bool HasToMove { get { return HasToChase || patrolPath.Length > 0;  } }
		public bool IsInAnimation { get; set; }
		#endregion

		#region Methods
		// ----------------------------------------------------

		public void SetMovementAnimation(bool _isMoving) => animator.SetBool(Moving_Anim, _isMoving);

		public void SetCatchAnimation()
		{
			IsInAnimation = true; 
			animator.SetTrigger(Catch_Anim);
		}

		// ----------------------------------------------------

		private void StartBehaviour()
		{
			stateMachine = stateMachine.Copy();
			stateMachine.StartFSM(this); 
		}

		// ----------------------------------------------------

		public void ReturnToOriginalPosition() => destination = NavAgent.InitialPosition; 

		public void ResetDestination() => destination = Vector2.zero;

		public void SetDestination(Vector2 _destination) => destination = _destination;

		// ----------------------------------------------------

		public void Die()
		{
			navAgent.StopAgent();
			stateMachine.StopFSM();
			gameObject.SetActive(false); 
		}

		// ----------------------------------------------------

		private void Start()
		{
			LevelManager.Instance.RegisterResetable(this);
			LevelManager.Instance.RegisterAnimator(animator); 
			StartBehaviour(); 
		}

		private void OnDestroy()
		{
			stateMachine.StopFSM();
		}
		private void OnDrawGizmos()
		{
			if(destination != Vector2.zero)
			{
				Gizmos.color = Color.yellow;
				Gizmos.DrawSphere(destination, .5f);
				Gizmos.DrawLine(transform.position, destination);
			}
			Gizmos.color = Color.magenta;
			for (int i = 0; i < patrolPath.Length; i++)
			{
				Gizmos.DrawSphere(patrolPath[i], .5f);
				if (i == patrolPath.Length - 1)
					Gizmos.DrawLine(patrolPath[i], patrolPath[0]);
				else Gizmos.DrawLine(patrolPath[i], patrolPath[i + 1]); 
			}
		}

		public void ResetBehaviour()
		{
			gameObject.SetActive(true); 
			navAgent.StopAgent();
			destination = initialDestination;
			NavAgent.ResetAgent(); 
			animator.SetBool(Moving_Anim, false); 
			stateMachine.ResetFSM(); 
		}
		#endregion
	}
}
