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

        [SerializeField] private UpdateManager updateManager = null;
        public UpdateManager UpdateManager => updateManager;

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
