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

        [SerializeField, Required] private UIManager uiManager = null;
        [SerializeField, Required] private UpdateManager updateManager = null;

        public UIManager UIManager => uiManager;
        public UpdateManager UpdateManager => updateManager;

        public LevelManager LevelManager = null;

        // -----------------------

        [SerializeField, ReadOnly] private float timeCoef = 1;
        public static float DeltaTime => Time.deltaTime * Instance.timeCoef;
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
