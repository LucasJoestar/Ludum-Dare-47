// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare47
{
	public class EndLevelTrigger : Trigger
    {
        #region Trigger
        [SerializeField, Required] private new Collider2D collider = null;

        public override void OnEnter(GameObject _gameObject)
        {
            collider.enabled = false;
            LevelManager.Instance.EndLevel();
        }
        #endregion
    }
}
