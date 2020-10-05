// ===== Ludum Dare #47 - https://github.com/LucasJoestar/Ludum-Dare-47 ===== //
//
// Notes :
//
// ========================================================================== //

using EnhancedEditor;
using UnityEngine;
using LudumDare47.Geometry; 

namespace LudumDare47.Navigation
{
	public class NavigationAgent : Movable
    {
        #region FieldsAndProperty
        [HorizontalLine(1, order = 0), Section("AGENT SETTINGS", order = 1), Space(order = 2)]
        [SerializeField, ReadOnly] private bool isMoving = false;
        public bool IsMoving { get { return isMoving; } }
        [SerializeField, ReadOnly] private Vector2 velocity = Vector2.zero;
        [Space(order = 1)]
        [SerializeField, Range(.05f, 1)] private float avoidanceForce = .25f;
        [SerializeField, Range(.05f, 1)] private float steerForce = .1f;
        [Space(order = 1)]
        [SerializeField, Range(1.0f, 10.0f)] private float patrolSpeed = 1.5f;
        [SerializeField, Range(1.0f, 10.0f)] private float alertSpeed = 2.5f;

        #region Vector2
        public Vector2 LastPosition
        {
            get
            {
                if (currentPath.Length == 0) return transform.position;
                return currentPath[currentPath.Length - 1];
            }
        }
        #endregion
        public Collider2D Collider => collider;
        public Vector2 InitialPosition => initialPosition; 
        public Quaternion InitialRotation { get; private set; }
        #endregion

        #region Methods

        #region Core Movements
        private int currentIndex = 0;
        private Vector2[] currentPath = new Vector2[] { };
        /// <summary>
        /// Make the agent move along the path and avoid the obstacles
        /// </summary>
        protected override void MovableUpdate()
        {
            if (currentPath == null || currentPath.Length == 0)
            {
                StopAgent();
                return; 
            }
            if (isMoving && (currentIndex > 0))
            {
                Vector2 _previousPosition = currentPath[currentIndex - 1];
                Vector2 _nextPosition = currentPath[currentIndex];

                if (Vector2.Distance(transform.position, LastPosition) < collider.bounds.extents.x)
                {
                    StopAgent();
                }
                else
                {
                    if (Vector2.Distance(transform.position, _nextPosition) <= collider.bounds.extents.x)
                    {
                        //Increasing path index
                        currentIndex++;
                        _previousPosition = currentPath[currentIndex - 1];
                        _nextPosition = currentPath[currentIndex];
                    }
                    /* Get the predicted Velocity and the Predicted position*/
                    Vector2 _predictedPosition = (Vector2)transform.position + velocity.normalized;
                    RaycastHit2D _obstacle;
                    if (CastCollider(_predictedPosition, out _obstacle))
                    {
                        Vector2 _dir = (_obstacle.point - (Vector2)_obstacle.transform.position).normalized;
                        Avoid(_dir);
                    }
                    else
                    {
                        /*Get the transposed Position of the predicted position on the segment between the previous and the next point
                        * The agent has to get closer while it's to far away from the path 
                        */
                        Vector2 _normalPoint = GeometryHelper2D.GetNormalPoint(_predictedPosition, _previousPosition, _nextPosition);

                        /* Direction of the segment between the previous and the next position normalized in order to go further on the path
                         * Targeted position is the normal point + an offset defined by the direction of the segment to go a little further on the path
                         * If the target is out of the segment between the previous and the next position, the target position is the next position
                         */
                        Vector2 _dir = (_nextPosition - _previousPosition).normalized;
                        Vector2 _targetPosition = _normalPoint + _dir;
                        if (!GeometryHelper2D.PointContainedInSegment(_previousPosition, _nextPosition, _targetPosition))
                            _targetPosition = _nextPosition;

                        /* Distance between the predicted position and the normal point on the segment 
                        * If the distance is greater than the radius, it has to steer to get closer
                        */
                        float _distance = Vector2.Distance(_predictedPosition, _normalPoint);
                        float _scalarProduct = Vector2.Dot(velocity, _dir);
                        if (_distance > collider.bounds.extents.x || _scalarProduct == -1 || velocity == Vector2.zero)
                        {
                            Seek(_targetPosition);
                        }
                    }
                    velocity.Normalize();
                    Move(velocity);
                }
            }
            base.MovableUpdate();
        }

        public void ResetAgent()
        {
            SetPosition(initialPosition);
            transform.rotation = InitialRotation;
            RefreshPosition(); 
        }
        #endregion

        #region Setting Destination
        /// <summary>
        /// Check if the destination can be reached
        /// </summary>
        /// <param name="_position">destination to reach</param>
        /// <returns>if the destination can be reached</returns>
        public bool CheckDestination(Vector2 _position)
        {
            if (NavMeshManager.Instance.Triangles == null || NavMeshManager.Instance.Triangles.Count == 0)
            {
                Debug.LogWarning("Triangles Not found. Must build the navmesh for the scene");
                return false;
            }
            bool _canBeReached = PathCalculator.CalculatePath(transform.position, _position, out currentPath, NavMeshManager.Instance.Triangles);
            if (_canBeReached)
            {
                isMoving = true;
                currentIndex = 1;
            }
            return _canBeReached;
        }

        /// <summary>
        /// Calculate a path until reaching a destination
        /// </summary>
        /// <param name="_position">destination to reach</param>
        public void SetDestination(Vector2 _position)
        {
            if (NavMeshManager.Instance.Triangles == null || NavMeshManager.Instance.Triangles.Count == 0)
            {
                Debug.LogWarning("Triangles Not found. Must build the navmesh for the scene");
                return;
            }
            if (PathCalculator.CalculatePath(transform.position, _position, out currentPath, NavMeshManager.Instance.Triangles))
            {
                isMoving = true;
                currentIndex = 1;
            }
        }

        /// <summary>
        /// Stop the agent and reset the path
        /// </summary>
        public virtual void StopAgent()
        {
            isMoving = false;
            currentPath = null;
            currentIndex = 0;
        }
        #endregion

        #region Movements 
        /// <summary>
        /// Apply the avoidance force to the velocity
        /// Avoidance force is equal to the direction from the center position of the obstacle to the hit point of the ray cast
        /// </summary>
        /// <param name="_direction">Direction from the center position of the obstacle to the hit point of the ray cast</param>
        private void Avoid(Vector2 _direction)
        {
            _direction.y = 0;
            _direction.Normalize();
            Vector2 _avoidance = _direction * avoidanceForce;
            velocity += _avoidance;
        }

        /// <summary>
        /// Calculate the needed velocity 
        /// Desired velocity - currentVelocity
        /// </summary>
        /// <param name="_target"></param>
        private void Seek(Vector2 _target)
        {
            Vector2 _desiredVelocity = (_target - (Vector2)transform.position).normalized;
            Vector2 _steer = (_desiredVelocity - velocity) * steerForce;
            velocity += _steer;
        }
        #endregion

        #region Speed 
        public void SetSpeedValue(bool _isInPatrol) => speed = _isInPatrol ? patrolSpeed : alertSpeed;
        #endregion 

        #endregion

        #region UnityMethods
        protected override void Start()
        {
            base.Start();
            InitialRotation = transform.rotation;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if(collider)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(collider.bounds.center, .1f);
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(collider.bounds.center + new Vector3(velocity.x, velocity.y), .1f);
                Gizmos.DrawLine(collider.bounds.center, collider.bounds.center + new Vector3(velocity.x, velocity.y));
            }
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, .1f);

            if (currentPath == null || currentPath.Length == 0) return;
            Gizmos.color = Color.cyan;
            for (int i = 0; i < currentPath.Length; i++)
            {
                Gizmos.DrawSphere(currentPath[i], .2f);
            }
            for (int i = 0; i < currentPath.Length - 1; i++)
            {
                Gizmos.DrawLine(currentPath[i], currentPath[i + 1]);
            }
        }

#endif
        #endregion
    }
}
