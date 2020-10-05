// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare47
{
	public class EnemyDetection : MonoBehaviour, IResetable 
    {
		#region Fields / Properties
		[HorizontalLine(1, order = 0), Section("EnemyDetection", order = 1)]
		[SerializeField, Range(10.0f, 90.0f)] protected float angleValue = 60.0f;
		[SerializeField, Range(3, 50)] protected int fieldOfViewAccuracy = 5;
		[SerializeField, Range(1.0f, 25.0f)] protected float range = 10.0f;
		[SerializeField] protected LayerMask detectionMask = new LayerMask(); 

		protected Vector2[] fieldOfView = new Vector2[] { };
		public Transform TargetTransform { get; protected set; }

		protected IPlayerBehaviour target;
		public IPlayerBehaviour Target => target;
        #endregion

        #region Methods
        private readonly static RaycastHit2D[] detectionCast = new RaycastHit2D[6];

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

		public virtual bool CastDetection()
		{
            PlayerGhost _ghost = null;
            for (int i = 0; i < fieldOfView.Length; i++)
			{
                int _amount = Physics2D.RaycastNonAlloc(transform.position, transform.rotation * fieldOfView[i], detectionCast, range, detectionMask.value);

                for (int _j = 0; _j < _amount; _j++)
                {
                    if (detectionCast[_j].collider.TryGetComponent(out PlayerController _player))
                    {
                        target = _player;
                        TargetTransform = detectionCast[_j].collider.transform;
                        return true;
                    }
                    else if (detectionCast[_j].collider.TryGetComponent(out PlayerGhost _testGhost))
                    {
                        // IS OK
                        _ghost = _testGhost;
                    }
                    else
                        break;
                }
            }

            if (_ghost)
            {
                target = _ghost;
                TargetTransform = _ghost.transform;
                return true;
            }

			return false; 
		}

		public void SetTarget(IPlayerBehaviour _target, Transform _targetTransform)
		{
			target = _target; 
			TargetTransform = _targetTransform;
		}

		public void ResetBehaviour() => ResetDetectionBehaviour();

		protected virtual void ResetDetectionBehaviour()
		{
			SetTarget(null, null); 
		}

		protected virtual void Start()
		{
			LevelManager.Instance.RegisterResetable(this);
			GenerateFOV();
		}

        private bool isInitiaized = false;

		protected virtual void OnDrawGizmos()
		{
            if (!isInitiaized)
            {
                isInitiaized = true;
                GenerateFOV();
            }

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
