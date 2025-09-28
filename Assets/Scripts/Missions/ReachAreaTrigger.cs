using UnityEngine;

namespace Taallam.Missions
{
    [RequireComponent(typeof(Collider2D))]
    public class ReachAreaTrigger : MonoBehaviour
    {
        [SerializeField] private string areaId = "crosswalk";
        [SerializeField] private string playerTag = "Player";

        private void Reset() { GetComponent<Collider2D>().isTrigger = true; }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(playerTag)) return;
            MissionManager.Instance?.ReportReachArea(areaId);
        }
    }
}
