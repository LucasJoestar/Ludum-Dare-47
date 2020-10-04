// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare47
{
	public class DoorOpener : MonoBehaviour, IInteractable
    {
		#region Fields / Properties
		[HorizontalLine(1, order = 0), Section("DoorOpener", order = 1)]
		[SerializeField, ReadOnly] private Door linkedDoor = null;
		public bool IsActivated { get; private set; } = false;
		#endregion

		#region Methods
		public bool LinkToDoor(Door _door) => linkedDoor = _door; 
		public bool Interact(IPlayerBehaviour _player)
		{
			IsActivated = true;
			linkedDoor.UpdateOpenningStatus(); 
			return true; 
		}
		#endregion
	}
}
