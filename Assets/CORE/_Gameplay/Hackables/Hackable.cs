// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare47
{
	public class Hackable : MonoBehaviour, IInteractable
    {
        #region Fields / Properties
        [HorizontalLine(1, order = 0), Section("HACKABLE", order = 1)]

        [SerializeField] private float hackDuration = 0;

        // -----------------------

        [HorizontalLine(1)]

        [SerializeField, Required] private bool isBeingHack = false;
        [SerializeField, Required] private float hackProgression = 0;
        #endregion

        #region Methods
        public bool Interact(IPlayerBehaviour _player)
        {
            if (!isBeingHack)
            {
                isBeingHack = true;
                hackProgression = 0;

                // Display Hack UI.

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
                return true;
            }
            return false;
        }

        public void CancelHack()
        {
            if (isBeingHack)
            {
                isBeingHack = false;

                // Hide Hack UI.
            }
        }
        #endregion
    }
}
