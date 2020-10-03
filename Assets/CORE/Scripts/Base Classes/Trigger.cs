// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using UnityEngine;

namespace LudumDare47
{
	public class Trigger : LudumBehaviour
    {
        #region Methods
        public virtual void OnEnter(Movable _movable) { }

        public virtual void OnExit(Movable _movable) { }
        #endregion
    }
}
