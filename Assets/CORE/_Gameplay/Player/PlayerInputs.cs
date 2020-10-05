// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LudumDare47
{
    [CreateAssetMenu(fileName = "ATT_PlayerInputs", menuName = "Attributes/Player Inputs", order = 50)]
    public class PlayerInputs : ScriptableObject
    {
        #region Inputs
        [HorizontalLine(1, order = 0), Section("PLAYER INPUTS", order = 1)]

        public InputAction Move = new InputAction();
        public InputAction Action = new InputAction();
        public InputAction Forward = new InputAction();

        [HorizontalLine(1)]

        public InputAction Loop = new InputAction();
        public InputAction ResetLoop = new InputAction();
        #endregion
    }
}
