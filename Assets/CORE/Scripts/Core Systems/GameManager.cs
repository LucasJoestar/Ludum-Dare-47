// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        [SerializeField, ReadOnly] private int levelIndex = 0;
        [SerializeField, ReadOnly] private float timeCoef = 1;
        public float TimeCoef => timeCoef;

        public static float DeltaTime => Time.deltaTime * Instance.timeCoef;
        #endregion

        #region Methods

        #region Time
        public void SetTimeCoef(float _value)
        {
            timeCoef = _value;
            UIManager.Instance.UpdateForwardIcon(_value);
            LevelManager.Instance.UpdateTimeCoef(_value);
        }
        #endregion

        #region Levels
        public void LoadNextLevel()
        {
            if (levelIndex != 0)
                SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

            levelIndex++;
            if (levelIndex == SceneManager.sceneCount)
                levelIndex = 1;

            SceneManager.LoadScene(levelIndex, LoadSceneMode.Additive);
        }

        public void UpdateLoadedScene()
        {
            Scene _scene = SceneManager.GetSceneAt(1);
            SceneManager.SetActiveScene(_scene);
            //levelIndex = _scene.buildIndex;
        }
        #endregion

        #region Monobehaviour
        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;

#if !UNITY_EDITOR
                LoadNextLevel();
#else
                levelIndex = 1;
#endif
            }
            else
                Destroy(gameObject);
        }
        #endregion

        #endregion
    }
}
