using UnityEngine;
using UnityEngine.InputSystem;

namespace TaallamGame.Player
{
    // Centralized input handler using Unity's new Input System.
    // Exposes simple properties other systems can poll.
    // Requires a PlayerInput component on the same GameObject.
    // Compatible with both PlayerInput "Send Messages" (InputValue) and "Unity Events" (CallbackContext).
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputHandler : MonoBehaviour
    {
        [Header("Input Maps")]
        [Tooltip("Action map to activate on enable if none is active. Leave empty to skip switching.")]
        [SerializeField] private string defaultActionMap = "Gameplay";

        // Movement vector (x,y) from WASD/Left Stick.
        public Vector2 Move { get; private set; }
        // Aiming vector (x,y), if you later add separate aim.
        public Vector2 Aim { get; private set; }
        // Action buttons
        public bool DashPressed { get; private set; }
        public bool InteractPressed { get; private set; }
        public bool AttackPressed { get; private set; }

        private PlayerInput _playerInput;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
        }

        private void OnEnable()
        {
            // Optionally, ensure default action map is active
            if (_playerInput != null && _playerInput.actions != null && _playerInput.currentActionMap == null)
            {
                TrySwitchToActionMap(defaultActionMap);
            }
        }

        // These methods are invoked by the Input System via PlayerInput component events.
        // Ensure the action names below match your Input Actions asset (e.g., "Move", "Aim", etc.).

        public void OnMove(InputAction.CallbackContext ctx)
        {
            Move = ctx.ReadValue<Vector2>();
        }

        // Send Messages overload
        public void OnMove(InputValue value)
        {
            Move = value.Get<Vector2>();
        }

        public void OnAim(InputAction.CallbackContext ctx)
        {
            Aim = ctx.ReadValue<Vector2>();
        }

        // Send Messages overload
        public void OnAim(InputValue value)
        {
            Aim = value.Get<Vector2>();
        }

        public void OnDash(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
                DashPressed = true;
            if (ctx.canceled)
                DashPressed = false;
        }

        // Send Messages overloads (performed/canceled)
        public void OnDash(InputValue value)
        {
            DashPressed = true;
        }

        public void OnDashCanceled(InputValue value)
        {
            DashPressed = false;
        }

        public void OnInteract(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                InteractPressed = true;
            }
            if (ctx.canceled)
                InteractPressed = false;
        }

        // Send Messages overloads
        public void OnInteract(InputValue value)
        {
            InteractPressed = true;
        }

        public void OnInteractCanceled(InputValue value)
        {
            InteractPressed = false;
        }

        public void OnAttack(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
                AttackPressed = true;
            if (ctx.canceled)
                AttackPressed = false;
        }

        // Send Messages overloads
        public void OnAttack(InputValue value)
        {
            AttackPressed = true;
        }

        public void OnAttackCanceled(InputValue value)
        {
            AttackPressed = false;
        }

        // Helper methods for one-shot actions if you prefer consume-once semantics later
        public bool ConsumeAttackPressed()
        {
            if (AttackPressed)
            {
                AttackPressed = false;
                return true;
            }
            return false;
        }

        public bool ConsumeInteractPressed()
        {
            if (InteractPressed)
            {
                InteractPressed = false;
                return true;
            }
            return false;
        }

        public bool ConsumeDashPressed()
        {
            if (DashPressed)
            {
                DashPressed = false;
                return true;
            }
            return false;
        }

        // Public API to change/switch action maps from other systems or via inspector
        public void SetDefaultActionMap(string mapName)
        {
            defaultActionMap = mapName;
        }

        public bool TrySwitchToActionMap(string mapName)
        {
            if (_playerInput == null || _playerInput.actions == null || string.IsNullOrWhiteSpace(mapName))
                return false;

            var map = _playerInput.actions.FindActionMap(mapName, throwIfNotFound: false);
            if (map == null) return false;
            _playerInput.SwitchCurrentActionMap(map.name);
            return true;
        }
    }
}
