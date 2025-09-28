using System;
using System.Collections.Generic;
using UnityEngine;

namespace Taallam.Save
{
    [Serializable]
    public class SaveFile
    {
        public string version = "1";
        public string profileId;
        public string childName;
        public string createdAtUtc;
        public string lastPlayedUtc;

        public PlayerSaveData player = new PlayerSaveData();
        public List<MissionSaveData> missions = new List<MissionSaveData>();
        public SettingsSaveData settings = new SettingsSaveData();

        public static SaveFile CreateNew(string id, string name)
        {
            return new SaveFile
            {
                profileId = id,
                childName = name,
                createdAtUtc = DateTime.UtcNow.ToString("o"),
                lastPlayedUtc = DateTime.UtcNow.ToString("o"),
            };
        }
    }

    [Serializable]
    public class PlayerSaveData
    {
        public string scene = "City";
        public float x;
        public float y;
    }

    [Serializable]
    public class MissionSaveData
    {
        public string id;
        public string state;     // "available", "active", "turnin", "completed"
        public int progress;     // step index or collected count
        public string completedAtUtc;
    }

    [Serializable]
    public class SettingsSaveData
    {
        public string language = "ar";
        public float masterVolume = 1f;
    }
}
