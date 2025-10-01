using System;
using System.Collections.Generic;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TaallamGame.Dialogue
{
    public class InkDialogueRunner : MonoBehaviour
    {
        public static InkDialogueRunner Instance { get; private set; }

        [Header("UI")]
        [SerializeField] private CanvasGroup root;
        [SerializeField] private TextMeshProUGUI speakerLabel;
        [SerializeField] private TextMeshProUGUI bodyLabel;
        [SerializeField] private Transform choicesParent;
        [SerializeField] private Button choiceButtonPrefab;

        private Story _story;
        private Action _onComplete;
        private readonly List<Button> _choiceButtons = new List<Button>();

        public bool IsRunning => _story != null;

        private void Awake()
        {
            if (Instance && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            Hide();
        }

        public void StartDialogue(TextAsset inkJson, Action onComplete = null)
        {
            if (inkJson == null) { Debug.LogWarning("InkDialogueRunner: no story assigned"); return; }
            _story = new Story(inkJson.text);
            _onComplete = onComplete;
            Show();
            ContinueStory();
        }

        public void ContinueStory()
        {
            if (_story == null) return;

            // If there are choices displayed, do nothing until one is picked.
            if (_story.currentChoices.Count > 0) return;

            // Advance text
            if (_story.canContinue)
            {
                string text = _story.Continue().Trim();
                ApplyTags(_story.currentTags);
                if (bodyLabel) bodyLabel.text = text;
            }

            // Show choices (if any)
            RefreshChoices();

            // Auto-end when finished and no choices
            if (!_story.canContinue && _story.currentChoices.Count == 0)
                EndDialogue();
        }

        private void ApplyTags(List<string> tags)
        {
            // Expect tags like: "speaker: Policeman"
            if (tags == null || tags.Count == 0) return;
            foreach (var tag in tags)
            {
                var idx = tag.IndexOf(':');
                if (idx > 0)
                {
                    var key = tag.Substring(0, idx).Trim().ToLowerInvariant();
                    var val = tag.Substring(idx + 1).Trim();
                    if (key == "speaker" && speakerLabel) speakerLabel.text = val;
                }
            }
        }

        private void RefreshChoices()
        {
            // Clear old
            for (int i = 0; i < _choiceButtons.Count; i++)
                Destroy(_choiceButtons[i].gameObject);
            _choiceButtons.Clear();

            var choices = _story?.currentChoices;
            if (choices == null || choices.Count == 0) return;

            for (int i = 0; i < choices.Count; i++)
            {
                var btn = Instantiate(choiceButtonPrefab, choicesParent);
                _choiceButtons.Add(btn);
                var label = btn.GetComponentInChildren<TextMeshProUGUI>();
                if (label) label.text = choices[i].text;

                int choiceIndex = i;
                btn.onClick.AddListener(() =>
                {
                    _story.ChooseChoiceIndex(choiceIndex);
                    // After choosing, continue to next line
                    if (bodyLabel) bodyLabel.text = "";
                    RefreshChoices(); // clear immediately
                    ContinueStory();
                });
            }
        }

        private void EndDialogue()
        {
            var cb = _onComplete;
            _onComplete = null;
            _story = null;
            Hide();
            cb?.Invoke();
        }

        private void Show()
        {
            if (!root) return;
            root.alpha = 1; root.interactable = true; root.blocksRaycasts = true;
        }

        private void Hide()
        {
            if (!root) return;
            root.alpha = 0; root.interactable = false; root.blocksRaycasts = false;
            if (bodyLabel) bodyLabel.text = "";
            if (speakerLabel) speakerLabel.text = "";
            for (int i = 0; i < _choiceButtons.Count; i++)
                if (_choiceButtons[i]) Destroy(_choiceButtons[i].gameObject);
            _choiceButtons.Clear();
        }

        // Input System "Send Messages" hook. If a PlayerInput is on this object, E will advance.
        private void OnInteract(InputValue _)
        {
            if (IsRunning) ContinueStory();
        }
    }
}