// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using UnityEngine;

namespace LudumDare47
{
	public abstract class Trigger : MonoBehaviour
    {
        #region Methods
        public int ID { get; private set; }

        // -----------------------

        public virtual void OnEnter(GameObject _gameObject) { }

        public virtual void OnExit(GameObject _gameObject) { }

        // -----------------------

        /// <summary>
        /// Compare two object.
        /// True if they are the same, false otherwise.
        /// </summary>
        public bool Compare(Trigger _other) => ID == _other.ID;

        private void Start() => ID = GetInstanceID();
        #endregion
    }
}
