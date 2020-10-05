// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic; 

namespace LudumDare47
{
	public class CameraDetection : EnemyDetection, ILateUpdate
	{
		#region Fields / Properties
		[HorizontalLine(1, order = 0), Section("CameraDetection", order = 1)]
		[SerializeField] private UnityEvent onPlayerDetected = new UnityEvent();
		[SerializeField] private UnityEvent onLevelReseted = new UnityEvent();
		[SerializeField] private EnemyDetection linkedEnemy = null;

		private List<int> detectedIDs = new List<int>(); 
		#endregion

		#region Methods
		protected override void GenerateFOV()
		{
			base.GenerateFOV();
			UpdateManager.Instance.Register(this);
		}

		public void LinkEnemy(EnemyDetection _linkedDetection)
		{
			if (linkedEnemy != null) return;
			linkedEnemy = _linkedDetection; 
		}

		public void BreakLink(EnemyDetection _unlinkedDetection)
		{
			if (linkedEnemy != _unlinkedDetection) return;
			linkedEnemy = null; 
		}

		void ILateUpdate.Update()
		{
			if (CastDetection())
			{
				onPlayerDetected.Invoke();
				if(linkedEnemy) linkedEnemy.GetComponent<EnemyController>().SetDestination(TargetTransform.position);
			}
		}

		private IPlayerBehaviour _tempTarget = null; 
		public override bool CastDetection()
		{
			RaycastHit2D _hit;
			for (int i = 0; i < fieldOfView.Length; i++)
			{
				_hit = Physics2D.Raycast(transform.position, transform.rotation * fieldOfView[i], range, detectionMask.value);
				if (_hit.collider == null)
					continue;
				if (_hit.collider.TryGetComponent<IPlayerBehaviour>(out _tempTarget))
				{
					if (detectedIDs.Contains(_hit.transform.GetInstanceID()))
						continue;
					target = _tempTarget; 
					TargetTransform = _hit.collider.transform;
					detectedIDs.Add(TargetTransform.GetInstanceID());
					return true;
				}
			}
			return false;
		}

		protected override void ResetDetectionBehaviour()
		{
			base.ResetDetectionBehaviour();
			onLevelReseted.Invoke();
			detectedIDs.Clear(); 
		}
		#endregion

		protected override void OnDrawGizmos()
		{
			base.OnDrawGizmos();
			if (!linkedEnemy) return;  
			Gizmos.color = Color.red;
			Gizmos.DrawLine(transform.position, linkedEnemy.transform.position);
		}
	}
}
