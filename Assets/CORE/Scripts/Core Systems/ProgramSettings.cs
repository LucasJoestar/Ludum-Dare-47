// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare47
{
    [CreateAssetMenu(fileName = "DAT_ProgramSettings", menuName = "Datas/Program Settings", order = 50)]
    public class ProgramSettings : ScriptableObject
    {
        #region Datas
        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static ProgramSettings I => GameManager.Instance.ProgramSettings;

        // -----------------------

        [HorizontalLine(1, order = 0), Section("PROGRAM SETTINGS", order = 1)]

        [Min(.1f)] public float RotationSpeed = 5;
        [Min(.1f)] public float DialogDisplay = 5;

        // -----------------------

        [HorizontalLine(1, order = 0), Section("PREFABS", order = 1)]

        public GameObject PlantExplosion = null;
        public GameObject WalkSmoke = null;
        #endregion
    }
}
