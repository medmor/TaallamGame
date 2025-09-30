using System;
using System.Collections.Generic;
using UnityEngine;

namespace TaallamGame.Missions
{
    [CreateAssetMenu(menuName = "Taallam/Mission Definition", fileName = "Mission_")]
    public class MissionDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string id; // unique
        [TextArea] public string title_ar;
        [TextArea] public string summary_ar;
        public string giverNpcId;
        public bool startAvailable = true;

        [Header("Flow")]
        public List<GoalDefinition> goals = new List<GoalDefinition>();
        public RewardDefinition reward = new RewardDefinition();
    }

    public enum GoalType { ReachArea, Interact, Collect, Quiz, MiniGame }

    [Serializable]
    public class GoalDefinition
    {
        public GoalType type;
        public string targetId;        // area id, interactable id, item id, quiz id, etc.
        public int amount = 1;         // for Collect or repeated interactions
        public string payload;         // optional extra (e.g., min score)
        [TextArea] public string hint_ar;
    }

    [Serializable]
    public class RewardDefinition
    {
        public int stars = 1;
        public string badgeId;
    }
}
