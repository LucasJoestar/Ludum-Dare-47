// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare47
{
	public class EnemyDetection : MonoBehaviour
    {
		#region Fields / Properties
		[HorizontalLine(1, order = 0), Section("EnemyDetection", order = 1)]
		[SerializeField, Range(10.0f, 90.0f)] protected float angleValue = 60.0f;
		[SerializeField, Range(3, 50)] protected int fieldOfViewAccuracy = 5;
		[SerializeField, Range(1.0f, 25.0f)] protected float range = 10.0f;		
		protected Vector2[] fieldOfView = new Vector2[] { };

		protected IPlayerBehaviour target = null;
		public IPlayerBehaviour Target => target;
		public Transform TargetTransform { get; protected set; }
		#endregion

		#region Methods
		protected virtual void GenerateFOV()
		{
			fieldOfView = new Vector2[fieldOfViewAccuracy];
			float _currentAngle = -angleValue / 2;
			float _angleInterval = angleValue / (fieldOfViewAccuracy - 1);
			for (int i = 0; i < fieldOfView.Length; i++)
			{
				fieldOfView[i] = new Vector3(Mathf.Sin(_currentAngle * Mathf.Deg2Rad), Mathf.Cos(_currentAngle * Mathf.Deg2Rad)).normalized;
				_currentAngle += _angleInterval; 
			}
		}

		protected bool targetFound = false;
		public virtual bool CastDetection()
		{
			targetFound = false; 
			RaycastHit2D _hit; 
			for (int i = 0; i < fieldOfView.Length; i++)
			{
				_hit = Physics2D.Raycast(transform.position, transform.rotation * fieldOfView[i]);
				if (_hit.collider == null)
					continue;
				if (!targetFound && _hit.collider.TryGetComponent<IPlayerBehaviour>(out target))
				{
					targetFound = true; 
					TargetTransform = _hit.collider.transform;
					continue;
				}				
			}
			if (!targetFound)
			{
				target = null;
				TargetTransform = null; 
			}
			return targetFound; 
		}

		public void SetTarget(IPlayerBehaviour _target, Transform _targetTransform)
		{
			if (target != null) return;
			target = _target;
			TargetTransform = _targetTransform;
		}

		protected virtual void Start()
		{
			GenerateFOV();
		}

		private void OnDrawGizmos()
		{
			for (int i = 0; i < fieldOfView.Length; i++)
			{
				if (i == 0) Gizmos.color = Color.cyan;
				else if (i == fieldOfView.Length - 1) Gizmos.color = Color.green;
				else Gizmos.color = Color.red;

				Gizmos.DrawRay(transform.position, transform.rotation * fieldOfView[i] * range);
			}

		}
		#endregion
	}
}
