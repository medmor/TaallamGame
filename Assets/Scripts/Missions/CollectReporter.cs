using UnityEngine;

namespace TaallamGame.Missions
{
    public class CollectReporter : MonoBehaviour
    {
        [SerializeField] private string itemId = "trash";
        [SerializeField] private int amount = 1;

        // Call this method after collecting/picking up an item
        public void Report()
        {
            MissionManager.Instance?.ReportCollect(itemId, amount);
        }
    }
}
