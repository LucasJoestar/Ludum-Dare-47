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

		[SerializeField] private DoorOpener[] doorOpeners = new DoorOpener[] { };
        #endregion
		
		#region Methods
		public void ForceOpenning()
		{
			if (isOpen) return;
			Debug.Log("Open!");
			// Cast the animation to open the door
		}
		private void Open()
		{
			Debug.Log("open"); 
		}

		private void Close()
		{
			Debug.Log("close");
		}

		public void UpdateOpenningStatus()
		{
			bool _tempOpen = true; 
			for (int i = 0; i < doorOpeners.Length; i++)
			{
				if (!doorOpeners[i].IsActivated)
				{
					_tempOpen = false;
					break;
				}
			}
			isOpen = _tempOpen;
			if (isOpen)
				Open();
			else Close();
		}

		private void Start()
		{
			for (int i = 0; i < doorOpeners.Length; i++)
			{
				doorOpeners[i].LinkToDoor(this);
			}
		}

		private void OnDrawGizmos()
		{
			for (int i = 0; i < doorOpeners.Length; i++)
			{
				Gizmos.DrawLine(transform.position, doorOpeners[i].transform.position);
			}
		}
		#endregion
	}
}
