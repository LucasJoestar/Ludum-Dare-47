// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare47
{
	public class Door : MonoBehaviour, IResetable, IUpdate
    {
		#region Fields / Properties
		public static readonly int Switch_Anim = Animator.StringToHash("Switch");

		[HorizontalLine(1, order = 0), Section("Door", order = 1)]
		[SerializeField] private bool isOpenAtStart = false;
		[SerializeField, ReadOnly] private bool isOpen = false; 
		public bool IsOpen => IsOpen;

		[SerializeField] private DoorOpener[] doorOpeners = new DoorOpener[] { };

        [HorizontalLine(1)]

		[SerializeField, Required] private new Rigidbody2D rigidbody = null;
        [SerializeField, Required] private Vector2 openPosition = new Vector2();
        [SerializeField] private float swichDuration = 1;
        #endregion

        #region Methods
        private bool isSwitching = false;
        private float swichVar = 0;

        private Vector2 closePosition = new Vector2();

        void IUpdate.Update()
        {
            if (isSwitching)
            {
                swichVar += Time.deltaTime;
                if (swichVar >= swichDuration)
                {
                    isSwitching = false;
                    swichVar = swichDuration;
                }

                Vector2 _from, _to;
                if (isOpen)
                {
                    _from = closePosition;
                    _to = openPosition;
                }
                else
                {
                    _to = closePosition;
                    _from = openPosition;
                }

                Vector2 _position = Vector2.Lerp(_from, _to, swichVar / swichDuration);
                rigidbody.position = _position;
                transform.position = _position;
            }
        }

		public void ForceOpenning()
		{
			if (!isOpen)
            {
                isOpen = true;
                Open();
            }
        }
		private void Open()
		{
            if (rigidbody.position != openPosition)
            {
                isSwitching = true;
                swichVar = 0;
            }
        }

		private void Close()
		{
            if (rigidbody.position != closePosition)
            {
                isSwitching = true;
                swichVar = 0;
            }
        }

		public void UpdateOpenningStatus()
		{
			bool _tempOpen = true; 
			for (int i = 0; i < doorOpeners.Length; i++)
			{
				if (!doorOpeners[i].IsActivated)
				{
					_tempOpen = false;
					break;
				}
			}
            if (isOpen != _tempOpen)
            {
                isOpen = _tempOpen;
                if (isOpen)
                    Open();
                else
                    Close();
            }
		}

		public void ResetBehaviour()
		{
			if (isOpen != isOpenAtStart)
			{
				isOpen = isOpenAtStart;

                Vector2 _position = isOpen ? openPosition : closePosition;
                rigidbody.position = _position;
                transform.position = _position;
			}
		}

		private void Start()
		{
			LevelManager.Instance.RegisterResetable(this);
			isOpen = isOpenAtStart;
			for (int i = 0; i < doorOpeners.Length; i++)
			{
				doorOpeners[i].LinkToDoor(this);
			}

            closePosition = transform.position;

            Vector2 _position = isOpen ? openPosition : closePosition;
            rigidbody.position = _position;
            transform.position = _position;
        }

		private void OnDrawGizmos()
		{
			for (int i = 0; i < doorOpeners.Length; i++)
			{
				Gizmos.DrawLine(transform.position, doorOpeners[i].transform.position);
			}
		}

        private void OnEnable()
        {
            UpdateManager.Instance.Register(this);
        }

        private void OnDisable()
        {
            UpdateManager.Instance.Unregister(this);
        }
        #endregion
    }
}
