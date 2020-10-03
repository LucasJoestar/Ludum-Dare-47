// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare47
{
	public class LudumBehaviour : MonoBehaviour
    {
        #region Fields / Properties
        [HorizontalLine(1, order = 0), Section("LUDUM BEHAVIOUR", order = 1)]

        [SerializeField] protected int id = 0;
        public int ID => id;
        #endregion

        #region Methods
        /// <summary>
        /// Compare two object.
        /// True if they are the same, false otherwise.
        /// </summary>
        public bool Compare(LudumBehaviour _other) => id == _other.ID;

        private void Start() => id = GetInstanceID();
        #endregion
    }
}
