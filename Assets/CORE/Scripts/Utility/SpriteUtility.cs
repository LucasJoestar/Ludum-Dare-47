// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using UnityEngine;

namespace LudumDare47
{
	public static class SpriteUtility
    {
        #region Methods
        /// <summary>
        /// Update the sorting order of a sprite.
        /// </summary>
        public static void Order(SpriteRenderer _sprite)
        {
            _sprite.sortingOrder = (int)(_sprite.transform.position.y * -100);
        }

        /// <summary>
        /// Update the sorting order of some sprites.
        /// </summary>
        public static void Order(SpriteRenderer[] _sprite)
        {
            for (int _i = 0; _i < _sprite.Length; _i++)
                _sprite[_i].sortingOrder = (int)(_sprite[_i].transform.position.y * -100);
        }
        #endregion
    }
}
