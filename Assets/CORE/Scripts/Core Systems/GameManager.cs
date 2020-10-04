// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare47
{
    public class GameManager : MonoBehaviour
    {
        #region Fields / Properties
        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static GameManager Instance { get; private set; } = null;

        // -----------------------

        [HorizontalLine(1, order = 0), Section("GAME MANAGER", order = 1)]

        [SerializeField, Required] private DialogDatabase dialogDatabase = null;
        [SerializeField, Required] private ProgramSettings programSettings = null;

        public DialogDatabase DialogDatabase => dialogDatabase;
        public ProgramSettings ProgramSettings => programSettings;

        [Space]

        [SerializeField, Required] private UIManager uiManager = null;
        [SerializeField, Required] private UpdateManager updateManager = null;

        public UIManager UIManager => uiManager;
        public UpdateManager UpdateManager => updateManager;

        [HideInInspector] public LevelManager LevelManager = null;

        // -----------------------

        [HorizontalLine(1)]

        [ReadOnly] public float TimeCoef = 1;
        public static float DeltaTime => Time.deltaTime * Instance.TimeCoef;
        #endregion

        #region Methods

        #region Monobehaviour
        private void Awake()
        {
            if (!Instance)
                Instance = this;
            else
                Destroy(gameObject);
        }
        #endregion

        #endregion
    }
}
