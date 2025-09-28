using UnityEngine;

namespace Taallam.Missions
{
    public class MissionInteractable : MonoBehaviour
    {
        [SerializeField] private string interactId = "npc_police";

        // Call this from your NPC dialogue when the player selects an interaction option
        public void Interact()
        {
            MissionManager.Instance?.ReportInteract(interactId);
        }
    }
}
