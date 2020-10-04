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
    public interface IPlayerBehaviour
    {
        void Hack(Hackable _hackable);
        void Die();
    }

    public class PlayerController : Movable, IPlayerBehaviour, IInputUpdate
    {
        #region Fields / Properties
		[HorizontalLine(1, order = 0), Section("PLAYER CONTROLLER", order = 1)]

        [SerializeField, Required] private PlayerAttributes attributes = null;
        [SerializeField, Required] private PlayerInputs inputs = null;
        [SerializeField, Required] private Animator animator = null;

        public Bounds GetCameraBounds => attributes.cameraBounds;

        // -----------------------

        [SerializeField, ReadOnly] private bool isMoving = false;
        [SerializeField, ReadOnly] private bool isInDialog = false;
        [SerializeField, ReadOnly] private bool isHacking = false;
        [SerializeField, ReadOnly] private bool isPlayable = true;

        public bool IsPaused = false;

        // -----------------------

        [SerializeField, ReadOnly] private Hackable hacking = null;
        [SerializeField, ReadOnly] private List<IPlayerGhostState> ghostStates = new List<IPlayerGhostState>();

        // -----------------------

        [HorizontalLine(1, order = 0)]

        [HelpBox("Last movement of the player on the X axis", HelpBoxType.Info, order = 1)]
        [SerializeField, ReadOnly] private Vector2 lastMovement = Vector2.zero;

        // -----------------------

        public static readonly int Moving_Anim = Animator.StringToHash("IsMoving");
        public static readonly int Hack_Anim = Animator.StringToHash("Hack");
        public static readonly int Die_Anim = Animator.StringToHash("Die");
        #endregion

        #region Methods

        #region Input Management
        void IInputUpdate.Update()
        {
            if (IsPaused)
                return;

            // Dialog update.
            if (isInDialog && inputs.Action.triggered)
                LevelManager.Instance.EndDialog();

            if (isPlayable)
            {
                // Hacking update.
                if (isHacking)
                {
                    if (hacking.UpdateHack())
                        isHacking = false;
                }
                else
                {
                    Move(inputs.Move.ReadValue<Vector2>());

                    // Register current state.
                    if (inputs.Action.triggered &&
                       (collider.OverlapCollider(contactFilter, overlapColliders) > 0) &&
                        overlapColliders[0].GetComponent<IInteractable>().Interact(this))
                    {
                        ghostStates.Add(new PlayerGhostInteractState(LevelManager.Instance.LoopTime));
                    }
                }
            }
            else if (inputs.Loop.triggered)
            {
                LevelManager.Instance.StartLoop();
                return;
            }

            // Reset loop on button trigger.
            if (inputs.ResetLoop.triggered)
                LevelManager.Instance.ResetLoop();
        }

        // -----------------------

        private void EnableInputs()
        {
            inputs.Move.Enable();
            inputs.Action.Enable();

            inputs.Loop.Enable();
            inputs.ResetLoop.Enable();
        }

        private void DisableInputs()
        {
            inputs.Move.Disable();
            inputs.Action.Disable();

            inputs.Loop.Disable();
            inputs.ResetLoop.Disable();
        }
        #endregion

        #region Actions
        public void Hack(Hackable _hackable)
        {
            isHacking = true;
            hacking = _hackable;

            animator.SetBool(Hack_Anim, true);
        }

        public void Die()
        {
            // Set animation and die.
            OnEndLoop();
            animator.SetTrigger(Die_Anim);
        }
        #endregion

        #region State
        public void SetInDialog(bool _isInDialog) => isInDialog = _isInDialog;

        // -----------------------

        /// <summary>
        /// Completely reset behaviour and restart the loop.
        /// </summary>
        public PlayerGhost OnStartLoop(Vector2 _position)
        {
            PlayerGhost _ghost = Instantiate(attributes.ghostPrefab, transform.position, transform.rotation);
            _ghost.Init(ghostStates);

            SetPosition(_position);
            transform.rotation = Quaternion.identity;
            rigidbody.rotation = 0;

            ghostStates.Clear();
            ghostStates.Add(new PlayerGhostStopState(0, rigidbody.position));

            isPlayable = true;
            return _ghost;
        }

        public void OnEndLoop()
        {
            if (isPlayable)
            {
                isPlayable = false;
                ResetMovement();
                ghostStates.Add(new PlayerGhostStopState(LevelManager.Instance.LoopTime, rigidbody.position));

                if (isHacking)
                {
                    isHacking = false;
                    hacking.CancelHack();
                }
            }
        }
        #endregion

        #region Velocity
        private float speedCurveVarTime = 0;

        protected override void Move(Vector2 _movement)
        {
            if (!_movement.IsNull())
            {
                // Increase speed and modify movement value
                AnimationCurve _speedCurve = attributes.SpeedCurve;
                if (speed < _speedCurve[_speedCurve.length - 1].value)
                {
                    speed = _speedCurve.Evaluate(speedCurveVarTime);
                    speedCurveVarTime = Mathf.Min(speedCurveVarTime + GameManager.DeltaTime, _speedCurve[_speedCurve.length - 1].time);
                }

                base.Move(_movement);
            }
        }

        // -----------------------

        private void ResetMovement()
        {
            speed = speedCurveVarTime = 0;

            movement.Set(0, 0);
            lastMovement.Set(0, 0);

            if (isMoving)
            {
                isMoving = false;
                animator.SetBool(Moving_Anim, isMoving);

                OnStopMoving();
            }
        }

        // -----------------------

        private bool isGoingXOppositeSide = false;
        private bool isGoingYOppositeSide = false;

        protected override void ComputeVelocityBeforeMovement()
        {
            // When going in opposite direction from previous movement, transit to it.
            if (isGoingXOppositeSide || ((lastMovement.x != 0) && !Mathm.HaveDifferentSign(lastMovement.x - movement.x, lastMovement.x)))
            {
                isGoingXOppositeSide = true;

                // About-Turn.
                if ((movement.x != 0) && Mathm.HaveDifferentSign(lastMovement.x, movement.x))
                {
                    movement.x = Mathf.MoveTowards(lastMovement.x, movement.x, speed * GameManager.DeltaTime * attributes.AboutTurnAccel);
                }
                // Deceleration.
                else if (lastMovement.x != movement.x)
                {
                    movement.x = Mathf.MoveTowards(lastMovement.x, movement.x, GameManager.DeltaTime * attributes.MovementDecel);
                }
                else
                    isGoingXOppositeSide = false;
            }
            if (isGoingYOppositeSide || ((lastMovement.y != 0) && !Mathm.HaveDifferentSign(lastMovement.y - movement.y, lastMovement.y)))
            {
                isGoingYOppositeSide = true;

                // About-Turn.
                if ((movement.y != 0) && Mathm.HaveDifferentSign(lastMovement.y, movement.y))
                {
                    movement.y = Mathf.MoveTowards(lastMovement.y, movement.y, speed * GameManager.DeltaTime * attributes.AboutTurnAccel);
                }
                // Deceleration.
                else if (lastMovement.y != movement.y)
                {
                    movement.y = Mathf.MoveTowards(lastMovement.y, movement.y, GameManager.DeltaTime * attributes.MovementDecel);
                }
                else
                    isGoingYOppositeSide = false;
            }

            lastMovement.Set(movement.x, movement.y);
            base.ComputeVelocityBeforeMovement();
        }
        #endregion

        #region Core Movements
        private float moveStartTime = 0;
        private Vector2 moveStartPosition = new Vector2();

        // -----------------------

        /// <summary>
        /// Called after velocity has been applied.
        /// </summary>
        protected override void OnAppliedVelocity(Vector2 _movement)
        {
            base.OnAppliedVelocity(_movement);

            // Player moving state.
            bool _isMoving = ((lastMovement.x != 0) && (Mathf.Abs(_movement.x) > .0001f)) ||
                             ((lastMovement.y != 0) && (Mathf.Abs(_movement.y) > .0001f));

            if (isMoving != _isMoving)
            {
                isMoving = _isMoving;
                animator.SetBool(Moving_Anim, isMoving);

                // Register ghost movement.
                if (_isMoving)
                {
                    moveStartTime = LevelManager.Instance.LoopTime;
                    moveStartPosition = rigidbody.position;
                }
                else
                    OnStopMoving();
            }

            // Reset movement if not moving.
            if (_isMoving)
            {
                float _movingTime = LevelManager.Instance.LoopTime - moveStartTime - attributes.ghostMovementRegistration;
                if (_movingTime >= 0)
                {
                    _movement = (rigidbody.position - moveStartPosition) / attributes.ghostMovementRegistration;
                    ghostStates.Add(new PlayerGhostMoveState(moveStartTime, _movement));

                    moveStartTime = LevelManager.Instance.LoopTime - _movingTime;
                    moveStartPosition = rigidbody.position;
                    ghostStates.Add(new PlayerGhostSetPositionState(moveStartTime, moveStartPosition));
                }
            }
            else if (speed > 0)
                ResetMovement();
        }

        private void OnStopMoving()
        {
            Vector2 _movement = (rigidbody.position - moveStartPosition) / (LevelManager.Instance.LoopTime - moveStartTime);
            ghostStates.Add(new PlayerGhostMoveState(moveStartTime, _movement));
            ghostStates.Add(new PlayerGhostStopState(LevelManager.Instance.LoopTime, rigidbody.position));
        }
        #endregion

        #region Monobehaviour
        protected override void OnEnable()
        {
            base.OnEnable();

            UpdateManager.Instance.Register((IInputUpdate)this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            DisableInputs();
            UpdateManager.Instance.Unregister((IInputUpdate)this);
        }

        protected override void Start()
        {
            base.Start();

            EnableInputs();
            ghostStates.Add(new PlayerGhostStopState(0, rigidbody.position));
        }
        #endregion

        #endregion
    }
}
