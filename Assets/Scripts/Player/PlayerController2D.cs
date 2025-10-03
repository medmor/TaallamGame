using TaallamGame.Dialogue;
using UnityEngine;

namespace TaallamGame.Player
{
    // Simple top-down 2D character controller using Rigidbody2D for smooth movement.
    // Attach to your Player GameObject with a Rigidbody2D and (optionally) Animator.
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController2D : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float acceleration = 20f;
        [SerializeField] private float deceleration = 30f;

        [Header("Dash")]
        [SerializeField] private float dashSpeed = 12f;
        [SerializeField] private float dashDuration = 0.2f;
        [SerializeField] private float dashCooldown = 0.8f;

        [Header("References")]
        [SerializeField] private PlayerInputHandler input;
        [SerializeField] private Animator animator; // Optional
        [Header("Gameplay State")]
        [SerializeField] private bool pauseMovementWhenDialogue = true;

        private Rigidbody2D _rb;
        private Vector2 _currentVelocity;
        private float _dashTimer;
        private float _dashCooldownTimer;
        private Vector2 _dashDirection;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            if (input == null)
            {
                input = GetComponent<PlayerInputHandler>();
            }
        }

        private void Update()
        {
            bool dialogueActive = pauseMovementWhenDialogue && DialogueManager.GetInstance()?.dialogueIsPlaying == true;

            if (!dialogueActive)
            {
                HandleDashInput();
            }
            UpdateAnimator();
        }

        private void FixedUpdate()
        {
            // Pause all movement when dialogue is active so arrows/wasd can navigate choices
            if (pauseMovementWhenDialogue && DialogueManager.GetInstance()?.dialogueIsPlaying == true)
            {
                _rb.linearVelocity = Vector2.zero;
                return;
            }

            if (_dashTimer > 0f)
            {
                // During dash, override velocity
                _rb.linearVelocity = _dashDirection * dashSpeed;
                _dashTimer -= Time.fixedDeltaTime;
                if (_dashTimer <= 0f)
                {
                    _dashCooldownTimer = dashCooldown;
                }
                return;
            }

            // Regular movement
            Vector2 targetVelocity = (input != null ? input.Move : Vector2.zero) * moveSpeed;
            Vector2 velocity = _rb.linearVelocity;

            // Apply acceleration/deceleration per axis for responsive top-down feel
            velocity.x = Mathf.MoveTowards(velocity.x, targetVelocity.x,
                (Mathf.Abs(targetVelocity.x) > 0.01f ? acceleration : deceleration) * Time.fixedDeltaTime);
            velocity.y = Mathf.MoveTowards(velocity.y, targetVelocity.y,
                (Mathf.Abs(targetVelocity.y) > 0.01f ? acceleration : deceleration) * Time.fixedDeltaTime);

            _rb.linearVelocity = velocity;
        }

        private void HandleDashInput()
        {
            if (_dashCooldownTimer > 0f)
            {
                _dashCooldownTimer -= Time.deltaTime;
            }

            if (input != null && input.ConsumeDashPressed() && _dashCooldownTimer <= 0f)
            {
                Vector2 moveDir = input.Move.sqrMagnitude > 0.01f ? input.Move.normalized : _rb.linearVelocity.normalized;
                if (moveDir == Vector2.zero)
                {
                    // No movement; dash in last known non-zero velocity or default right
                    moveDir = _dashDirection != Vector2.zero ? _dashDirection : Vector2.right;
                }
                _dashDirection = moveDir;
                _dashTimer = dashDuration;
            }
        }

        private void UpdateAnimator()
        {
            if (animator == null) return;

            Vector2 vel = _rb.linearVelocity;
            animator.SetFloat("MoveX", vel.x);
            animator.SetFloat("MoveY", vel.y);
            animator.SetFloat("Speed", vel.sqrMagnitude);
            animator.SetBool("IsDashing", _dashTimer > 0f);
        }

        // Public API to change movement properties at runtime if needed
        public void SetMoveSpeed(float newSpeed) => moveSpeed = Mathf.Max(0f, newSpeed);
        public void SetAcceleration(float newAccel) => acceleration = Mathf.Max(0f, newAccel);
        public void SetDeceleration(float newDecel) => deceleration = Mathf.Max(0f, newDecel);
    }
}
