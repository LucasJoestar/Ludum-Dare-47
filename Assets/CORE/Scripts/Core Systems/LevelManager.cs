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

        [SerializeField, Required] private PlayerController player = null;

        // -----------------------

        [HorizontalLine(1)]

        [SerializeField] private float loopDuration = 30;

        // -----------------------

        [HorizontalLine(1)]

        [SerializeField, ReadOnly] private bool isLooping = true;
        [SerializeField, ReadOnly] private float loopTime = 0;

        public float LoopTime => loopTime;

        // -----------------------

        [HorizontalLine(1)]

        [SerializeField, ReadOnly] private Vector2 playerStartPosition = new Vector2();
        [SerializeField, ReadOnly] private List<PlayerGhost> ghosts = new List<PlayerGhost>();
        #endregion

        #region Methods

        #region Loop State
        void ILateUpdate.Update()
        {
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
        }

        /// <summary>
        /// Completely reset and restart this level loop.
        /// </summary>
        public void ResetLoop()
        {
            for (int _i = 0; _i < ghosts.Count; _i++)
                Destroy(ghosts[_i].gameObject);

            ghosts.Clear();
            Loop();
        }
        #endregion

        #region Monobehaviour
        private void Awake()
        {
            GameManager.Instance.LevelManager = this;
            UpdateManager.Instance.Register(this);

            playerStartPosition = player.transform.position;
        }

        protected virtual void OnDisable()
        {
            UpdateManager.Instance.Unregister(this);
        }
        #endregion

        #endregion
    }
}
