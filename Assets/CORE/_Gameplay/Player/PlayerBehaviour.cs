// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare47
{
	public abstract class PlayerBehaviour : Movable, IMovableUpdate
    {
        #region Fields / Properties
		[HorizontalLine(1, order = 0), Section("PLAYER BEHAVIOUR", order = 1)]

        [SerializeField, Required] private PlayerAttributes attributes = null;
        [SerializeField, Required] private Animator animator = null;

        // -----------------------

        [SerializeField, ReadOnly] protected bool isMoving = false;

        // -----------------------

        [HorizontalLine(1, order = 0)]

        [ReadOnly, HelpBox("Last movement of the player on the X axis", HelpBoxType.Info, order = 1)]
        [SerializeField] private Vector2 lastMovement = Vector2.zero;

        // -----------------------

        private readonly int anim_Moving = Animator.StringToHash("IsMoving");
        #endregion

        #region Methods

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
                animator.SetBool(anim_Moving, isMoving);
            }

            // Reset movement if not moving.
            if (!_isMoving && (speed > 0))
                ResetMovement();
        }
        #endregion

        #endregion
    }
}
