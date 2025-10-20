using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;

namespace TaallamGame.Dialogue
{
    public class TeleportBridge : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private DialogueManager dialogueManager; // assign
        [SerializeField] private Transform player;                 // assign

        [Header("Destinations")] 
        [SerializeField] private List<TeleportPoint> points = new();

        private Story story;
        private Dictionary<string, Transform> lookup;

        private void Awake()
        {
            BuildLookup();
        }

        private void OnEnable()
        {
            TryBind();
        }

        private void Update()
        {
            if (story == null) TryBind();
        }

        void TryBind()
        {
            if (!dialogueManager || dialogueManager.CurrentStory == null) return;
            if (story == dialogueManager.CurrentStory) return;
            story = dialogueManager.CurrentStory;
            // bind or rebind EXTERNAL
            story.UnbindExternalFunction("teleport_to");
            story.BindExternalFunction<string>("teleport_to", TeleportTo);
        }

        void BuildLookup()
        {
            lookup = new Dictionary<string, Transform>();
            foreach (var p in points)
            {
                if (p != null && p.spawn != null && !string.IsNullOrWhiteSpace(p.id))
                    lookup[p.id] = p.spawn;
            }
        }

        public void TeleportTo(string id)
        {
            if (!player) return;
            if (lookup == null || !lookup.TryGetValue(id, out var t) || t == null)
            {
                Debug.LogWarning($"[TeleportBridge] Destination not found: {id}");
                return;
            }

            // If player has a CharacterController/Rigidbody, handle properly; here we set position directly
            player.position = t.position;

            // Optional: align facing
            var rb = player.GetComponent<Rigidbody2D>();
            if (rb) { rb.linearVelocity = Vector2.zero; }
        }
    }

    [System.Serializable]
    public class TeleportPoint
    {
        public string id;
        public Transform spawn;
    }
}
