using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace TaallamGame.Dialogue
{
    public class TeleportBridge : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private DialogueManager dialogueManager; // assign
        [SerializeField] private Transform player;                 // assign
        [SerializeField] private bool debugLogs = false;

        [Header("Fade Effect")]
        [SerializeField] private CanvasGroup fadePanel;           // assign a black Image with CanvasGroup
        [SerializeField] private float fadeInSpeed = 2f;          // How fast to fade to black (higher = faster)
        [SerializeField] private float fadeOutSpeed = 2f;         // How fast to fade back in (higher = faster)
        [SerializeField] private float holdAtBlackTime = 0.5f;    // How long to stay fully black (seconds)
        
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
                try 
                { 
                    story.UnbindExternalFunction("teleport_to"); 
                }
                catch { }
                try 
                { 
                    story.UnbindExternalFunction("fade_to_black"); 
                }
                catch { }
                if (debugLogs) Debug.Log("[TeleportBridge] Unbound externals on disable");
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
            try { story.UnbindExternalFunction("teleport_to"); } catch { }
            try { story.UnbindExternalFunction("fade_to_black"); } catch { }
            
            story.BindExternalFunction("teleport_to", new System.Action<string>(TeleportTo));
            story.BindExternalFunction("fade_to_black", new System.Action<int>(FadeToBlack));
            if (debugLogs) Debug.Log("[TeleportBridge] Bound EXTERNAL teleport_to and fade_to_black to current Story");
        }

        // Allow DialogueManager to bind immediately upon Story creation
        public void BindForStory(Story s)
        {
            if (s == null) return;
            story = s;
            try { story.UnbindExternalFunction("teleport_to"); } catch { }
            try { story.UnbindExternalFunction("fade_to_black"); } catch { }
            
            story.BindExternalFunction("teleport_to", new System.Action<string>(TeleportTo));
            story.BindExternalFunction("fade_to_black", new System.Action<int>(FadeToBlack));
            if (debugLogs) Debug.Log("[TeleportBridge] Force-bound EXTERNAL teleport_to and fade_to_black to new Story");
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

        public void FadeToBlack(int durationSeconds)
        {
            if (fadePanel == null)
            {
                Debug.LogWarning("[TeleportBridge] FadePanel not assigned, cannot fade to black");
                return;
            }
            StartCoroutine(FadeToBlackCoroutine(durationSeconds));
        }

        private IEnumerator FadeToBlackCoroutine(int durationSeconds)
        {
            if (fadePanel == null) yield break;

            // Ensure fade panel is active
            fadePanel.gameObject.SetActive(true);
            
            // Phase 1: Fade to black
            float elapsed = 0f;
            while (fadePanel.alpha < 1f)
            {
                elapsed += Time.deltaTime * fadeInSpeed;
                fadePanel.alpha = Mathf.Clamp01(elapsed);
                yield return null;
            }
            fadePanel.alpha = 1f;

            // Phase 2: Stay black (hold time)
            yield return new WaitForSeconds(holdAtBlackTime);

            // Phase 3: Fade back in
            elapsed = 1f;
            while (fadePanel.alpha > 0f)
            {
                elapsed -= Time.deltaTime * fadeOutSpeed;
                fadePanel.alpha = Mathf.Clamp01(elapsed);
                yield return null;
            }
            fadePanel.alpha = 0f;

            // Deactivate panel to avoid blocking input
            fadePanel.gameObject.SetActive(false);
            
            if (debugLogs) Debug.Log($"[TeleportBridge] Fade to black completed");
        }
    }

    [System.Serializable]
    public class TeleportPoint
    {
        public string id;
        public Transform spawn;
    }
}
