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
        bool Interact();
        void Hack();
        void Die();
    }

    public class PlayerController : Movable, IPlayerBehaviour, IInputUpdate
    {
        #region Fields / Properties
		[HorizontalLine(1, order = 0), Section("PLAYER CONTROLLER", order = 1)]

        [SerializeField, Required] private PlayerAttributes attributes = null;
        [SerializeField, Required] private PlayerInputs inputs = null;
        [SerializeField, Required] private Animator animator = null;

        // -----------------------

        [SerializeField, ReadOnly] private bool isMoving = false;
        [SerializeField, ReadOnly] private bool isPlayable = true;

        // -----------------------

        [SerializeField, ReadOnly] private List<IPlayerGhostState> ghostStates = new List<IPlayerGhostState>();

        // -----------------------

        [HorizontalLine(1, order = 0)]

        [ReadOnly, HelpBox("Last movement of the player on the X axis", HelpBoxType.Info, order = 1)]
        [SerializeField] private Vector2 lastMovement = Vector2.zero;

        // -----------------------

        public static readonly int Moving_Anim = Animator.StringToHash("IsMoving");
        public static readonly int Hack_Anim = Animator.StringToHash("Hack");
        public static readonly int Die_Anim = Animator.StringToHash("Die");
        #endregion

        #region Methods

        #region Input Management
        void IInputUpdate.Update()
        {
            if (isPlayable)
            {
                Move(inputs.Move.ReadValue<Vector2>());

                // Register current state.
                if (inputs.Action.triggered)
                    Interact();
            }
        }

        // -----------------------

        private void EnableInputs()
        {
            inputs.Move.Enable();
            inputs.Action.Enable();
        }

        private void DisableInputs()
        {
            inputs.Move.Disable();
            inputs.Action.Disable();
        }
        #endregion

        #region Actions
        /// <summary>
        /// Interact with whatever is close to the ghost.
        /// </summary>
        public bool Interact()
        {
            if (collider.OverlapCollider(contactFilter, overlapColliders) > 0)
                return overlapColliders[0].GetComponent<IInteractable>().Interact(this);

            return false;
        }

        public void Hack()
        {
            // Set animation and other things.
            animator.SetBool(Hack_Anim, isMoving);
        }

        public void Die()
        {
            // Set animation and die.
            animator.SetBool(Die_Anim, isMoving);
        }
        #endregion

        #region State
        /// <summary>
        /// Completely reset behaviour and restart the loop.
        /// </summary>
        public PlayerGhost OnStartLoop(Vector2 _position)
        {
            PlayerGhost _ghost = Instantiate(attributes.ghostPrefab, transform.position, transform.rotation);
            _ghost.Init(ghostStates);

            ghostStates.Clear();
            ghostStates.Add(new PlayerGhostStopState(0, rigidbody.position));

            isPlayable = true;

            return _ghost;
        }

        public void OnEndLoop()
        {
            ghostStates.Add(new PlayerGhostStopState(LevelManager.Instance.LoopTime, rigidbody.position));
            isPlayable = true;
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
            }

            // Reset movement if not moving.
            if (!_isMoving && (speed > 0))
                ResetMovement();
        }
        #endregion

        #region Monobehaviour
        protected override void OnEnable()
        {
            base.OnEnable();

            EnableInputs();
            UpdateManager.Instance.Register((IInputUpdate)this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            DisableInputs();
            UpdateManager.Instance.Unregister((IInputUpdate)this);
        }
        #endregion

        #endregion
    }
}
