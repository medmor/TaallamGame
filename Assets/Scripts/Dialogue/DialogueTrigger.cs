using UnityEngine;
using TaallamGame.Player;

namespace TaallamGame.Dialogue
{
    [RequireComponent(typeof(Collider2D))]
    public class DialogueTrigger : MonoBehaviour
    {
        [Header("Visual Cue")]
        [SerializeField] private GameObject visualCue;

        [Header("Emote Animator")]
        [SerializeField] private Animator emoteAnimator;

        [Header("Ink JSON")]
        [SerializeField] private TextAsset inkJSON;

        [Header("Input")]
        [SerializeField] private bool triggerOnInteract = true; // press E near trigger

        private bool _playerInRange;
        private PlayerInputHandler _playerInput;

        private void Reset()
        {
            var col = GetComponent<Collider2D>();
            if (col) col.isTrigger = true;
        }

        private void Awake()
        {
            _playerInRange = false;
            if (visualCue) visualCue.SetActive(false);
        }

        private void Update()
        {
            var dm = DialogueManager.GetInstance();
            bool dialogueActive = dm != null && dm.dialogueIsPlaying;

            if (_playerInRange && !dialogueActive)
            {
                if (visualCue && !visualCue.activeSelf) visualCue.SetActive(true);

                if (triggerOnInteract)
                {
                    if (_playerInput == null)
                        _playerInput = FindFirstObjectByType<PlayerInputHandler>();

                    if (_playerInput != null && _playerInput.ConsumeInteractPressed())
                    {
                        dm?.EnterDialogueMode(inkJSON, emoteAnimator);
                    }
                }
            }
            else
            {
                if (visualCue && visualCue.activeSelf) visualCue.SetActive(false);
            }
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag("Player"))
            {
                _playerInRange = true;
                if (_playerInput == null)
                    _playerInput = collider.GetComponentInParent<PlayerInputHandler>();
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.CompareTag("Player"))
            {
                _playerInRange = false;
            }
        }

        // Public helper so external systems can trigger dialogue (e.g. PlayerInteraction)
        public void Interact()
        {
            var dm = DialogueManager.GetInstance();
            if (dm == null) return;
            dm.EnterDialogueMode(inkJSON, emoteAnimator);
        }
    }
}
