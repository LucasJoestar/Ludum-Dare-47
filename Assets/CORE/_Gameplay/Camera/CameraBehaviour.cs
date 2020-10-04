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

        [SerializeField, Required] private PlayerController player = null;
        [SerializeField, Required] private Animator animator = null;

        // -----------------------

        [HorizontalLine(1)]

        [SerializeField] private Bounds bounds = new Bounds();

        // -----------------------

        private readonly int loop_Anim = Animator.StringToHash("Loop");
        private readonly int glitch_Anim = Animator.StringToHash("Glitch");
        private readonly int forward_Anim = Animator.StringToHash("IsForward");
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
        public void Loop() => animator.SetTrigger(loop_Anim);

        public void Glitch() => animator.SetTrigger(glitch_Anim);

        public void Forward(bool _isForward) => animator.SetBool(forward_Anim, _isForward);

        // -----------------------

        public void OnEndLoop() => LevelManager.Instance.Loop();
        #endregion

        #endregion

        #region Monobehaviour
        private void Awake()
        {
            bounds = player.GetCameraBounds;
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
