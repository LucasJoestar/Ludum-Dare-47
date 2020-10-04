// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare47
{
	public class Door : MonoBehaviour
    {
		#region Fields / Properties
		[HorizontalLine(1, order = 0), Section("Door", order = 1)]
		[SerializeField] private bool isOpen = false;
		public bool IsOpen => IsOpen; 
        #endregion
		
		#region Methods
		public void Open()
		{
			if (isOpen) return; 
			// Cast the animation to open the door
		}

		public void Close()
		{
			if (!isOpen) return;
			// Cast the animation to close the door
		}
		#endregion
    }
}
