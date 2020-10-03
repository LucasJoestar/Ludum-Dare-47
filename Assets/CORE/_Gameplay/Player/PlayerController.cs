// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare47
{
	public class PlayerController : PlayerBehaviour, IInputUpdate
    {
        #region Fields / Properties
		[HorizontalLine(1, order = 0), Section("PLAYER CONTROLLER", order = 1)]

        [SerializeField, Required] private PlayerInputs inputs = null;

        // -----------------------

        [SerializeField, ReadOnly] private bool isPlayable = true;
        #endregion

        #region Methods

        #region Input Management
        void IInputUpdate.Update()
        {
            if (isPlayable)
            {
                Move(inputs.Move.ReadValue<Vector2>());
            }
        }

        // -----------------------

        private void EnableInputs()
        {
            inputs.Move.Enable();
            inputs.Action.Enable();
        }

        private void DisableInputs()
        {
            inputs.Move.Disable();
            inputs.Action.Disable();
        }
        #endregion

        #region Monobehaviour
        protected override void OnEnable()
        {
            base.OnEnable();

            EnableInputs();
            UpdateManager.Instance.Register((IInputUpdate)this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            DisableInputs();
            UpdateManager.Instance.Unregister((IInputUpdate)this);
        }
        #endregion

        #endregion
    }
}
