using UnityEngine;
using TaallamGame.Dialogue;

namespace TaallamGame.Interaction
{
    [RequireComponent(typeof(Collider2D))]
    public class DoorInteract : MonoBehaviour
    {
        [Tooltip("Ink JSON TextAsset compiled from door.ink")] 
        public TextAsset doorInkJson;
        public DialogueManager dialogueManager;

        private bool _inside;

        private void Reset()
        {
            var col = GetComponent<Collider2D>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            _inside = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            _inside = false;
        }

        // Call this from your input handler when pressing Interact while _inside
        public void TryOpen()
        {
            if (!_inside || !doorInkJson || !dialogueManager) return;
            dialogueManager.EnterDialogueMode(doorInkJson, null);
        }
    }
}
