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
		[SerializeField, Range(10.0f, 90.0f)] private float angleValue = 60.0f;
		[SerializeField, Range(3, 10)] private int fieldOfViewAccuracy = 5;
		[SerializeField, Range(1.0f, 25.0f)] private float range = 10.0f; 
		private Vector2[] fieldOfView = new Vector2[] { };

		private IPlayerBehaviour target = null;
		public IPlayerBehaviour Target => target; 
        #endregion
		
		#region Methods
		private void  GenerateFOV()
		{
			fieldOfView = new Vector2[fieldOfViewAccuracy];
			float _currentAngle = -angleValue / 2;
			float _angleInterval = angleValue / (fieldOfViewAccuracy - 1);
			for (int i = 0; i < fieldOfView.Length; i++)
			{
				fieldOfView[i] = new Vector3(Mathf.Sin(_currentAngle * Mathf.Deg2Rad), Mathf.Cos(_currentAngle * Mathf.Deg2Rad)) * range;
				_currentAngle += _angleInterval; 
			}
		}

		public bool CastDetection()
		{
			RaycastHit2D _hit; 
			for (int i = 0; i < fieldOfView.Length; i++)
			{
				_hit = Physics2D.Raycast(transform.position, transform.rotation * fieldOfView[i]);
				if (_hit.collider == null)
					continue;
				if (_hit.collider.TryGetComponent<IPlayerBehaviour>(out target))
					continue; 
				// ELSE STORE THE POINT HERE TO BUILD THE POLYGON
			}
			return target != null; 
		}

		private void Start()
		{
			GenerateFOV();
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			for (int i = 0; i < fieldOfView.Length; i++)
			{
				Gizmos.DrawRay(transform.position, transform.rotation * fieldOfView[i]);
			}
		}
		#endregion
	}
}
