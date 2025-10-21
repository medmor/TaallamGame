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
        [SerializeField] private bool debugLogs = false;

        [Header("Destinations")] 
        [SerializeField] private List<TeleportPoint> points = new();

        private Story story;
        private Dictionary<string, Transform> lookup;

        private void Awake()
        {
            if (!dialogueManager)
                dialogueManager = DialogueManager.GetInstance() ?? FindFirstObjectByType<DialogueManager>();
            BuildLookup();
        }

        private void OnEnable()
        {
            TryBind();
        }

        private void OnDisable()
        {
            if (story != null)
            {
                story.UnbindExternalFunction("teleport_to");
                if (debugLogs) Debug.Log("[TeleportBridge] Unbound teleport_to on disable");
            }
        }

        private void Update()
        {
            // Re-check frequently so we rebind when a new Story is started
            TryBind();
        }

        void TryBind()
        {
            if (!dialogueManager)
            {
                dialogueManager = DialogueManager.GetInstance() ?? FindFirstObjectByType<DialogueManager>();
                if (!dialogueManager) return;
            }
            if (dialogueManager.CurrentStory == null) return;
            if (story == dialogueManager.CurrentStory) return;
            story = dialogueManager.CurrentStory;
            // bind or rebind EXTERNAL
            story.UnbindExternalFunction("teleport_to");
            story.BindExternalFunction("teleport_to", new System.Action<string>(TeleportTo));
            if (debugLogs) Debug.Log("[TeleportBridge] Bound EXTERNAL teleport_to to current Story");
        }

        // Allow DialogueManager to bind immediately upon Story creation
        public void BindForStory(Story s)
        {
            if (s == null) return;
            story = s;
            story.UnbindExternalFunction("teleport_to");
            story.BindExternalFunction("teleport_to", new System.Action<string>(TeleportTo));
            if (debugLogs) Debug.Log("[TeleportBridge] Force-bound EXTERNAL teleport_to to new Story");
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
            if (debugLogs) Debug.Log($"[TeleportBridge] Teleported player to '{id}' at {t.position}");
        }
    }

    [System.Serializable]
    public class TeleportPoint
    {
        public string id;
        public Transform spawn;
    }
}
