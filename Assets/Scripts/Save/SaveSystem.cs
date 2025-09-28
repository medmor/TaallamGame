using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Taallam.Save
{
    public static class SaveSystem
    {
        private static string Root => Path.Combine(Application.persistentDataPath, "saves");
        private static string PathFor(string profileId) => Path.Combine(Root, $"{profileId}.json");

        public static void Save(SaveFile data)
        {
            Directory.CreateDirectory(Root);
            string path = PathFor(data.profileId);
            string tmp = path + ".tmp";

            string json = JsonUtility.ToJson(data, prettyPrint: true);
            File.WriteAllText(tmp, json, new UTF8Encoding(false));

#if UNITY_WEBGL && !UNITY_EDITOR
            if (File.Exists(path)) File.Delete(path);
            File.Move(tmp, path);
            WebGLFileSystem.RequestSync();
#else
            if (File.Exists(path)) File.Replace(tmp, path, null);
            else File.Move(tmp, path);
#endif
        }

        public static bool TryLoad(string profileId, out SaveFile data)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            WebGLFileSystem.RequestLoad();
#endif
            string path = PathFor(profileId);
            if (!File.Exists(path))
            {
                data = null;
                return false;
            }
            string json = File.ReadAllText(path, Encoding.UTF8);
            data = JsonUtility.FromJson<SaveFile>(json);
            return data != null;
        }

        public static IEnumerable<string> ListProfiles()
        {
            if (!Directory.Exists(Root)) yield break;
            foreach (var f in Directory.GetFiles(Root, "*.json"))
                yield return Path.GetFileNameWithoutExtension(f);
        }

        public static void Delete(string profileId)
        {
            string path = PathFor(profileId);
            if (File.Exists(path)) File.Delete(path);
#if UNITY_WEBGL && !UNITY_EDITOR
            WebGLFileSystem.RequestSync();
#endif
        }
    }
}
