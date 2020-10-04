// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare47
{
	public class DoorOpener : Trigger, IInteractable
    {
		#region Fields / Properties
		[HorizontalLine(1, order = 0), Section("DoorOpener", order = 1)]
		[SerializeField, ReadOnly] private Door linkedDoor = null;
		public bool IsActivated { get; private set; } = false;
		[SerializeField] private bool isTrigger = false; 
		#endregion

		#region Methods
		public bool LinkToDoor(Door _door) => linkedDoor = _door; 
		public bool Interact(IPlayerBehaviour _player)
		{
			if (isTrigger) return false; 
			IsActivated = true;
			linkedDoor.UpdateOpenningStatus(); 
			return true; 
		}

		public override void OnEnter(GameObject _gameObject)
		{
			if (!isTrigger) return;
			IsActivated = true;
			linkedDoor.UpdateOpenningStatus();
		}
		public override void OnExit(GameObject _gameObject)
		{
			if (!isTrigger) return;
			IsActivated = false;
			linkedDoor.UpdateOpenningStatus();
		}
		#endregion
	}
}
