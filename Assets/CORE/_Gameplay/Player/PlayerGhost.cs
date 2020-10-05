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
    #region Ghost States
    public interface IPlayerGhostState
    {
        bool HasTimeCome();
        bool DoUpdateContinuously();
        void UpdateState(PlayerGhost _ghost);
    }

    // -----------------------
    
    public struct PlayerGhostMoveState : IPlayerGhostState
    {
        private readonly float time;
        private readonly Vector2 movement;

        // -----------------------

        public PlayerGhostMoveState(float _time, Vector2 _movement)
        {
            time = _time;
            movement = _movement;
        }

        // -----------------------

        public bool HasTimeCome() => LevelManager.Instance.LoopTime >= time;

        public bool DoUpdateContinuously() => true;

        public void UpdateState(PlayerGhost _ghost) => _ghost.Move(movement * GameManager.DeltaTime);
    }

    public struct PlayerGhostSetPositionState : IPlayerGhostState
    {
        private readonly float time;
        private readonly Vector2 position;

        // -----------------------

        public PlayerGhostSetPositionState(float _time, Vector2 _position)
        {
            time = _time;
            position = _position;
        }

        // -----------------------

        public bool HasTimeCome() => LevelManager.Instance.LoopTime >= time;

        public bool DoUpdateContinuously() => true;

        public void UpdateState(PlayerGhost _ghost) => _ghost.SetPosition(position);
    }

    public struct PlayerGhostStopState : IPlayerGhostState
    {
        private readonly float time;
        private readonly Vector2 position;

        // -----------------------

        public PlayerGhostStopState(float _time, Vector2 _position)
        {
            time = _time;
            position = _position;
        }

        // -----------------------

        public bool HasTimeCome() => LevelManager.Instance.LoopTime >= time;

        public bool DoUpdateContinuously() => false;

        public void UpdateState(PlayerGhost _ghost) => _ghost.Stop(position);
    }
    
    public struct PlayerGhostInteractState : IPlayerGhostState
    {
        private readonly float time;

        // -----------------------

        public PlayerGhostInteractState(float _time)
        {
            time = _time;
        }

        // -----------------------

        public bool HasTimeCome() => LevelManager.Instance.LoopTime >= time;

        public bool DoUpdateContinuously() => false;

        public void UpdateState(PlayerGhost _ghost) => _ghost.Interact();
    }
    #endregion

    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerGhost : MonoBehaviour, IPlayerBehaviour
    {
        #region Fields / Properties
        [HorizontalLine(1, order = 0), Section("PLAYER GHOST", order = 1)]

        [SerializeField, Required] private new Collider2D collider = null;
        [SerializeField, Required] private new Rigidbody2D rigidbody = null;
        [SerializeField, Required] private Animator animator = null;

        [Space]

        [SerializeField,] protected SpriteRenderer[] sprites = new SpriteRenderer[] { };

        // -----------------------

        [HorizontalLine(1)]

        [SerializeField, ReadOnly] private bool isMoving = false;
        [SerializeField, ReadOnly] private bool isHacking = false;
        [SerializeField, ReadOnly] private bool isLoopCompleted = false;
        [SerializeField, ReadOnly] private bool isStateContinouslyUpdating = false;

        [SerializeField, ReadOnly] private Hackable hacking = null;

        [HorizontalLine(1)]

        [SerializeField, ReadOnly] private int stateIndex = -1;
        [SerializeField, ReadOnly] private IPlayerGhostState[] states = new IPlayerGhostState[] { };

        // -----------------------

        private ContactFilter2D contactFilter = new ContactFilter2D();
        #endregion

        #region Methods

        #region Actions
        private bool isParent = false;
        private Transform parent = null;

        public void Parent(Transform _parent)
        {
            isParent = true;
            parent = _parent;
            OnEndLoop();
        }

        public void Unparent()
        {
            if (isParent)
            {
                isParent = false;
                parent = null;
            }
        }

        // -----------------------

        private ContactFilter2D interactFilter = new ContactFilter2D();

        /// <summary>
        /// Interact with whatever is close to the ghost.
        /// </summary>
        public bool Interact()
        {
            if (collider.OverlapCollider(interactFilter, overlapColliders) > 0)
                return overlapColliders[0].GetComponentInParent<IInteractable>().Interact(this);

            return false;
        }

        public void Hack(Hackable _hackable)
        {
            isHacking = true;
            hacking = _hackable;

            animator.SetBool(PlayerController.Hack_Anim, true);
        }

        public void Die()
        {
            // Set animation and die.
            animator.SetTrigger(PlayerController.Die_Anim);
            OnEndLoop();
        }

        private void OnEndLoop()
        {
            isLoopCompleted = true;
            collider.enabled = false;

            if (isHacking)
            {
                isHacking = false;
                animator.SetBool(PlayerController.Hack_Anim, false);

                hacking.CancelHack();
            }
        }
        #endregion

        #region State
        public void Init(List<IPlayerGhostState> _states, ContactFilter2D _filter)
        {
            states = _states.ToArray();
            interactFilter = _filter;
        }

        // -----------------------

        public void ResetBehaviour(Vector2 _position)
        {
            // Reset animator.
            animator.enabled = false;
            animator.enabled = true;

            if (isHacking)
            {
                isHacking = false;
                animator.SetBool(PlayerController.Hack_Anim, false);

                hacking.CancelHack();
            }

            collider.enabled = true;
            SetPosition(_position);
            transform.rotation = Quaternion.identity;
            rigidbody.rotation = 0;

            stateIndex = -1;
            isLoopCompleted = false;
            isStateContinouslyUpdating = false;
        }
        #endregion

        #region Core Movements
        public void PlayFootsteps() => AkSoundEngine.PostEvent(Movable.Foosteps_ID, gameObject);

        /// <summary>
        /// Move the ghost with a certain movement.
        /// </summary>
        public void Move(Vector2 _movement)
        {
            if (!isMoving)
            {
                isMoving = true;
                animator.SetBool(PlayerController.Moving_Anim, true);
            }

            // Rotate towards moving direction.
            float _absAngle = Vector2.SignedAngle(transform.up, _movement);
            if (_absAngle != 0)
            {
                float _angle = Mathf.Min(ProgramSettings.I.RotationSpeed * GameManager.DeltaTime, Mathf.Abs(_absAngle));

                transform.Rotate(Vector3.forward, _angle * Mathf.Sign(_absAngle));
                rigidbody.rotation = transform.rotation.eulerAngles.z;
            }

            SetPosition(rigidbody.position + _movement);
        }

        /// <summary>
        /// Stop movement and set position.
        /// </summary>
        public void Stop(Vector2 _position)
        {
            if (isMoving)
            {
                isMoving = false;
                animator.SetBool(PlayerController.Moving_Anim, false);
            }

            SetPosition(_position);
        }

        public void SetPosition(Vector2 _position)
        {
            rigidbody.position = _position;
            transform.position = _position;

            RefreshPosition();
        }

        // -----------------------

        public void MovableUpdate()
        {
            // Parent update position.
            if (isParent)
            {
                Vector2 _position = parent.position;
                Vector2 _movement = _position - (Vector2)transform.position;

                if (!_movement.IsNull())
                    Move(_movement);
                else if (isMoving)
                    Stop(_position);
            }

            // Hacking update.
            if (isHacking)
            {
                if (hacking.UpdateHack())
                {
                    isHacking = false;
                    animator.SetBool(PlayerController.Hack_Anim, false);
                }
            }

            // Update state while not reached loop end.
            if (!isLoopCompleted)
            {
                if (isStateContinouslyUpdating)
                {
                    states[stateIndex].UpdateState(this);
                }
                while (states[stateIndex + 1].HasTimeCome())
                {
                    stateIndex++;
                    states[stateIndex].UpdateState(this);

                    if (stateIndex < states.Length - 1)
                    {
                        isStateContinouslyUpdating = states[stateIndex].DoUpdateContinuously();
                    }
                    else
                    {
                        isLoopCompleted = true;
                        break;
                    }
                }
            }
        }

        // -----------------------

        protected static readonly Collider2D[] overlapColliders = new Collider2D[4];
        protected static readonly Trigger[] overlapTriggers = new Trigger[4];
        protected List<Trigger> remainingTriggers = new List<Trigger>();

        private void RefreshPosition()
        {
            // Extract collider from potential collisions.
            contactFilter.useTriggers = true;
            int _overlapAmount = collider.OverlapCollider(contactFilter, overlapColliders);

            // For each overlapping colliders, detect triggers and extract from collision ones.
            if (_overlapAmount > 0)
            {
                int _triggerAmount = 0;
                for (int _i = 0; _i < _overlapAmount; _i++)
                {
                    // Trigger detection and activation.
                    if (overlapColliders[_i].isTrigger && overlapColliders[_i].TryGetComponent(out Trigger _trigger))
                    {
                        overlapTriggers[_triggerAmount] = _trigger;
                        _triggerAmount++;

                        if (DoEnterTrigger(_trigger))
                        {
                            _trigger.OnEnter(gameObject);
                            remainingTriggers.Add(_trigger);
                        }
                    }
                }

                // Remove no more overlapping triggers.
                for (int _i = 0; _i < remainingTriggers.Count; _i++)
                {
                    if (HasExitedTrigger(remainingTriggers[_i], _triggerAmount))
                    {
                        remainingTriggers[_i].OnExit(gameObject);
                        remainingTriggers.RemoveAt(_i);
                        _i--;
                    }
                }
            }

            // Update sprites order.
            Vector3 _position = rigidbody.position;
            if (transform.position.y != _position.y)
                SpriteUtility.Order(sprites);

            // Finally, move the object transform.
            contactFilter.useTriggers = false;
            transform.position = _position;

            // ----- Local Methods ----- //
            bool DoEnterTrigger(Trigger _trigger)
            {
                for (int _i = 0; _i < remainingTriggers.Count; _i++)
                {
                    if (_trigger.Compare(remainingTriggers[_i]))
                        return false;
                }
                return true;
            }

            bool HasExitedTrigger(Trigger _trigger, int _amount)
            {
                for (int _i = 0; _i < _amount; _i++)
                {
                    if (_trigger.Compare(overlapTriggers[_i]))
                        return false;
                }
                return true;
            }
        }
        #endregion

        #region Monobehaviour
        protected virtual void Start()
        {
            // Initialize object contact filter.
            contactFilter.layerMask = Physics2D.GetLayerCollisionMask(gameObject.layer);
            contactFilter.useLayerMask = true;
        }
        #endregion

        #endregion
    }
}
