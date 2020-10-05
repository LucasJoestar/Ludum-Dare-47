// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using UnityEngine;

namespace LudumDare47
{
	public class Plant : Hackable
    {
        #region Methods
        public override void ResetBehaviour()
        {
            base.ResetBehaviour();
            gameObject.SetActive(true);
        }

        public override void OnEnter(GameObject _gameObject)
        {
            if (_gameObject.TryGetComponent(out EnemyController _enemy))
            {
                gameObject.SetActive(false);
                //_enemy.Die();

                Instantiate(ProgramSettings.I.PlantExplosion, transform.position, Quaternion.identity);
                LevelManager.Instance.PlayDialog(ProgramSettings.I.plantDialogID);
            }
        }
        #endregion
    }
}
