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
        [SerializeField, Required] private GameObject ghostAnchor = null;
        [SerializeField, Required] private TextMeshProUGUI ghostAmount = null;

        [HorizontalLine(1)]

        [SerializeField, Required] private Animator dialogAnchor = null;
        [SerializeField, Required] private TextMeshProUGUI dialogText = null;
        [SerializeField, Required] private Image dialogIcon = null;

        [HorizontalLine(1)]

        [SerializeField, Required] private Animator blackBarsAnimator = null;
        [SerializeField, Required] private Animator fadeAnimator = null;

        // -----------------------

        private readonly int EnterDialog_Anim = Animator.StringToHash("Enter");
        private readonly int CloseDialog_Anim = Animator.StringToHash("Close");

        private readonly int BlackBars_Anim = Animator.StringToHash("Switch");
        private readonly int FadeToBlack_Anim = Animator.StringToHash("IsFading");
        #endregion

        #region Methods

        #region Loop
        public void UpdateLoopUI(float _loopTime, float _percent)
        {
            loopTime.text = _loopTime.ToString("0.00");
            loopGauge.fillAmount = _percent;
        }


        public void UpdateGhostAmount(int _amount)
        {
            if (_amount == 1)
                ghostAnchor.SetActive(true);

            ghostAmount.text = _amount.ToString("00");
        }

        public void ResetUI(float _loopTime)
        {
            ghostAnchor.SetActive(false);
            UpdateLoopUI(_loopTime, 0);
        }
        #endregion

        #region Animations
        public void SwitchBlackBars() => blackBarsAnimator.SetTrigger(BlackBars_Anim);

        public void FadeToBlack(bool _isFading) => fadeAnimator.SetBool(FadeToBlack_Anim, _isFading);
        #endregion

        #region Dialogs
        private bool isInDialog = false;

        /// <summary>
        /// Display a dialog on screen.
        /// </summary>
        public void DisplayDialog(string _sentence, Sprite _sprite)
        {
            if (!isInDialog)
            {
                isInDialog = true;
                dialogAnchor.SetTrigger(EnterDialog_Anim);
            }
            else
                LevelManager.Instance.ActivateDialog();

            dialogText.text = _sentence;
            dialogIcon.sprite = _sprite;

            dialogText.maxVisibleCharacters = 0;
        }

        public void UpdateDialog(int _wordCount) => dialogText.maxVisibleCharacters = _wordCount;

        /// <summary>
        /// Ends a dialog and hide dialog box.
        /// </summary>
        public void EndDialog()
        {
            dialogAnchor.SetTrigger(CloseDialog_Anim);
            isInDialog = false;
        }
        #endregion

        #endregion
    }
}
