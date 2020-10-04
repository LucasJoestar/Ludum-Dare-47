// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using System.Collections.Generic;
using UnityEngine;

namespace LudumDare47
{
	public class LevelManager : MonoBehaviour, ILateUpdate
    {
        #region Fields / Properties
        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static LevelManager Instance => GameManager.Instance.LevelManager;

        // -----------------------

        [HorizontalLine(1, order = 0), Section("LEVEL MANAGER", order = 1)]

        [SerializeField, Required] protected new CameraBehaviour camera = null;
        [SerializeField, Required] protected PlayerController player = null;

        // -----------------------

        [HorizontalLine(1)]

        [SerializeField] protected int startDialogID = 0;
        [SerializeField] private float loopDuration = 30;

        [Space]

        [SerializeField] private Vector2 endLevelMovement = Vector2.up;
        [SerializeField] private IResetable[] resetables = new IResetable[] { };

        // -----------------------

        [HorizontalLine(1)]

        [SerializeField, ReadOnly] private bool isLooping = true;
        [SerializeField, ReadOnly] private float loopTime = 0;

        public float LoopTime => loopTime;

        // -----------------------

        [HorizontalLine(1)]

        [SerializeField, ReadOnly] protected bool isInDialog = false;
        [SerializeField, ReadOnly] private bool isDisplayingDialog = false;
        [SerializeField, ReadOnly] private bool isDialogAutomatic = false;

        [Space]

        [SerializeField, ReadOnly] private int dialogDisplay = 0;
        [SerializeField, ReadOnly] private float dialogDisplayVar = 0;

        [Space]

        [SerializeField, ReadOnly] private float dialogDurationVar = 0;
        [SerializeField, ReadOnly] private int nextDialogID = 0;

        // -----------------------

        [HorizontalLine(1)]

        [SerializeField, ReadOnly] protected Vector2 playerStartPosition = new Vector2();
        [SerializeField, ReadOnly] private List<PlayerGhost> ghosts = new List<PlayerGhost>();
        #endregion

        #region Methods

        #region Loop State
        private bool isLevelEnded = false;
        private float endLevelTimer = 5;

        public void EndLevel()
        {
            isLevelEnded = true;
            player.IsPaused = true;

            camera.enabled = false;
            UIManager.Instance.FadeToBlack(true);
            UIManager.Instance.SwitchBlackBars();
        }

        // -----------------------

        void ILateUpdate.Update() => LevelUpdate();

        protected virtual void LevelUpdate()
        {
            // End level.
            if (isLevelEnded)
            {
                player.Move(endLevelMovement);
                endLevelTimer -= Time.deltaTime;

                if (endLevelTimer <= 0)
                {
                    // Load things.
                    gameObject.SetActive(false);
                    UIManager.Instance.SwitchBlackBars();
                    GameManager.Instance.LoadNextLevel();
                }
                return;
            }

            // Dialog update.
            if (isInDialog)
            {
                if (isDisplayingDialog)
                {
                    dialogDisplayVar += (isDialogAutomatic ? GameManager.DeltaTime : Time.deltaTime) * ProgramSettings.I.DialogDisplay;
                    if (dialogDisplayVar >= dialogDisplay)
                    {
                        isDisplayingDialog = false;
                        UIManager.Instance.UpdateDialog(dialogDisplay);
                    }
                    else
                        UIManager.Instance.UpdateDialog((int)dialogDisplayVar);
                }
                else if (isDialogAutomatic)
                {
                    dialogDurationVar -= GameManager.DeltaTime;
                    if (dialogDurationVar <= 0)
                        EndDialog();
                }
            }

            if (isLooping)
            {
                // Update ghosts.
                for (int _i = 0; _i < ghosts.Count; _i++)
                    ghosts[_i].MovableUpdate();

                loopTime += GameManager.DeltaTime;
                if (loopTime >= loopDuration)
                {
                    loopTime = loopDuration;

                    // Stop everything, and display end loop informations.
                    StopLoop();

                    // Update ghosts.
                    for (int _i = 0; _i < ghosts.Count; _i++)
                        ghosts[_i].MovableUpdate();
                }

                UIManager.Instance.UpdateLoopUI(loopDuration - loopTime, loopTime / loopDuration);
            }
        }

        // -----------------------

        public void StopLoop()
        {
            isLooping = false;
            player.OnEndLoop();
            
            // Do stop enemies ?
        }

        public void StartLoop() => camera.Loop();

        /// <summary>
        /// Restart this level loop.
        /// </summary>
        public void Loop()
        {
            // Reset all interactables, enemies state
            // and player ghosts.
            isLooping = true;
            loopTime = 0;

            ghosts.Add(player.OnStartLoop(playerStartPosition));
            for (int _i = 0; _i < ghosts.Count; _i++)
                ghosts[_i].ResetBehaviour(playerStartPosition);

            UIManager.Instance.UpdateGhostAmount(ghosts.Count);

            // Reset all behaviours.
            for (int _i = 0; _i < resetables.Length; _i++)
                resetables[_i].ResetBehaviour();
        }

        /// <summary>
        /// Completely reset and restart this level loop.
        /// </summary>
        public void ResetLoop()
        {
            for (int _i = 0; _i < ghosts.Count; _i++)
                Destroy(ghosts[_i].gameObject);

            ghosts.Clear();
            StartLoop();
        }
        #endregion

        #region Dialogs
        public void PlayDialog(int _id)
        {
            // Register dialog information.
            Dialog _dialog = GameManager.Instance.DialogDatabase.GetDialog(_id);

            isDisplayingDialog = true;
            dialogDisplay = _dialog.Sentence.Length;
            dialogDisplayVar = 0;
            nextDialogID = _dialog.NextID;

            if (_dialog.Duration > 0)
            {
                isDialogAutomatic = true;
                dialogDurationVar = _dialog.Duration;
            }
            else
            {
                isDialogAutomatic = false;
                player.IsPaused = true;
                GameManager.Instance.TimeCoef = 0;
            }

            // Display dialog.
            UIManager.Instance.DisplayDialog(_dialog.Sentence, _dialog.Sprite);
        }

        public void ActivateDialog()
        {
            isInDialog = true;
            if (!isDialogAutomatic)
            {
                player.IsPaused = false;
                player.SetInDialog(true);
            }
        }

        public void EndDialog()
        {
            if (isDisplayingDialog)
            {
                isDisplayingDialog = false;
                UIManager.Instance.UpdateDialog(dialogDisplay);
            }
            else if (nextDialogID == 0)
            {
                isInDialog = false;
                UIManager.Instance.EndDialog();

                if (!isDialogAutomatic)
                {
                    player.SetInDialog(false);
                    GameManager.Instance.TimeCoef = 1;
                }
                else
                    isDialogAutomatic = false;
            }
            else
                PlayDialog(nextDialogID);
        }
        #endregion

        #region Monobehaviour
        protected virtual void Start()
        {
            GameManager.Instance.LevelManager = this;
            GameManager.Instance.UpdateLoadedScene();

            UpdateManager.Instance.Register(this);
            UIManager.Instance.FadeToBlack(false);

            playerStartPosition = player.transform.position;
            if (startDialogID != 0)
                PlayDialog(startDialogID);
        }

        protected virtual void OnDisable()
        {
            UpdateManager.Instance.Unregister(this);
        }
        #endregion

        #endregion
    }
}
