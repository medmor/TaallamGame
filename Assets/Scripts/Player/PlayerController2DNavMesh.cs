using UnityEngine;
using UnityEngine.AI;

namespace TaallamGame.Player
{
    /// <summary>
    /// NavMesh-based player controller for 2D top-down movement.
    /// Uses PlayerInputHandler for input and supports both keyboard and click-to-move.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class PlayerController2DNavMesh : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 720f; // degrees per second

        [Header("Input")]
        [SerializeField] private bool useKeyboardMovement = true;
        [SerializeField] private bool useClickToMove = true;
        [SerializeField] private PlayerInputHandler inputHandler;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private LayerMask groundLayer = -1; // what can be clicked on

        [Header("Visual Feedback")]
        [SerializeField] private GameObject moveTargetPrefab; // optional visual indicator
        [SerializeField] private float targetIndicatorDuration = 1f;

        [Header("Animation")]
        [SerializeField] private Animator animator; // optional for animations

        private NavMeshAgent agent;
        private GameObject currentTargetIndicator;
        private Vector3 keyboardMoveDirection;
        private bool isUsingKeyboard = false;
        
        // Animation parameter names
        private const string SPEED = "Speed";
        private const string IS_MOVING = "IsMoving";

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            
            // Auto-find PlayerInputHandler if not assigned
            if (inputHandler == null)
                inputHandler = GetComponent<PlayerInputHandler>();
            
            // Configure NavMeshAgent for 2D
            agent.updateRotation = false; // We'll handle rotation manually
            agent.updateUpAxis = false;   // For 2D navigation
            agent.speed = moveSpeed;
        }

        private void Start()
        {
            // Auto-find camera if not assigned
            if (playerCamera == null)
                playerCamera = Camera.main;
        }

        private void Update()
        {
            HandleInput();
            UpdateMovement();
            UpdateAnimation();
        }

        private void HandleInput()
        {
            // Handle keyboard input using PlayerInputHandler
            if (useKeyboardMovement && inputHandler != null)
            {
                HandleKeyboardInput();
            }

            // Handle mouse input for click-to-move (using interact button)
            if (useClickToMove && inputHandler != null && inputHandler.ConsumeInteractPressed())
            {
                Vector3 worldPosition = GetMouseWorldPosition();
                if (worldPosition != Vector3.zero)
                {
                    MoveToPosition(worldPosition);
                }
            }
        }

        private void HandleKeyboardInput()
        {
            // Get movement input from PlayerInputHandler
            Vector2 input = inputHandler.Move;

            if (input.magnitude > 0.1f)
            {
                isUsingKeyboard = true;
                keyboardMoveDirection = new Vector3(input.x, input.y, 0);
                
                // Calculate target position ahead of the player
                Vector3 targetPosition = transform.position + keyboardMoveDirection * moveSpeed;
                
                // Use NavMesh to move towards the target
                if (agent.isOnNavMesh)
                {
                    agent.SetDestination(targetPosition);
                }
            }
            else if (isUsingKeyboard)
            {
                // Stop when no keys are pressed
                isUsingKeyboard = false;
                agent.ResetPath();
            }
        }

        private Vector3 GetMouseWorldPosition()
        {
            if (playerCamera == null) return Vector3.zero;

            // Get mouse position using the new Input System
            Vector2 mousePosition = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
            Ray ray = playerCamera.ScreenPointToRay(mousePosition);
            
            // For 2D, we can use a simple plane raycast or layer-based raycast
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                return hit.point;
            }
            
            // Alternative: Project to Z=0 plane for pure 2D
            Plane groundPlane = new Plane(Vector3.forward, Vector3.zero);
            if (groundPlane.Raycast(ray, out float distance))
            {
                return ray.GetPoint(distance);
            }

            return Vector3.zero;
        }

        private void MoveToPosition(Vector3 targetPosition)
        {
            // Set NavMeshAgent destination
            if (agent.isOnNavMesh)
            {
                // Stop keyboard movement when using click-to-move
                isUsingKeyboard = false;
                
                agent.SetDestination(targetPosition);
                
                // Show visual feedback
                ShowTargetIndicator(targetPosition);
                
                Debug.Log($"Moving to: {targetPosition}");
            }
            else
            {
                Debug.LogWarning("Player is not on NavMesh!");
            }
        }

        private void StopMovement()
        {
            if (agent.isOnNavMesh)
            {
                agent.ResetPath();
                isUsingKeyboard = false;
                Debug.Log("Movement stopped");
            }
        }

        private void UpdateMovement()
        {
            if (!agent.isOnNavMesh) return;

            // Handle 2D rotation towards movement direction
            Vector3 velocityDirection = agent.velocity.normalized;
            
            // For keyboard movement, prefer input direction over velocity
            if (isUsingKeyboard && keyboardMoveDirection.magnitude > 0.1f)
            {
                velocityDirection = keyboardMoveDirection;
            }

            if (velocityDirection.sqrMagnitude > 0.1f)
            {
                float targetAngle = Mathf.Atan2(velocityDirection.y, velocityDirection.x) * Mathf.Rad2Deg;
                
                // Smooth rotation
                float currentAngle = transform.eulerAngles.z;
                float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0, 0, newAngle);
            }
        }

        private void UpdateAnimation()
        {
            if (animator == null) return;

            bool isMoving = agent.velocity.sqrMagnitude > 0.1f;
            float speed = agent.velocity.magnitude;

            animator.SetBool(IS_MOVING, isMoving);
            animator.SetFloat(SPEED, speed);
        }

        private void ShowTargetIndicator(Vector3 position)
        {
            // Clean up previous indicator
            if (currentTargetIndicator != null)
            {
                Destroy(currentTargetIndicator);
            }

            // Create new indicator if prefab is assigned
            if (moveTargetPrefab != null)
            {
                currentTargetIndicator = Instantiate(moveTargetPrefab, position, Quaternion.identity);
                
                // Auto-destroy after duration
                Destroy(currentTargetIndicator, targetIndicatorDuration);
            }
        }

        // Public API for external scripts
        public void SetMoveSpeed(float newSpeed)
        {
            moveSpeed = newSpeed;
            if (agent != null)
                agent.speed = moveSpeed;
        }

        public bool IsMoving => agent != null && agent.velocity.sqrMagnitude > 0.1f;
        public bool HasPath => agent != null && agent.hasPath;
        public Vector3 Destination => agent != null ? agent.destination : transform.position;
        public float DistanceToDestination => agent != null ? agent.remainingDistance : 0f;

        // Gizmos for debugging
        private void OnDrawGizmosSelected()
        {
            if (agent == null) return;

            // Draw path
            if (agent.hasPath)
            {
                Gizmos.color = Color.blue;
                Vector3[] corners = agent.path.corners;
                for (int i = 0; i < corners.Length - 1; i++)
                {
                    Gizmos.DrawLine(corners[i], corners[i + 1]);
                }
            }

            // Draw destination
            if (agent.hasPath)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(agent.destination, 0.5f);
            }
        }
    }
}