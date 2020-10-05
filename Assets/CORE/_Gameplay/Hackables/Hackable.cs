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

        // -----------------------

        [HorizontalLine(1)]

        [SerializeField] private float hackDuration = 0;

        // -----------------------

        [HorizontalLine(1)]

        [SerializeField, ReadOnly] private bool isHacked = false;
        [SerializeField, ReadOnly] private bool isBeingHack = false;
        [SerializeField, ReadOnly] private float hackProgression = 0;
        #endregion

        #region Methods

        #region Reset
        public virtual void ResetBehaviour()
        {
            isHacked = false;
            collider.enabled = false;
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

                isHacked = true;
                collider.enabled = true;
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
