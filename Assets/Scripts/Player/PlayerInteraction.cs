using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TaallamGame.Missions;
using TaallamGame.Dialogue;

namespace TaallamGame.Player
{
    [RequireComponent(typeof(Collider2D))]
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Setup")]
        [Tooltip("Optional. If present on the same GameObject, will be used to read Interact presses.")]
        [SerializeField] private PlayerInputHandler input;
        [Tooltip("Optional on-screen prompt to show when an interactable is in range.")]
        [SerializeField] private GameObject promptUI;

    private readonly List<MissionInteractable> _inRange = new List<MissionInteractable>();
        private readonly List<DialogueTrigger> _dialogueInRange = new List<DialogueTrigger>();
        private MissionInteractable _current;
        private DialogueTrigger _currentDialogue;
        private bool _interactPressed; // set by Send Messages or manually

        private void Reset()
        {
            // Ensure the sensor collider is a trigger.
            var col = GetComponent<Collider2D>();
            if (col) col.isTrigger = true;
        }

        private void Awake()
        {
            if (!input) input = GetComponent<PlayerInputHandler>();
        }

        private void Update()
        {
            // Skip interaction input when dialogue is active - let DialogueManager handle it
            if (TaallamGame.Dialogue.DialogueManager.GetInstance()?.dialogueIsPlaying == true)
                return;

            // Option A: using your PlayerInputHandler
            bool pressed = input ? input.ConsumeInteractPressed() : false;

            // Option B: direct Send Messages from PlayerInput (OnInteract below sets _interactPressed)
            if (_interactPressed) { pressed = true; _interactPressed = false; }

            if (pressed) TryInteract();

            // Prompt visibility
            if (promptUI)
                promptUI.SetActive(_current != null || _currentDialogue != null);
        }

        private void TryInteract()
        {
            // Prefer dialogue triggers when available
            if (_currentDialogue != null)
            {
                _currentDialogue.Interact();
                return;
            }

            if (_current != null)
            {
                _current.Interact();
            }
        }

        // Track nearest interactable inside the trigger
        private void OnTriggerEnter2D(Collider2D other)
        {
            var dt = other.GetComponentInParent<DialogueTrigger>();
            if (dt && !_dialogueInRange.Contains(dt))
            {
                _dialogueInRange.Add(dt);
                RecomputeCurrent();
            }

            var mi = other.GetComponentInParent<MissionInteractable>();
            if (mi && !_inRange.Contains(mi))
            {
                _inRange.Add(mi);
                RecomputeCurrent();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var dt = other.GetComponentInParent<DialogueTrigger>();
            if (dt && _dialogueInRange.Remove(dt))
            {
                if (_currentDialogue == dt) _currentDialogue = null;
                RecomputeCurrent();
            }

            var mi = other.GetComponentInParent<MissionInteractable>();
            if (mi && _inRange.Remove(mi))
            {
                if (_current == mi) _current = null;
                RecomputeCurrent();
            }
        }

        private void RecomputeCurrent()
        {
            // Prefer nearest DialogueTrigger if any; otherwise nearest MissionInteractable
            _currentDialogue = null;
            _current = null;

            var me = transform.position;

            float best = float.MaxValue;
            for (int i = _dialogueInRange.Count - 1; i >= 0; i--)
            {
                var t = _dialogueInRange[i] ? _dialogueInRange[i].transform : null;
                if (!t) { _dialogueInRange.RemoveAt(i); continue; }

                float d = (t.position - me).sqrMagnitude;
                if (d < best) { best = d; _currentDialogue = _dialogueInRange[i]; }
            }

            if (_currentDialogue != null) return;

            best = float.MaxValue;
            for (int i = _inRange.Count - 1; i >= 0; i--)
            {
                var t = _inRange[i] ? _inRange[i].transform : null;
                if (!t) { _inRange.RemoveAt(i); continue; }

                float d = (t.position - me).sqrMagnitude;
                if (d < best) { best = d; _current = _inRange[i]; }
            }
        }

        // PlayerInput “Send Messages” compatibility (if you set Behavior = Send Messages)
        private void OnInteract(InputValue _)
        {
            _interactPressed = true;
        }
        private void OnInteractCanceled(InputValue _) { /* optional */ }
    }
}