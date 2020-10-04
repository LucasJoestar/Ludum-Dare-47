// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare47
{
	public class TutorialLevel : LevelManager
    {
        #region Fields / Properties
        [HorizontalLine(1, order = 0), Section("LEVEL MANAGER", order = 1)]

        [SerializeField] private float cutsceneTime = 5;

        // -----------------------

        private bool isWaitingCutscene = true;
        #endregion

        #region Methods
        private bool hasGlitched = false;

        protected override void LevelUpdate()
        {
            // Intro cutscene.
            if (isWaitingCutscene)
            {
                cutsceneTime -= Time.deltaTime;
                if (!hasGlitched && (cutsceneTime <= 3.5f))
                {
                    camera.Glitch();
                    hasGlitched = true;
                }

                if (cutsceneTime <= 0)
                {
                    isWaitingCutscene = false;
                    UIManager.Instance.SwitchBlackBars();

                    if (startDialogID != 0)
                        PlayDialog(startDialogID);
                }
            }
            else
                base.LevelUpdate();
        }

        protected override void Start()
        {
            GameManager.Instance.LevelManager = this;
            GameManager.Instance.UpdateLoadedScene();

            UpdateManager.Instance.Register(this);
            UIManager.Instance.FadeToBlack(false);

            playerStartPosition = player.transform.position;
            player.IsPaused = true;

            GameManager.Instance.TimeCoef = 0;
            UIManager.Instance.SwitchBlackBars();
        }
        #endregion
    }
}
