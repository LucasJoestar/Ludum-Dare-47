// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare47
{
	public class CameraRoomTrigger : Trigger
    {
		#region Fields / Properties
		[HorizontalLine(1, order = 0), Section("CameraRoomTrigger", order = 1)]
		[SerializeField] private CameraDetection linkedCamera = null;
		#endregion

		#region Methods
		public override void OnEnter(GameObject _gameObject)
		{
			if(_gameObject.TryGetComponent<EnemyDetection>(out EnemyDetection _newDetection))
			{
				linkedCamera.LinkEnemy(_newDetection);
			}
		}

		public override void OnExit(GameObject _gameObject)
		{
			base.OnExit(_gameObject);
			if (_gameObject.TryGetComponent<EnemyDetection>(out EnemyDetection _newDetection))
			{
				linkedCamera.BreakLink(_newDetection); 
			}
		}
		#endregion
	}
}
