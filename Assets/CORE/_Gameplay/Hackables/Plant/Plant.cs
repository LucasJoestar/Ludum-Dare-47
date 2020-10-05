// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare47
{
	public class Plant : Hackable
    {
        #region Fields / Properties
        [HorizontalLine(1, order = 0), Section("PLANT", order = 1)]

        [SerializeField] private GameObject sprite = null;
        #endregion

        #region Methods

        #region Behaviour
        public override void ResetBehaviour()
        {
            base.ResetBehaviour();
            sprite.SetActive(true);
        }

        public override void OnEnter(GameObject _gameObject)
        {
            collider.enabled = false;
        }
        #endregion

        #endregion
    }
}
