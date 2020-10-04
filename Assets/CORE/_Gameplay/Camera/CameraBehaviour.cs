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

        // -----------------------

        [HorizontalLine(1)]

        [SerializeField] private Bounds bounds = new Bounds();
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
