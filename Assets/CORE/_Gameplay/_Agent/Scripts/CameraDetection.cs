// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;
using UnityEngine.Events; 

namespace LudumDare47
{
	public class CameraDetection : EnemyDetection, ILateUpdate
	{
		#region Fields / Properties
		[HorizontalLine(1, order = 0), Section("CameraDetection", order = 1)]
		[SerializeField] private UnityEvent onPlayerDetected = new UnityEvent();
		[SerializeField] private EnemyDetection linkedEnemy = null; 
		#endregion

		#region Methods
		protected override void GenerateFOV()
		{
			base.GenerateFOV();
			UpdateManager.Instance.Register(this);
		}

		private IPlayerBehaviour _tempTarget;
		public override bool CastDetection()
		{
			targetFound = false;
			RaycastHit2D _hit;
			for (int i = 0; i < fieldOfView.Length; i++)
			{
				_hit = Physics2D.Raycast(transform.position, transform.rotation * fieldOfView[i]);
				if (_hit.collider == null)
					continue;
				if (_hit.collider.TryGetComponent<IPlayerBehaviour>(out _tempTarget))
				{
					if(target != _tempTarget)
					{
						targetFound = true;
						target = _tempTarget;
						TargetTransform = _hit.collider.transform;
						break;
					}					
				}
			}
			if (!targetFound)
			{
				target = null;
				TargetTransform = null;
			}
			return targetFound;
		}

		void ILateUpdate.Update()
		{
			if (CastDetection())
			{
				onPlayerDetected.Invoke();
				linkedEnemy.SetTarget(target, TargetTransform);
			}
		}
		#endregion


		private void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(transform.position, linkedEnemy.transform.position);
		}
	}
}
