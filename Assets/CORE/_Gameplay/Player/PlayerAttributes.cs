// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare47
{
    [CreateAssetMenu(fileName = "ATT_PlayerAttributes", menuName = "Attributes/Player Attributes", order = 50)]
    public class PlayerAttributes : ScriptableObject
    {
        #region Attributes
        [HorizontalLine(1, order = 0), Section("PLAYER ATTRIBUTES", order = 1), Space(order = 2)]

        public AnimationCurve SpeedCurve = null;
        public PlayerGhost ghostPrefab = null;

        [HorizontalLine(1)]

        [Min(0)] public float MovementDecel = 75f;
        [Min(0)] public float AboutTurnAccel = 6f;

        [HorizontalLine(1)]

        public LayerMask InteractMask = new LayerMask();
        #endregion
    }
}
