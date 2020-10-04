// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LudumDare47
{
	public class UIManager : MonoBehaviour
    {
        #region Fields / Properties
        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static UIManager Instance => GameManager.Instance.UIManager;

        // -----------------------

        [HorizontalLine(1, order = 0), Section("UI MANAGER", order = 1)]

        [SerializeField, Required] private Image loopGauge = null;
        [SerializeField, Required] private TextMeshProUGUI loopTime = null;
        #endregion

        #region Methods
        public void UpdateLoopUI(float _loopTime, float _percent)
        {
            loopTime.text = _loopTime.ToString("0.##");
            loopGauge.fillAmount = _percent;
        }
        #endregion
    }
}
