﻿// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

namespace LudumDare47
{
    public static class BoolExtensions
    {
        #region Methods
        /// <summary>
        /// Get this boolean as a sign.
        /// 1 if true, -1 otherwise.
        /// </summary>
        /// <param name="_bool">Boolean to get sign from.</param>
        /// <returns>Returns this boolean sign as 1 or -1.</returns>
        public static int Sign(this bool _bool) => Mathm.Sign(_bool);
        #endregion
    }
}
