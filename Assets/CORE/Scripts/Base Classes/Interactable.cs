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
	public class Interactable : MonoBehaviour, IInteractable, IResetable
    {
        #region Fields / Properties
        [HorizontalLine(1, order = 0), Section("Interactable", order = 1)]

        [SerializeField] private UnityEvent OnInteract = new UnityEvent();
        [SerializeField] private UnityEvent OnReset = new UnityEvent();

        [SerializeField] private bool isSwitch = false;
        [SerializeField] private bool isLever = false;

        [SerializeField, Required] protected GameObject info = null;

        // -----------------------

        public static readonly uint Switch_ID = AkSoundEngine.GetIDFromString("Play_door_pressure_plate");
        public static readonly uint Lever_ID = AkSoundEngine.GetIDFromString("Play_lever_pull");
        #endregion

        #region Methods

        #region Interactions
        private bool isInteract = false;

        // -----------------------

        public bool Interact(IPlayerBehaviour _player)
        {
            if (!isInteract)
            {
                isInteract = true;
                OnInteract.Invoke();
                info.SetActive(false);

                if (isSwitch)
                    AkSoundEngine.PostEvent(Switch_ID, gameObject);
                else if (isLever)
                    AkSoundEngine.PostEvent(Lever_ID, gameObject);

                return true;
            }
            return false;
        }

        public void ResetBehaviour()
        {
            if (isInteract)
            {
                isInteract = false;
                info.SetActive(true);
                OnReset.Invoke();
            }
        }
        #endregion

        #region Monobehaviour
        private void Start()
        {
            LevelManager.Instance.RegisterResetable(this);
        }
        #endregion

        #endregion
    }
}
