// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare47
{
	public class CoffeeMachine : Hackable
    {
        #region Fields / Properties
        [HorizontalLine(1, order = 0), Section("COFFEE MACHINE", order = 1)]

        [SerializeField, Required] private GameObject originalSprite = null;
        [SerializeField, Required] private GameObject hackSprite = null;
        #endregion

        #region Methods
        public override void ResetBehaviour()
        {
            base.ResetBehaviour();

            hackSprite.SetActive(false);
            originalSprite.SetActive(true);
        }

        public override void OnEnter(GameObject _gameObject)
        {
            if (_gameObject.TryGetComponent(out EnemyController _enemy))
            {
                collider.enabled = false;
                trigger.enabled = false;

                originalSprite.SetActive(false);
                hackSprite.SetActive(true);

                hackLight.SetActive(false);

                _enemy.Die();

                //Instantiate(ProgramSettings.I.PlantExplosion, transform.position, Quaternion.identity);
                LevelManager.Instance.PlayDialog(ProgramSettings.I.plantDialogID);
            }
        }
        #endregion
    }
}
