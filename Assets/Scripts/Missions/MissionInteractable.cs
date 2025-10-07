using UnityEngine;

namespace TaallamGame.Missions
{
    // Simple mission reporter component.
    // Keeps only mission-related data and exposes Interact()/ReportInteract() for other systems to call.
    public class MissionInteractable : MonoBehaviour
    {
        [SerializeField] private string interactId = "npc_police";
        [SerializeField] private bool reportWhenInteracted = true;

        // Call this when the player interacts. Kept for compatibility with existing PlayerInteraction code.
        public void Interact()
        {
            if (reportWhenInteracted)
                ReportInteract();
        }

        // Public API to report this interaction to the mission system.
        public void ReportInteract()
        {
            // Try new bridge first, fall back to legacy MissionManager
            var bridge = InkMissionBridge.Instance;
            if (bridge != null)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                bridge.ReportInteract(interactId);
#pragma warning restore CS0618 // Type or member is obsolete
            }
            else
            {
#pragma warning disable CS0618 // Type or member is obsolete
                MissionManager.Instance?.ReportInteract(interactId);
#pragma warning restore CS0618 // Type or member is obsolete
            }
        }
    }
}
