// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare47
{
	public class CameraBehaviour : MonoBehaviour, ILateUpdate
    {
        #region Fields / Properties
        [HorizontalLine(1, order = 0), Section("CAMERA BEHAVIOUR", order = 1)]

        [SerializeField, Required] private new Camera camera = null;
        [SerializeField, Required] private PlayerController player = null;
        [SerializeField, Required] private Animator animator = null;

        // -----------------------

        [HorizontalLine(1)]

        [SerializeField] private Bounds bounds = new Bounds();

        // -----------------------

        private readonly int loop_Anim = Animator.StringToHash("Loop");
        private readonly int glitch_Anim = Animator.StringToHash("Glitch");
        private readonly int forward_Anim = Animator.StringToHash("IsForward");
        private readonly int bigGlitch_Anim = Animator.StringToHash("IsBigGlitch");

        // -----------------------

        private readonly uint loop_ID = AkSoundEngine.GetIDFromString("Play_rewind_reset_");
        private readonly uint rewind_ID = AkSoundEngine.GetIDFromString("Play_reset_fin_");

        private readonly uint doForward_ID = AkSoundEngine.GetIDFromString("Play_start_fastforward");
        private readonly uint noForward_ID = AkSoundEngine.GetIDFromString("Stop_fast_forward");
        #endregion

        #region Methods

        #region Behaviour
        void ILateUpdate.Update()
        {
            Vector3 _position = transform.position;
            _position.z = 0;
            bounds.center = _position;

            _position = player.transform.position;
            if (!bounds.Contains(_position))
            {
                transform.position += (_position - bounds.ClosestPoint(_position)) * .1f;
            }
        }

        #region Animation
        public void Loop()
        {
            animator.SetTrigger(loop_Anim);
            AkSoundEngine.PostEvent(loop_ID, gameObject);
        }

        public void Glitch()
        {
            animator.SetTrigger(glitch_Anim);
            AkSoundEngine.PostEvent(loop_ID, gameObject);
        }

        public void BigGlitch(bool _isBigGlitch) => animator.SetBool(bigGlitch_Anim, _isBigGlitch);

        public void Forward(bool _isForward)
        {
            animator.SetBool(forward_Anim, _isForward);
            AkSoundEngine.PostEvent(_isForward ? doForward_ID : noForward_ID, gameObject);
            AkSoundEngine.SetState(GameManager.Forward_ID, _isForward ? GameManager.DoForward_ID : GameManager.NoForward_ID);
        }

        // -----------------------

        public void OnEndLoop() => LevelManager.Instance.Loop();
        #endregion

        #endregion

        #region Monobehaviour
        private void Awake()
        {
            bounds = player.GetCameraBounds;
            UIManager.Instance.SetCamera(camera);
        }

        private void OnEnable()
        {
            UpdateManager.Instance.Register(this);
        }

        private void OnDisable()
        {
            UpdateManager.Instance.Unregister(this);
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawWireCube(transform.position + new Vector3(0, 0, -transform.position.z), bounds.size);
            }
        }
        #endregion

        #endregion
    }
}
