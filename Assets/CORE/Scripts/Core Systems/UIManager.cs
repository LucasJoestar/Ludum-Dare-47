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

        [SerializeField, Required] private Canvas canvas = null;

        [HorizontalLine(1)]

        [SerializeField, Required] private GameObject loopAnchor = null;
        [SerializeField, Required] private RectTransform loopGaugeParent = null;
        [SerializeField, Required] private RectTransform loopGauge = null;
        [SerializeField, Required] private TextMeshProUGUI forwardText = null;
        [SerializeField, Required] private GameObject forwardImage = null;
        [SerializeField, Required] private Image loopGaugeImage = null;
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

        [HorizontalLine(1)]

        [SerializeField, Required] private GameObject loopInfo = null;

        // -----------------------

        private readonly int EnterDialog_Anim = Animator.StringToHash("Enter");
        private readonly int CloseDialog_Anim = Animator.StringToHash("Close");

        private readonly int BlackBars_Anim = Animator.StringToHash("Switch");
        private readonly int FadeToBlack_Anim = Animator.StringToHash("IsFading");
        private readonly int FadeOver_Anim = Animator.StringToHash("IsOver");
        #endregion

        #region Methods

        #region Camera
        public void SetCamera(Camera _camera) => canvas.worldCamera = _camera;
        #endregion

        #region Loop
        public void UpdateForwardIcon(float _value)
        {
            bool _isForward = _value > 1;

            if (forwardImage.activeInHierarchy != _isForward)
                forwardImage.SetActive(_isForward);

            if (_isForward)
                forwardText.text = "x " + (Mathf.CeilToInt((int)_value / 2) * 2).ToString();
        }

        public void UpdateLoopUI(float _loopTime, float _percent)
        {
            loopTime.text = _loopTime.ToString("0.00");
            loopGaugeImage.fillAmount = _percent;
            loopGauge.anchoredPosition = new Vector2(loopGaugeParent.sizeDelta.x * _percent, loopGauge.anchoredPosition.y);
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

        public void DisplayLoopUI(bool _doDisplay)
        {
            if (loopAnchor.activeInHierarchy != _doDisplay)
                loopAnchor.SetActive(_doDisplay);
        }
        #endregion

        #region Animations
        public void SwitchBlackBars() => blackBarsAnimator.SetTrigger(BlackBars_Anim);

        public void FadeToBlack(bool _isFading) => fadeAnimator.SetBool(FadeToBlack_Anim, _isFading);

        public void FadeOver(bool isOver)
        {
            loopInfo.SetActive(isOver);
            fadeAnimator.SetBool(FadeOver_Anim, isOver);
        }
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
