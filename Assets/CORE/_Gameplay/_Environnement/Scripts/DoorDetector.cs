// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare47
{
	public class DoorDetector : Trigger
    {
		#region Fields / Properties
		[HorizontalLine(1, order = 0), Section("EnemyDoorDetector", order = 1)]
		[SerializeField] private Door linkedDoor = null; 
		#endregion

		#region Methods
		public override void OnEnter(GameObject _gameObject)
		{
			linkedDoor.ForceOpenning();
		}
		#endregion
	}
}
