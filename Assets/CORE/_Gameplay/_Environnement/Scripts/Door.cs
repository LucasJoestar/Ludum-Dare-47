// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare47
{
	public class Door : MonoBehaviour, IResetable
    {
		#region Fields / Properties
		public static readonly int Switch_Anim = Animator.StringToHash("Switch");

		[HorizontalLine(1, order = 0), Section("Door", order = 1)]
		[SerializeField] private bool isOpenAtStart = false;
		[SerializeField, ReadOnly] private bool isOpen = false; 
		public bool IsOpen => IsOpen;

		[SerializeField] private DoorOpener[] doorOpeners = new DoorOpener[] { };
		[SerializeField, Required] private Animator animator = null;
		[SerializeField, Required] private Collider2D doorCollider = null; 
        #endregion
		
		#region Methods
		public void ForceOpenning()
		{
			if (isOpen) return;
			isOpen = true; 
			animator.SetTrigger(Switch_Anim); 
		}
		private void Open()
		{
			if (isOpen)
			{
				animator.SetTrigger(Switch_Anim);
				doorCollider.enabled = false;
			}
		}

		private void Close()
		{
			if (!isOpen)
			{
				animator.SetTrigger(Switch_Anim);
				doorCollider.enabled = false; 
			}
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

		public void ResetBehaviour()
		{
			if(isOpen != isOpenAtStart)
			{
				isOpen = isOpenAtStart;
				if (isOpen) Open();
				else Close(); 
			}
		}

		private void Start()
		{
			LevelManager.Instance.RegisterResetable(this);
			isOpen = isOpenAtStart;
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
