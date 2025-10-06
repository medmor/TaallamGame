using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace TaallamGame.NPCs
{
    /// <summary>
    /// NavMesh-based AI for NPC patrol behavior.
    /// NPC will patrol between waypoints, pause at each point, and handle obstacles.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class PatrolAINavMesh : MonoBehaviour
    {
        [Header("Patrol Settings")]
        [SerializeField] private Transform[] waypoints;
        [SerializeField] private float waitTime = 2f; // Time to wait at each waypoint
        [SerializeField] private float patrolSpeed = 3f;
        [SerializeField] private bool loopPatrol = true;
        [SerializeField] private bool randomizeWaypoints = false;

        [Header("Detection")]
        [SerializeField] private float detectionRange = 5f;
        [SerializeField] private LayerMask playerLayer = 1;
        [SerializeField] private Transform player; // Optional: assign player reference
        [SerializeField] private bool stopOnPlayerDetection = true;
        [SerializeField] private bool resumeOnPlayerLost = true;
        [SerializeField] private float resumeDelay = 2f;

        [Header("Behavior")]
        [SerializeField] private float stoppingDistance = 0.5f;
        [SerializeField] private bool pauseOnPlayerDetection = true;
        [SerializeField] private float playerDetectionPause = 3f;

        [Header("Animation")]
        [SerializeField] private Animator animator;

        [Header("NavMesh Agent")]
        [Tooltip("If enabled the NavMeshAgent will not update rotation (useful for 2D sprites)")]


        private NavMeshAgent agent;
        private int currentWaypointIndex = 0;
        private bool isPatrolling = true;
        private bool playerDetected = false;
        private Coroutine patrolCoroutine;
        private enum Facing { Up, Down, Left, Right }
        private Facing lastFacing = Facing.Down;

        // Animation parameter names
        private const string IS_MOVING = "IsMoving";
        private const string MOVING_LEFT = "MovingLeft";
        private const string MOVING_RIGHT = "MovingRight";
        private const string MOVING_UP = "MovingUp";
        private const string MOVING_DOWN = "MovingDown";
        private const string IDLE_LEFT = "IdleLeft";
        private const string IDLE_RIGHT = "IdleRight";
        private const string IDLE_UP = "IdleUp";
        private const string IDLE_DOWN = "IdleDown";

        public enum PatrolState
        {
            Patrolling,
            Waiting,
            PlayerDetected,
            Stopped
        }

        public PatrolState CurrentState { get; private set; } = PatrolState.Patrolling;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();


            // Configure NavMeshAgent for 2D based on inspector toggles
            agent.updateRotation = false; // if true, agent won't update rotation
            agent.updateUpAxis = false;    // if true, agent won't modify up axis

            agent.speed = patrolSpeed;
            agent.stoppingDistance = stoppingDistance;
        }

        private void Start()
        {
            // Auto-find player if not assigned
            if (player == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                    player = playerObj.transform;
            }

            // Validate waypoints
            if (waypoints == null || waypoints.Length == 0)
            {
                waypoints = new Transform[] { transform };
            }

            // Start patrolling
            if (isPatrolling)
            {
                StartPatrol();
            }
        }

        private void Update()
        {
            HandlePlayerDetection();
            UpdateAnimation();
        }

        public void StartPatrol()
        {
            if (patrolCoroutine != null)
                StopCoroutine(patrolCoroutine);

            isPatrolling = true;
            patrolCoroutine = StartCoroutine(PatrolRoutine());
        }

        public void StopPatrol()
        {
            if (patrolCoroutine != null)
                StopCoroutine(patrolCoroutine);

            isPatrolling = false;
            agent.ResetPath();
            CurrentState = PatrolState.Stopped;
        }

        public void PausePatrol(float duration = -1f)
        {
            if (patrolCoroutine != null)
                StopCoroutine(patrolCoroutine);

            // Mark not patrolling so resume logic can restart patrol later
            isPatrolling = false;
            agent.ResetPath();
            CurrentState = PatrolState.Waiting;

            if (duration > 0)
            {
                StartCoroutine(ResumePatrolAfterDelay(duration));
            }
        }

        private IEnumerator ResumePatrolAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (!isPatrolling) // Only resume if we're still supposed to be patrolling
                StartPatrol();
        }

        private IEnumerator PatrolRoutine()
        {
            while (isPatrolling && waypoints.Length > 0)
            {
                // Skip if player is detected and we should pause
                if (playerDetected && pauseOnPlayerDetection)
                {
                    CurrentState = PatrolState.PlayerDetected;
                    yield return new WaitForSeconds(playerDetectionPause);
                    continue;
                }

                // Get current waypoint
                Transform targetWaypoint = GetCurrentWaypoint();
                if (targetWaypoint == null)
                {
                    yield return new WaitForSeconds(0.1f);
                    continue;
                }

                // Move to waypoint
                CurrentState = PatrolState.Patrolling;
                agent.SetDestination(targetWaypoint.position);

                // Wait until we reach the waypoint
                while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
                {
                    if (!isPatrolling) yield break;
                    yield return new WaitForSeconds(0.1f);
                }

                // Wait at waypoint
                CurrentState = PatrolState.Waiting;

                yield return new WaitForSeconds(waitTime);

                // Move to next waypoint
                MoveToNextWaypoint();
            }
        }

        private Transform GetCurrentWaypoint()
        {
            if (waypoints == null || waypoints.Length == 0) return null;

            // Clamp index to valid range
            currentWaypointIndex = Mathf.Clamp(currentWaypointIndex, 0, waypoints.Length - 1);
            return waypoints[currentWaypointIndex];
        }

        private void MoveToNextWaypoint()
        {
            if (waypoints.Length <= 1) return;

            if (randomizeWaypoints)
            {
                // Pick random waypoint (excluding current one)
                int newIndex;
                do
                {
                    newIndex = Random.Range(0, waypoints.Length);
                } while (newIndex == currentWaypointIndex && waypoints.Length > 1);

                currentWaypointIndex = newIndex;
            }
            else
            {
                // Move to next waypoint in sequence
                currentWaypointIndex++;

                if (currentWaypointIndex >= waypoints.Length)
                {
                    if (loopPatrol)
                    {
                        currentWaypointIndex = 0; // Loop back to start
                    }
                    else
                    {
                        currentWaypointIndex = waypoints.Length - 1; // Stay at last waypoint
                        StopPatrol(); // Stop patrolling if not looping
                    }
                }
            }
        }

        private void HandlePlayerDetection()
        {
            if (player == null) return;

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            bool wasDetected = playerDetected;
            playerDetected = distanceToPlayer <= detectionRange;

            // If detection state changed, handle start/stop behavior
            if (playerDetected && !wasDetected)
            {
                // Player was just detected
                if (stopOnPlayerDetection)
                {
                    // Stop patrolling immediately
                    PausePatrol();
                }
            }
            else if (!playerDetected && wasDetected)
            {
                // Player was just lost
                if (resumeOnPlayerLost)
                {
                    // Resume after a short delay
                    StartCoroutine(ResumeAfterDelayCoroutine(resumeDelay));
                }
            }
        }

        private IEnumerator ResumeAfterDelayCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (!playerDetected && !isPatrolling)
            {
                StartPatrol();
            }
        }



        private void UpdateAnimation()
        {
            bool isMoving = agent.velocity.sqrMagnitude > 0.01f;

            // Set main moving flag
            animator.SetBool(IS_MOVING, isMoving);

            if (isMoving)
            {
                // Compute primary facing from velocity and store lastFacing
                Vector3 velocity = agent.velocity.normalized;
                if (Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y))
                {
                    // Horizontal primary
                    bool movingRight = velocity.x > 0f;
                    animator.SetBool(MOVING_LEFT, !movingRight);
                    animator.SetBool(MOVING_RIGHT, movingRight);
                    animator.SetBool(MOVING_UP, false);
                    animator.SetBool(MOVING_DOWN, false);
                    lastFacing = movingRight ? Facing.Right : Facing.Left;
                }
                else
                {
                    // Vertical primary
                    bool movingUp = velocity.y > 0f;
                    animator.SetBool(MOVING_LEFT, false);
                    animator.SetBool(MOVING_RIGHT, false);
                    animator.SetBool(MOVING_UP, movingUp);
                    animator.SetBool(MOVING_DOWN, !movingUp);
                    lastFacing = movingUp ? Facing.Up : Facing.Down;
                }

                // Clear idle flags while moving
                animator.SetBool(IDLE_LEFT, false);
                animator.SetBool(IDLE_RIGHT, false);
                animator.SetBool(IDLE_UP, false);
                animator.SetBool(IDLE_DOWN, false);
            }
            else
            {
                // Clear moving flags
                animator.SetBool(MOVING_LEFT, false);
                animator.SetBool(MOVING_RIGHT, false);
                animator.SetBool(MOVING_UP, false);
                animator.SetBool(MOVING_DOWN, false);

                // Set idle flag based on lastFacing
                animator.SetBool(IDLE_LEFT, lastFacing == Facing.Left);
                animator.SetBool(IDLE_RIGHT, lastFacing == Facing.Right);
                animator.SetBool(IDLE_UP, lastFacing == Facing.Up);
                animator.SetBool(IDLE_DOWN, lastFacing == Facing.Down);
            }
        }

        // Public API
        public void SetWaypoints(Transform[] newWaypoints)
        {
            waypoints = newWaypoints;
            currentWaypointIndex = 0;
        }

        public void AddWaypoint(Transform waypoint)
        {
            System.Array.Resize(ref waypoints, waypoints.Length + 1);
            waypoints[waypoints.Length - 1] = waypoint;
        }

        public void SetPatrolSpeed(float speed)
        {
            patrolSpeed = speed;
            agent.speed = speed;
        }

        // Properties
        public bool IsPatrolling => isPatrolling;
        public bool IsPlayerDetected => playerDetected;
        public int CurrentWaypointIndex => currentWaypointIndex;
        public Transform CurrentWaypoint => GetCurrentWaypoint();

        // Gizmos for debugging
        private void OnDrawGizmos()
        {
            // Draw waypoints and patrol path
            if (waypoints != null && waypoints.Length > 1)
            {
                Gizmos.color = Color.yellow;
                for (int i = 0; i < waypoints.Length; i++)
                {
                    if (waypoints[i] == null) continue;

                    // Draw waypoint
                    Gizmos.DrawWireSphere(waypoints[i].position, 0.5f);

                    // Draw path to next waypoint
                    if (loopPatrol || i < waypoints.Length - 1)
                    {
                        int nextIndex = (i + 1) % waypoints.Length;
                        if (waypoints[nextIndex] != null)
                        {
                            Gizmos.DrawLine(waypoints[i].position, waypoints[nextIndex].position);
                        }
                    }
                }
            }

            // Draw current waypoint
            if (Application.isPlaying && GetCurrentWaypoint() != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(GetCurrentWaypoint().position, 0.7f);
            }


            // Draw current path
            if (Application.isPlaying && agent != null && agent.hasPath)
            {
                Gizmos.color = Color.blue;
                Vector3[] corners = agent.path.corners;
                for (int i = 0; i < corners.Length - 1; i++)
                {
                    Gizmos.DrawLine(corners[i], corners[i + 1]);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Draw more detailed info when selected
            if (waypoints != null)
            {
                Gizmos.color = Color.cyan;
                for (int i = 0; i < waypoints.Length; i++)
                {
                    if (waypoints[i] != null)
                    {
                        Gizmos.DrawSphere(waypoints[i].position, 0.2f);
                    }
                }
            }
        }
    }
}