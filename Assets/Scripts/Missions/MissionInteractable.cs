using UnityEngine;
using TaallamGame.Dialogue;

namespace TaallamGame.Missions
{
    public class MissionInteractable : MonoBehaviour
    {
        [SerializeField] private string interactId = "npc_police";
        [Header("Ink")]
        [SerializeField] private TextAsset inkJson; // assign police.ink.json in Inspector
        [SerializeField] private bool reportWhenDialogueEnds = true;
        [Header("Emote Animator")]
        [SerializeField] private Animator emoteAnimator;

        private bool _subscribed;

        // Call this when the player interacts
        public void Interact()
        {
            var dm = DialogueManager.GetInstance();
            if (dm == null)
            {
                MissionManager.Instance?.ReportInteract(interactId);
                return;
            }

            if (dm.dialogueIsPlaying)
            {
                // Already in dialogue; ignore re-entry
                return;
            }

            if (reportWhenDialogueEnds && !_subscribed)
            {
                dm.DialogueEnded += OnDialogueEnded;
                _subscribed = true;
            }

            dm.EnterDialogueMode(inkJson, emoteAnimator);
        }

        private void OnDialogueEnded()
        {
            var dm = DialogueManager.GetInstance();
            if (dm != null)
            {
                dm.DialogueEnded -= OnDialogueEnded;
                _subscribed = false;
            }
            if (reportWhenDialogueEnds)
                MissionManager.Instance?.ReportInteract(interactId);
        }
    }
}
