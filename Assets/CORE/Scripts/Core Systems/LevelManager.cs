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

        public CameraBehaviour Camera => camera;

        // -----------------------

        [HorizontalLine(1)]

        [SerializeField] protected int startDialogID = 0;
        [SerializeField] protected float loopDuration = 30;

        [Space]

        [SerializeField] private Vector2 endLevelMovement = Vector2.up;

        [SerializeField] private List<Animator> animators = new List<Animator>();
        private List<IResetable> resetables = new List<IResetable>();

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

        // -----------------------

        private readonly uint doTalk_ID = AkSoundEngine.GetIDFromString("Play_talk");
        private readonly uint noTalk_ID = AkSoundEngine.GetIDFromString("Stop_talk");
        #endregion

        #region Methods

        #region Management
        private readonly int speed_Anim = Animator.StringToHash("Speed");

        public void UpdateTimeCoef(float _timeCoef)
        {
            camera.Forward(_timeCoef > 1);
            for (int _i = 0; _i < animators.Count; _i++)
                animators[_i].SetFloat(speed_Anim, _timeCoef);
        }

        public void RegisterAnimator(Animator _animator) => animators.Add(_animator);

        /// <summary>
        /// Register a resetable on this level.
        /// </summary>
        public void RegisterResetable(IResetable _resetable) => resetables.Add(_resetable);
        #endregion

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
                        AkSoundEngine.PostEvent(noTalk_ID, gameObject);
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

        private bool isHardReset = false;

        public void StopLoop()
        {
            isLooping = false;
            player.OnEndLoop();
            GameManager.Instance.SetTimeCoef(0);
            
            // Do stop enemies ?
        }

        public void StartLoop()
        {
            camera.BigGlitch(false);
            camera.Loop();
        }

        /// <summary>
        /// Restart this level loop.
        /// </summary>
        public void Loop()
        {
            // Reset all interactables, enemies state
            // and player ghosts.
            isLooping = true;
            loopTime = 0;

            if (isHardReset)
            {
                isHardReset = false;
                UIManager.Instance.ResetUI(loopDuration);

                for (int _i = 0; _i < ghosts.Count; _i++)
                    Destroy(ghosts[_i].gameObject);

                ghosts.Clear();
                player.OnStartLoop(playerStartPosition, false);
            }
            else
            {
                ghosts.Add(player.OnStartLoop(playerStartPosition, true));
                for (int _i = 0; _i < ghosts.Count; _i++)
                    ghosts[_i].ResetBehaviour(playerStartPosition);
            }

            GameManager.Instance.SetTimeCoef(1);
            UIManager.Instance.UpdateGhostAmount(ghosts.Count);
            UIManager.Instance.FadeOver(false);

            // Reset all behaviours.
            for (int _i = 0; _i < resetables.Count; _i++)
                resetables[_i].ResetBehaviour();
        }

        /// <summary>
        /// Completely reset and restart this level loop.
        /// </summary>
        public void ResetLoop()
        {
            isHardReset = true;
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
                GameManager.Instance.SetTimeCoef(0);
            }

            // Display dialog.
            UIManager.Instance.DisplayDialog(_dialog.Sentence, _dialog.Sprite);
        }

        public void ActivateDialog()
        {
            isInDialog = true;
            AkSoundEngine.PostEvent(doTalk_ID, gameObject);
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
                AkSoundEngine.PostEvent(noTalk_ID, gameObject);
                UIManager.Instance.UpdateDialog(dialogDisplay);
            }
            else if (nextDialogID == 0)
            {
                isInDialog = false;
                UIManager.Instance.EndDialog();

                if (!isDialogAutomatic)
                {
                    player.SetInDialog(false);
                    GameManager.Instance.SetTimeCoef(1);
                    UIManager.Instance.DisplayLoopUI(true);
                }
                else
                    isDialogAutomatic = false;
            }
            else
                PlayDialog(nextDialogID);
        }
        #endregion

        #region Monobehaviour
        private void Awake()
        {
            GameManager.Instance.LevelManager = this;
            UpdateManager.Instance.Register(this);
        }

        protected virtual void Start()
        {
            GameManager.Instance.UpdateLoadedScene();

            UIManager.Instance.FadeToBlack(false);
            UIManager.Instance.ResetUI(loopDuration);

            playerStartPosition = player.transform.position;
            if (startDialogID != 0)
            {
                PlayDialog(startDialogID);
                UIManager.Instance.DisplayLoopUI(false);
            }
            else
                UIManager.Instance.DisplayLoopUI(true);
        }

        protected virtual void OnDisable()
        {
            UpdateManager.Instance.Unregister(this);
        }
        #endregion

        #endregion
    }
}
