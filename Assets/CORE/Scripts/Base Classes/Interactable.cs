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
                return true;
            }
            return false;
        }

        public void ResetBehaviour()
        {
            if (isInteract)
            {
                isInteract = false;
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
