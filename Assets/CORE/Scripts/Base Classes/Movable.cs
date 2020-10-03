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
    [RequireComponent(typeof(Rigidbody2D))]
	public abstract class Movable : MonoBehaviour, IMovableUpdate
    {
        #region Fields / Properties
        protected const float CAST_MAX_DISTANCE_DETECTION = .001f;
        protected const int MAX_COLLISION_CALCULS_RECURSIVITY = 3;

        // -----------------------

        [HorizontalLine(1, order = 0), Section("MOVABLE", order = 1)]

        [SerializeField, Required] protected new Collider2D collider = null;
        [SerializeField, Required] protected new Rigidbody2D rigidbody = null;

        [Space]

        [SerializeField,] protected SpriteRenderer[] sprites = new SpriteRenderer[] { };

        [HorizontalLine(1)]

        [SerializeField] protected float speed = 1;

        // -----------------------

        [SerializeField] protected Vector2 movement = Vector2.zero;

        #if UNITY_EDITOR
        [HorizontalLine(1, order = 0)]

        [HelpBox("Previous position is used for debug and refreshing object after moving it in scene editor", HelpBoxType.Warning, order = 1)]
        [SerializeField, ReadOnly] protected Vector2 lastPosition = new Vector2();
        #endif

        // -----------------------

        protected ContactFilter2D contactFilter = new ContactFilter2D();
        #endregion

        #region Methods

        #region Velocity
        protected virtual void Move(Vector2 _movement)
        {
            // Rotate towards moving direction.
            movement += _movement * speed;
        }

        // -----------------------

        /// <summary>
        /// Get the object velocity movement for this frame.
        /// </summary>
        protected virtual Vector2 GetVelocity() => movement * GameManager.DeltaTime;

        // -----------------------

        protected virtual void ComputeVelocityBeforeMovement() { }
        #endregion

        #region Core Movements
        protected bool shouldBeRefreshed = false;

        /// <summary>
        /// Set this object position.
        /// Use this instead of setting <see cref="Transform.position"/>.
        /// </summary>
        public void SetPosition(Vector2 _position)
        {
            rigidbody.position = _position;
            shouldBeRefreshed = true;
        }

        // -----------------------

        void IMovableUpdate.Update() => MovableUpdate();

        protected virtual void MovableUpdate()
        {
            #if UNITY_EDITOR
            // Get if object moved in editor.
            if (lastPosition != rigidbody.position)
                shouldBeRefreshed = true;
            #endif

            // Refresh position before collision calculs if needed.
            if (shouldBeRefreshed)
            {
                RefreshPosition();
                shouldBeRefreshed = false;
            }

            // Apply velocity and move the object.
            ComputeVelocityBeforeMovement();
            if (!movement.IsNull())
            {
                Vector2 _lastPosition = rigidbody.position;

                CalculateCollisions();
                RefreshPosition();

                OnAppliedVelocity(rigidbody.position - _lastPosition);
            }
            else
                OnAppliedVelocity(Vector3.zero);

            #if UNITY_EDITOR
            lastPosition = rigidbody.position;
            #endif
        }

        // -----------------------

        protected static readonly Collider2D[] overlapColliders = new Collider2D[4];
        protected static readonly Trigger[] overlapTriggers = new Trigger[4];
        protected List<Trigger> remainingTriggers = new List<Trigger>();

        private void RefreshPosition()
        {
            // Extract collider from potential collisions.
            int _overlapAmount = collider.OverlapCollider(contactFilter, overlapColliders);

            // For each overlapping colliders, detect triggers and extract from collision ones.
            if (_overlapAmount > 0)
            {
                ColliderDistance2D _distance;
                int _triggerAmount = 0;

                for (int _i = 0; _i < _overlapAmount; _i++)
                {
                    // Trigger detection and activation.
                    if (overlapColliders[_i].isTrigger)
                    {
                        if (overlapColliders[_i].TryGetComponent(out Trigger _trigger))
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
                    else
                    {
                        // If overlap, extract from collision.
                        _distance = collider.Distance(overlapColliders[_i]);

                        if (_distance.isOverlapped)
                            rigidbody.position += _distance.normal * _distance.distance;
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

        // -----------------------

        /// <summary>
        /// Called after velocity has been applied.
        /// </summary>
        protected virtual void OnAppliedVelocity(Vector2 _movement) { }
        #endregion

        #region Collision Calculs
        protected static int collisionBufferCount = 0;
        protected static readonly RaycastHit2D[] collisionBuffer = new RaycastHit2D[16];
        protected static readonly RaycastHit2D[] castBuffer = new RaycastHit2D[16];

        /// <summary>
        /// Calculate the object collisions according to its velocity and move the rigidbody.
        /// Override this to implement a custom collision detection system.
        /// </summary>
        private void CalculateCollisions()
        {
            // Calculate collisions recursively.
            collisionBufferCount = 0;
            CalculateCollisionsRecursively(GetVelocity());

            // Reset movement after calculs.
            movement = Vector3.zero;
        }

        private void CalculateCollisionsRecursively(Vector2 _velocity, int _recursivityCount = 1)
        {
            int _amount = CastCollider(_velocity, out float _distance);

            // No movement mean object is stuck into something, so return.
            if (_distance == 0)
                return;

            if (_amount == 0)
            {
                rigidbody.position += _velocity;
                return;
            }

            // Move rigidbody and get extra cast velocity.
            if ((_distance -= Physics.defaultContactOffset) > 0)
            {
                Vector2 _normalizedVelocity = _velocity.normalized;

                rigidbody.position += _normalizedVelocity * _distance;
                _velocity = _normalizedVelocity * (_velocity.magnitude - _distance);
            }

            // Add as many hits as possible while there is enough space,
            // or replace the last one if the buffer is already full.
            if (collisionBufferCount < collisionBuffer.Length)
            {
                for (int _i = 0; _i < _amount; _i++)
                {
                    collisionBuffer[collisionBufferCount] = castBuffer[_i];
                    collisionBufferCount++;

                    if (collisionBufferCount == collisionBuffer.Length)
                        return;
                }
            }
            else
                collisionBuffer[collisionBufferCount - 1] = castBuffer[0];

            // If reached recursion limit, stop calculs.
            if (_recursivityCount > MAX_COLLISION_CALCULS_RECURSIVITY)
                return;

            // Reduce extra movement according to main impact normals.
            _velocity -= castBuffer[0].normal * Vector2.Dot(_velocity, castBuffer[0].normal);
            if (!_velocity.IsNull())
            {
                CalculateCollisionsRecursively(_velocity, _recursivityCount + 1);
            }
        }
        #endregion

        #region Collider Operations
        protected bool CastCollider(Vector2 _movement, out RaycastHit2D _hit)
        {
            bool _result = collider.Cast(_movement, contactFilter, castBuffer, _movement.magnitude) > 0;

            _hit = castBuffer[0];
            return _result;
        }

        protected int CastCollider(Vector2 _movement, out float _distance)
        {
            _distance = _movement.magnitude;

            int _hitAmount = collider.Cast(_movement, contactFilter, castBuffer, _distance);
            if (_hitAmount > 0)
            {
                // Hits are already sorted by distance, so simply get closest one.
                _distance = Mathf.Max(0, castBuffer[0].distance - Physics2D.defaultContactOffset);

                // Retains only closest hits by ignoring those with too distants.
                for (int _i = 1; _i < _hitAmount; _i++)
                {
                    if ((castBuffer[_i].distance + CAST_MAX_DISTANCE_DETECTION) > castBuffer[0].distance)
                        return _i;
                }
            }
            return _hitAmount;
        }
        #endregion

        #region Monobehaviour
        protected virtual void OnEnable()
        {
            UpdateManager.Instance.Register(this);
        }

        protected virtual void OnDisable()
        {
            UpdateManager.Instance.Unregister(this);
        }

        protected virtual void Start()
        {
            #if UNITY_EDITOR
            lastPosition = transform.position;
            #endif

            // Initialize object contact filter.
            contactFilter.layerMask = Physics2D.GetLayerCollisionMask(gameObject.layer);
            contactFilter.useLayerMask = true;
        }
        #endregion

        #endregion
    }
}
