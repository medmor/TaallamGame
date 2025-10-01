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

        // Call this from your NPC dialogue when the player selects an interaction option
        public void Interact()
        {
            print($"MissionInteractable.Interact() called for {interactId}");
            if (inkJson && InkDialogueRunner.Instance && !InkDialogueRunner.Instance.IsRunning)
            {print($"Starting dialogue {inkJson.name}...");
                InkDialogueRunner.Instance.StartDialogue(inkJson, onComplete: () =>
                {
                    print("Dialogue complete.");
                    if (reportWhenDialogueEnds)
                        MissionManager.Instance?.ReportInteract(interactId);
                });
            }
            else
            {
                MissionManager.Instance?.ReportInteract(interactId);
            }
        }
    }
}
