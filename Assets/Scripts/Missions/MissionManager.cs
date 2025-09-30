using System;
using System.Collections.Generic;
using UnityEngine;
using TaallamGame.Save;

namespace TaallamGame.Missions
{
    public enum MissionLifecycle { Locked, Available, Active, TurnIn, Completed }

    public class MissionManager : MonoBehaviour
    {
        public static MissionManager Instance { get; private set; }

        [Tooltip("All mission assets for this build")]
        [SerializeField] private List<MissionDefinition> missions = new List<MissionDefinition>();

        private readonly Dictionary<string, MissionDefinition> defs = new();
        private readonly Dictionary<string, MissionRuntime> state = new();

        public string ActiveMissionId { get; private set; }
        public MissionDefinition ActiveMission => string.IsNullOrEmpty(ActiveMissionId) ? null : (defs.ContainsKey(ActiveMissionId) ? defs[ActiveMissionId] : null);
        public int ActiveStep => string.IsNullOrEmpty(ActiveMissionId) ? -1 : state[ActiveMissionId].step;

        public event Action<MissionDefinition, int> OnActiveMissionChanged; // (def, step)
        public event Action<MissionDefinition, int> OnProgressChanged;      // (def, step)
        public event Action<MissionDefinition> OnMissionCompleted;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            defs.Clear();
            foreach (var m in missions)
            {
                if (m == null || string.IsNullOrWhiteSpace(m.id)) continue;
                defs[m.id] = m;
                if (!state.ContainsKey(m.id))
                    state[m.id] = new MissionRuntime { id = m.id, lifecycle = m.startAvailable ? MissionLifecycle.Available : MissionLifecycle.Locked };
            }

            ProfileManager.AfterLoad += ApplyFromSave;
            ProfileManager.BeforeSave += CaptureToSave;
        }

        private void OnDestroy()
        {
            if (ProfileManager.Instance != null)
            {
                ProfileManager.AfterLoad -= ApplyFromSave;
                ProfileManager.BeforeSave -= CaptureToSave;
            }
        }

        // Public API -------------------------------------------------------------

        public void SetAvailable(string id)
        {
            if (!defs.ContainsKey(id)) return;
            var s = GetState(id);
            if (s.lifecycle == MissionLifecycle.Locked) s.lifecycle = MissionLifecycle.Available;
        }

        public bool AcceptMission(string id)
        {
            if (!defs.TryGetValue(id, out var def)) return false;
            var s = GetState(id);
            if (s.lifecycle != MissionLifecycle.Available) return false;

            s.lifecycle = MissionLifecycle.Active;
            s.step = 0; s.sub = 0;
            ActiveMissionId = id;
            OnActiveMissionChanged?.Invoke(def, s.step);
            return true;
        }

        public void TurnInActive()
        {
            if (ActiveMission == null) return;
            var s = GetState(ActiveMissionId);
            if (s.lifecycle != MissionLifecycle.TurnIn) return;

            s.lifecycle = MissionLifecycle.Completed;
            OnMissionCompleted?.Invoke(defs[ActiveMissionId]);
            ActiveMissionId = null;
            OnActiveMissionChanged?.Invoke(null, -1);
        }

        // Reporters: call these from triggers/UI/minigames
        public void ReportReachArea(string areaId)  => TryAdvance(GoalType.ReachArea, areaId, 1, true);
        public void ReportInteract(string targetId) => TryAdvance(GoalType.Interact, targetId, 1, true);
        public void ReportCollect(string itemId, int amount = 1) => TryAdvance(GoalType.Collect, itemId, amount, true);
        public void ReportQuizResult(string quizId, bool passed) => TryAdvance(GoalType.Quiz, quizId, 1, passed);
        public void ReportMiniGameComplete(string gameId, bool passed) => TryAdvance(GoalType.MiniGame, gameId, 1, passed);

        // Internal ---------------------------------------------------------------

        private void TryAdvance(GoalType type, string targetId, int amount, bool success)
        {
            if (ActiveMission == null) return;
            var s = GetState(ActiveMissionId);
            var def = defs[ActiveMissionId];

            if (s.lifecycle != MissionLifecycle.Active) return;
            if (s.step < 0 || s.step >= def.goals.Count) return;

            var goal = def.goals[s.step];
            if (goal.type != type) return;
            if (!string.IsNullOrEmpty(goal.targetId) && goal.targetId != targetId) return;

            if (!success) return;

            // Handle counts
            s.sub += Mathf.Max(1, amount);
            int needed = Mathf.Max(1, goal.amount);
            if (s.sub < needed)
            {
                OnProgressChanged?.Invoke(def, s.step);
                return;
            }

            // Step complete
            s.step++;
            s.sub = 0;
            if (s.step >= def.goals.Count)
            {
                s.lifecycle = MissionLifecycle.TurnIn; // go back to giver
                OnProgressChanged?.Invoke(def, def.goals.Count);
            }
            else
            {
                OnProgressChanged?.Invoke(def, s.step);
            }
        }

        private MissionRuntime GetState(string id)
        {
            if (!state.ContainsKey(id)) state[id] = new MissionRuntime { id = id };
            return state[id];
        }

        // Save/Load binding ------------------------------------------------------

        private void ApplyFromSave(SaveFile file)
        {
            ActiveMissionId = null;
            if (file?.missions != null)
            {
                foreach (var m in file.missions)
                {
                    if (!defs.ContainsKey(m.id)) continue;
                    MissionLifecycle life = ParseLifecycle(m.state);
                    state[m.id] = new MissionRuntime
                    {
                        id = m.id,
                        lifecycle = life,
                        step = m.progress,
                        sub = m.subProgress
                    };
                    if (life == MissionLifecycle.Active) ActiveMissionId = m.id;
                }
            }
            OnActiveMissionChanged?.Invoke(ActiveMission, ActiveStep);
        }

        private void CaptureToSave(SaveFile file)
        {
            file.missions.Clear();
            foreach (var kv in state)
            {
                var s = kv.Value;
                file.missions.Add(new MissionSaveData
                {
                    id = s.id,
                    state = ToLifecycleString(s.lifecycle),
                    progress = s.step,
                    subProgress = s.sub
                });
            }
        }

        private static string ToLifecycleString(MissionLifecycle s) => s.ToString().ToLowerInvariant();

        private static MissionLifecycle ParseLifecycle(string v)
        {
            if (string.IsNullOrEmpty(v)) return MissionLifecycle.Locked;
            v = v.ToLowerInvariant();
            return v switch
            {
                "available" => MissionLifecycle.Available,
                "active"    => MissionLifecycle.Active,
                "turnin"    => MissionLifecycle.TurnIn,
                "completed" => MissionLifecycle.Completed,
                _           => MissionLifecycle.Locked
            };
        }

        private class MissionRuntime
        {
            public string id;
            public MissionLifecycle lifecycle;
            public int step;
            public int sub; // count inside current step
        }
    }
}
