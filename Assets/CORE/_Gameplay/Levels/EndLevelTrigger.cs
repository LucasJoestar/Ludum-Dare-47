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

        // -----------------------

        private readonly uint endLevel_ID = AkSoundEngine.GetIDFromString("Play_end_level");

        public override void OnEnter(GameObject _gameObject)
        {
            collider.enabled = false;

            AkSoundEngine.PostEvent(endLevel_ID, gameObject);
            LevelManager.Instance.EndLevel();
        }
        #endregion
    }
}
