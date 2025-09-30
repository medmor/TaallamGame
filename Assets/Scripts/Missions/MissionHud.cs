using TMPro;
using UnityEngine;

namespace TaallamGame.Missions
{
    public class MissionHud : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI label;

        private void OnEnable()
        {
            if (MissionManager.Instance == null) return;
            MissionManager.Instance.OnActiveMissionChanged += Refresh;
            MissionManager.Instance.OnProgressChanged += Refresh;
            Refresh(MissionManager.Instance.ActiveMission, MissionManager.Instance.ActiveStep);
        }

        private void OnDisable()
        {
            if (MissionManager.Instance == null) return;
            MissionManager.Instance.OnActiveMissionChanged -= Refresh;
            MissionManager.Instance.OnProgressChanged -= Refresh;
        }

        private void Refresh(MissionDefinition def, int step)
        {
            if (label == null) return;
            if (def == null) { label.text = string.Empty; return; }

            string line = def.title_ar;
            if (step >= 0 && step < def.goals.Count)
            {
                var g = def.goals[step];
                var hint = string.IsNullOrWhiteSpace(g.hint_ar) ? string.Empty : $" — {g.hint_ar}";
                line = $"{def.title_ar}{hint}";
            }
            else if (step >= def.goals.Count)
            {
                line = $"{def.title_ar} — سلِّم المهمة";
            }
            label.text = line;
        }
    }
}
