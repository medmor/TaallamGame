using System;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

namespace TaallamGame.Missions
{
    /// <summary>
    /// Simplified MissionManager that acts as a bridge between Ink and Unity.
    /// All mission logic is handled in Ink files - this just responds to external function calls.
    /// </summary>
    public class InkMissionBridge : MonoBehaviour
    {
        public static InkMissionBridge Instance { get; private set; }

        [Header("Debug")]
        [SerializeField] private bool debugLogging = true;

        // Events for UI/gameplay systems to listen to
        public event Action<string> OnMissionStarted;
        public event Action<string> OnMissionCompleted;
        public event Action<string> OnItemGiven;
        public event Action<string> OnSoundRequested;
        public event Action<string> OnEffectRequested;

        // Current mission tracking (for UI/save system)
        public string CurrentMissionId { get; private set; }
        public bool HasActiveMission => !string.IsNullOrEmpty(CurrentMissionId);

        // Story state tracking for persistence
        private readonly Dictionary<string, string> storyStates = new();

        private void Awake()
        {
            if (Instance != null && Instance != this) 
            { 
                Destroy(gameObject); 
                return; 
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Call this when creating any new Ink Story to bind external functions
        /// </summary>
        public void BindExternalFunctions(Story story)
        {
            if (story == null) return;

            // Mission lifecycle
            story.BindExternalFunction("external_StartMission", (string missionId) =>
            {
                Log($"Ink -> StartMission: {missionId}");
                CurrentMissionId = missionId;
                OnMissionStarted?.Invoke(missionId);
            });

            story.BindExternalFunction("external_CompleteMission", (string missionId) =>
            {
                Log($"Ink -> CompleteMission: {missionId}");
                if (CurrentMissionId == missionId)
                    CurrentMissionId = null;
                OnMissionCompleted?.Invoke(missionId);
            });

            // Item/inventory management
            story.BindExternalFunction("external_GiveItem", (string itemId) =>
            {
                Log($"Ink -> GiveItem: {itemId}");
                OnItemGiven?.Invoke(itemId);
            });

            // Audio/visual feedback
            story.BindExternalFunction("external_PlaySound", (string soundId) =>
            {
                Log($"Ink -> PlaySound: {soundId}");
                OnSoundRequested?.Invoke(soundId);
            });

            story.BindExternalFunction("external_ShowEffect", (string effectId) =>
            {
                Log($"Ink -> ShowEffect: {effectId}");
                OnEffectRequested?.Invoke(effectId);
            });

            // Save/persistence
            story.BindExternalFunction("external_SaveProgress", () =>
            {
                Log("Ink -> SaveProgress");
                SaveStoryState(story);
            });
        }

        /// <summary>
        /// Save the current story state for persistence
        /// </summary>
        public void SaveStoryState(Story story, string storyKey = "main")
        {
            if (story == null) return;
            
            string json = story.state.ToJson();
            storyStates[storyKey] = json;
            
            // Save to PlayerPrefs or your save system
            PlayerPrefs.SetString($"ink_story_state_{storyKey}", json);
            PlayerPrefs.Save();
            
            Log($"Saved story state for: {storyKey}");
        }

        /// <summary>
        /// Load story state from persistence
        /// </summary>
        public void LoadStoryState(Story story, string storyKey = "main")
        {
            if (story == null) return;

            string key = $"ink_story_state_{storyKey}";
            if (PlayerPrefs.HasKey(key))
            {
                string json = PlayerPrefs.GetString(key);
                if (!string.IsNullOrEmpty(json))
                {
                    story.state.LoadJson(json);
                    storyStates[storyKey] = json;
                    Log($"Loaded story state for: {storyKey}");
                }
            }
        }

        /// <summary>
        /// Get a variable value from a story (for UI/game state queries)
        /// </summary>
        public T GetStoryVariable<T>(Story story, string variableName, T defaultValue = default)
        {
            if (story == null)
                return defaultValue;

            try
            {
                var value = story.variablesState[variableName];
                if (value == null)
                    return defaultValue;
                    
                if (value is T typedValue)
                    return typedValue;
                
                // Try conversion for common types
                if (typeof(T) == typeof(bool) && value is int intVal)
                    return (T)(object)(intVal != 0);
                    
                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Set a variable in a story (for external game events affecting Ink state)
        /// </summary>
        public void SetStoryVariable<T>(Story story, string variableName, T value)
        {
            if (story == null) return;
            
            try
            {
                story.variablesState[variableName] = value;
                Log($"Set story variable: {variableName} = {value}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to set story variable {variableName}: {e.Message}");
            }
        }

        /// <summary>
        /// Clear all saved story states (for new game)
        /// </summary>
        public void ClearAllStoryStates()
        {
            storyStates.Clear();
            
            // Clear from PlayerPrefs
            var keys = new List<string>();
            for (int i = 0; i < 100; i++) // reasonable limit
            {
                string key = $"ink_story_state_{i}";
                if (PlayerPrefs.HasKey(key))
                    keys.Add(key);
            }
            
            foreach (string key in keys)
                PlayerPrefs.DeleteKey(key);
                
            PlayerPrefs.DeleteKey("ink_story_state_main");
            PlayerPrefs.Save();
            
            Log("Cleared all story states");
        }

        private void Log(string message)
        {
            if (debugLogging)
                Debug.Log($"[InkMissionBridge] {message}");
        }

        // Legacy compatibility methods (in case other code still references old MissionManager)
        [Obsolete("Use Ink external functions instead")]
        public void ReportInteract(string targetId) 
        {
            Log($"Legacy ReportInteract called: {targetId} - Consider using Ink external functions");
            // For now, just log - Ink should handle this via external functions
        }

        [Obsolete("Use Ink external functions instead")]
        public void ReportCollect(string itemId, int amount = 1) 
        {
            Log($"Legacy ReportCollect called: {itemId} x{amount} - Consider using Ink external functions");
            // For now, just log - Ink should handle this via external functions
        }

        [Obsolete("Use Ink external functions instead")]
        public void ReportReachArea(string areaId)
        {
            Log($"Legacy ReportReachArea called: {areaId} - Consider using Ink external functions");
            // For now, just log - Ink should handle this via external functions
        }
    }
}