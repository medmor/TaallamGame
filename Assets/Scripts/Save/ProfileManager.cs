using System;
using System.Collections;
using UnityEngine;

namespace Taallam.Save
{
    public class ProfileManager : MonoBehaviour
    {
        public static ProfileManager Instance { get; private set; }
        public static SaveFile Current { get; private set; }

        public static event Action<SaveFile> BeforeSave;  // capture state into Current here
        public static event Action<SaveFile> AfterLoad;   // apply loaded state to scene

        [SerializeField] private string defaultChildName = "لاعب";

        private const string LastProfileKey = "lastProfileId";

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

        private void Start()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            StartCoroutine(StartupRoutine());
#else
            Bootstrap();
#endif
        }

        private IEnumerator StartupRoutine()
        {
            WebGLFileSystem.RequestLoad();
            yield return null; // allow IDBFS mount
            Bootstrap();
        }

        private void Bootstrap()
        {
            var last = PlayerPrefs.GetString(LastProfileKey, string.Empty);
            if (!string.IsNullOrEmpty(last) && SaveSystem.TryLoad(last, out var loaded))
            {
                Current = loaded;
                AfterLoad?.Invoke(Current);
            }
            else
            {
                var created = NewProfile(defaultChildName);
                SelectProfile(created.profileId);
            }
        }

        public SaveFile NewProfile(string childName)
        {
            var id = System.Guid.NewGuid().ToString("N");
            var file = SaveFile.CreateNew(id, childName);
            SaveSystem.Save(file);
            return file;
        }

        public void SelectProfile(string profileId)
        {
            if (!SaveSystem.TryLoad(profileId, out var file))
            {
                Debug.LogWarning($"Profile {profileId} not found. Creating new.");
                file = NewProfile(defaultChildName);
            }
            Current = file;
            PlayerPrefs.SetString(LastProfileKey, file.profileId);
            PlayerPrefs.Save();
            AfterLoad?.Invoke(Current);
        }

        public void Save()
        {
            if (Current == null) return;
            BeforeSave?.Invoke(Current);
            Current.lastPlayedUtc = System.DateTime.UtcNow.ToString("o");
            SaveSystem.Save(Current);
        }
    }
}
