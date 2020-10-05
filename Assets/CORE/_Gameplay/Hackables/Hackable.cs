// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LudumDare47
{
	public abstract class Hackable : Trigger, IInteractable, IResetable
    {
        #region Fields / Properties
        [HorizontalLine(1, order = 0), Section("HACKABLE", order = 1)]

        [SerializeField, Required] private GameObject canvas = null;
        [SerializeField, Required] private Image gauge = null;

        [Space()]

        [SerializeField, Required] protected new Collider2D collider = null;
        [SerializeField, Required] protected Collider2D interactable = null;
        [SerializeField, Required] protected Collider2D trigger = null;

        [Space()]

        [SerializeField, Required] protected GameObject hackLight = null;
        [SerializeField, Required] protected GameObject info = null;

        // -----------------------

        [HorizontalLine(1)]

        [SerializeField] private float hackDuration = 0;

        // -----------------------

        [HorizontalLine(1)]

        [SerializeField, ReadOnly] private bool isHacked = false;
        [SerializeField, ReadOnly] private bool isBeingHack = false;
        [SerializeField, ReadOnly] private float hackProgression = 0;

        // -----------------------

        private readonly uint hackLoopStart_ID = AkSoundEngine.GetIDFromString("Play_hacking_loop");
        private readonly uint hackLoopEnd_ID = AkSoundEngine.GetIDFromString("Stop_hacking_loop");
        private readonly uint hackValidate_ID = AkSoundEngine.GetIDFromString("Play_hack_validation");
        #endregion

        #region Methods

        #region Reset
        public virtual void ResetBehaviour()
        {
            isHacked = false;

            collider.enabled = true;
            interactable.enabled = true;
            trigger.enabled = false;

            info.SetActive(true);
            hackLight.SetActive(false);
        }
        #endregion

        #region Hack
        public bool Interact(IPlayerBehaviour _player)
        {
            if (!isHacked && !isBeingHack)
            {
                isBeingHack = true;
                hackProgression = 0;

                // Display Hack UI.
                canvas.SetActive(true);
                gauge.fillAmount = 0;

                _player.Hack(this);
                AkSoundEngine.PostEvent(hackLoopStart_ID, gameObject);
                return true;
            }
            return false;
        }

        public bool UpdateHack()
        {
            hackProgression += GameManager.DeltaTime;
            if (hackProgression >= hackDuration)
            {
                hackProgression = hackDuration;
                isBeingHack = false;

                // Hide Hack UI.
                canvas.SetActive(false);
                AkSoundEngine.PostEvent(hackLoopEnd_ID, gameObject);
                AkSoundEngine.PostEvent(hackValidate_ID, gameObject);

                isHacked = true;
                trigger.enabled = true;
                interactable.enabled = false;

                info.SetActive(false);
                hackLight.SetActive(true);
                return true;
            }

            gauge.fillAmount = hackProgression / hackDuration;
            return false;
        }

        public void CancelHack()
        {
            if (isBeingHack)
            {
                isBeingHack = false;

                // Hide Hack UI.
                canvas.SetActive(false);
                AkSoundEngine.PostEvent(hackLoopEnd_ID, gameObject);
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
