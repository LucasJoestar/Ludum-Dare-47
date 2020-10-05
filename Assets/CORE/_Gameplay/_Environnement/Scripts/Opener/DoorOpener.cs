// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;
using UnityEngine.Events;

namespace LudumDare47
{
	public class DoorOpener : Trigger, IInteractable, IResetable
    {
        #region Fields / Properties
        [HorizontalLine(1, order = 0), Section("DoorOpener", order = 1)]

        [SerializeField] private UnityEvent OnEnterEvent = null;
		[SerializeField, ReadOnly] private Door linkedDoor = null;
		public bool IsActivated { get; private set; } = false;

        [SerializeField] private bool isSwitch = false;
        [SerializeField] private bool isLever = false;
        #endregion

        #region Methods
        private int inAmount = 0;

		public bool LinkToDoor(Door _door) => linkedDoor = _door; 

		public bool Interact(IPlayerBehaviour _player)
		{
			IsActivated = !IsActivated;
			linkedDoor.UpdateOpenningStatus(); 
			return true; 
		}

		public override void OnEnter(GameObject _gameObject)
		{
            inAmount++;
            if (isSwitch)
                AkSoundEngine.PostEvent(Interactable.Switch_ID, gameObject);
            else if (isLever)
                AkSoundEngine.PostEvent(Interactable.Lever_ID, gameObject);

            if (inAmount == 1)
            {
                IsActivated = true;
                linkedDoor.UpdateOpenningStatus();
            }

            if (_gameObject.TryGetComponent(out PlayerController _))
                OnEnterEvent.Invoke();
        }
		public override void OnExit(GameObject _gameObject)
		{
            inAmount--;
            if (inAmount == 0)
            {
                IsActivated = false;
                linkedDoor.UpdateOpenningStatus();
            }
		}

		public void ResetBehaviour()
		{
			IsActivated = false; 
		}

		private void Start()
		{
			LevelManager.Instance.RegisterResetable(this);
		}
		#endregion
	}
}
