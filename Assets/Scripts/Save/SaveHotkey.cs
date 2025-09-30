using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace TaallamGame.Save
{
    /// <summary>
    /// Simple hotkey to trigger a manual save. Attach to any always-present GameObject (e.g., Systems).
    /// Default key: F5. Optionally require Ctrl.
    /// Works with both the new Input System and the legacy Input Manager.
    /// </summary>
    public class SaveHotkey : MonoBehaviour
    {
        [SerializeField] private bool requireCtrl = false;
        [SerializeField] private bool logOnSave = true;

#if ENABLE_INPUT_SYSTEM
        [SerializeField] private Key saveKey = Key.F5;
#else
        [SerializeField] private KeyCode saveKey = KeyCode.F5;
#endif

        private void Update()
        {
#if ENABLE_INPUT_SYSTEM
            var kb = Keyboard.current;
            if (kb == null) return;
            bool pressed = false;
            try
            {
                var keyCtrl = kb[(Key)saveKey];
                pressed = keyCtrl != null && keyCtrl.wasPressedThisFrame;
            }
            catch { /* indexer may throw if key missing; ignore */ }

            if (requireCtrl)
                pressed &= kb.ctrlKey.isPressed;
#else
            bool pressed = Input.GetKeyDown(saveKey);
            if (requireCtrl)
                pressed &= (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl));
#endif

            if (pressed && ProfileManager.Instance != null)
            {
                ProfileManager.Instance.Save();
                if (logOnSave)
                    Debug.Log("Manual save triggered by hotkey.");
            }
        }
    }
}
