using System.Collections.Generic;
using UnityEngine;

namespace TaallamGame.Dialogue
{
    [CreateAssetMenu(menuName = "TaallamGame/Dialogue/Portrait Library", fileName = "PortraitLibrary")]
    public class PortraitLibrary : ScriptableObject
    {
        [System.Serializable]
        public struct Entry
        {
            public string id;      // e.g. "ahmed_energetic"
            public Sprite sprite;  // portrait sprite
        }

        [SerializeField] private List<Entry> entries = new List<Entry>();
        private Dictionary<string, Sprite> _map;

        void OnEnable()
        {
            _map = new Dictionary<string, Sprite>();
            foreach (var e in entries)
            {
                if (!string.IsNullOrEmpty(e.id) && e.sprite != null)
                    _map[e.id] = e.sprite;
            }
        }

        public bool TryGet(string id, out Sprite sprite)
        {
            if (_map == null) OnEnable();
            return _map.TryGetValue(id, out sprite);
        }
    }
}