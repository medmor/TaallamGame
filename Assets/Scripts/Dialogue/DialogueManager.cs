using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TaallamGame.Player;
using TaallamGame.Missions;
using RTLTMPro;
using System.Text.RegularExpressions;

namespace TaallamGame.Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        [Header("Params")]
        [SerializeField] private float typingSpeed = 0.04f;
        [SerializeField] private bool debugLogging = false; // enable to print detailed logs

        [Header("Load Globals JSON")]
        [SerializeField] private TextAsset loadGlobalsJSON;

        [Header("Dialogue UI")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private GameObject continueIcon;
        [SerializeField] private RTLTextMeshPro dialogueText;
        [SerializeField] private RTLTextMeshPro displayNameText;
        private Animator layoutAnimator;

        [Header("Choices UI")]
        [SerializeField] private GameObject choicePrefab; // Prefab for creating choices dynamically
        [SerializeField] private Transform choiceContainer; // Parent container for choices (e.g., VerticalLayoutGroup)
        [SerializeField] private int maxChoices = 10; // Optional limit for safety

        private List<GameObject> activeChoices = new List<GameObject>(); // Track created choices

        [Header("Audio")]
        [SerializeField] private DialogueAudioInfoSO defaultAudioInfo;
        [SerializeField] private DialogueAudioInfoSO[] audioInfos;
        [SerializeField] private bool makePredictable;
        private DialogueAudioInfoSO currentAudioInfo;
        private Dictionary<string, DialogueAudioInfoSO> audioInfoDictionary;
        private AudioSource audioSource;

    private Story currentStory;
    public Story CurrentStory => currentStory; // expose for bridges (read-only)
        public bool dialogueIsPlaying { get; private set; }

        private bool canContinueToNextLine = false;

        private Coroutine displayLineCoroutine;
        private Coroutine typingCoroutine;
        private string currentStoryText = "";

        private static DialogueManager instance;

        private const string SPEAKER_TAG = "speaker";
        private const string PORTRAIT_TAG = "portrait";
        private const string LAYOUT_TAG = "layout";
        private const string AUDIO_TAG = "audio";

        private DialogueVariables dialogueVariables;
        private InkExternalFunctions inkExternalFunctions;

        [SerializeField] private PlayerInputHandler input;
        [SerializeField] private PortraitController portraitController;

        [Header("Re-entry Control")]
        [SerializeField] private float reentryCooldownSeconds = 0.25f; // block re-open after close
        private float _reentryBlockUntil = -1f;

        // Emits when a dialogue finishes (after UI hides)
        public event Action DialogueEnded;

        public static DialogueManager GetInstance() => instance;

        private void DLog(string msg)
        {
            if (debugLogging) Debug.Log($"[Dialogue] {msg}");
        }

        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogWarning("Found more than one Dialogue Manager in the scene");
            }
            instance = this;

            dialogueVariables = new DialogueVariables(loadGlobalsJSON);
            inkExternalFunctions = new InkExternalFunctions();

            audioSource = this.gameObject.AddComponent<AudioSource>();
            currentAudioInfo = defaultAudioInfo;

            if (!input)
                input = FindFirstObjectByType<PlayerInputHandler>();
        }

        private void Start()
        {
            dialogueIsPlaying = false;
            dialoguePanel.SetActive(false);

            // get the layout animator
            layoutAnimator = dialoguePanel.GetComponent<Animator>();

            // Validate choice setup
            if (choicePrefab == null)
            {
                Debug.LogError("DialogueManager: choicePrefab is not assigned!");
            }
            if (choiceContainer == null)
            {
                Debug.LogError("DialogueManager: choiceContainer is not assigned!");
            }

            InitializeAudioInfoDictionary();
            DLog("Initialized DialogueManager");
        }

        private void InitializeAudioInfoDictionary()
        {
            audioInfoDictionary = new Dictionary<string, DialogueAudioInfoSO>();
            audioInfoDictionary.Add(defaultAudioInfo.id, defaultAudioInfo);
            foreach (DialogueAudioInfoSO audioInfo in audioInfos)
            {
                audioInfoDictionary.Add(audioInfo.id, audioInfo);
            }
        }

        private void SetCurrentAudioInfo(string id)
        {
            if (audioInfoDictionary.TryGetValue(id, out var audioInfo))
            {
                this.currentAudioInfo = audioInfo;
            }
            else
            {
                Debug.LogWarning("Failed to find audio info for id: " + id);
            }
        }

        private void Update()
        {
            if (!dialogueIsPlaying)
            {
                return;
            }

            if (input != null && input.ConsumeInteractPressed())
            {
                int choiceCount = currentStory.currentChoices.Count;
                DLog($"Interact pressed in Update. canContinue={canContinueToNextLine}, choices={choiceCount}, typing={(displayLineCoroutine != null)}");

                // If typing is in progress, complete it immediately
                if (!canContinueToNextLine && displayLineCoroutine != null)
                {
                    DLog("Completing typing immediately");
                    StopCoroutine(displayLineCoroutine);
                    displayLineCoroutine = null;
                    dialogueText.maxVisibleCharacters = currentStoryText.Length;

                    // Only show continue icon if there's more content OR choices
                    bool hasMoreContent = currentStory.canContinue || currentStory.currentChoices.Count > 0;
                    continueIcon.SetActive(hasMoreContent && currentStory.currentChoices.Count == 0);
                    DisplayChoices();
                    canContinueToNextLine = true;
                    DLog("Typing completed, canContinueToNextLine set to true");
                    return;
                }

                // If line is complete and no choices, advance to next line
                if (canContinueToNextLine && choiceCount == 0)
                {
                    DLog($"Advancing story from Update - story.canContinue={currentStory.canContinue}");
                    ContinueStory();
                }
                else if (choiceCount > 0)
                {
                    DLog("Choices are present; Interact will not advance. Use Submit/Click to choose.");
                }
                else
                {
                    DLog($"Input consumed but conditions not met for advance. canContinueToNextLine={canContinueToNextLine}, choiceCount={choiceCount}");
                }
            }
        }

        public bool IsInReentryCooldown => Time.time < _reentryBlockUntil;

        public void EnterDialogueMode(TextAsset inkJSON, Animator emoteAnimator)
        {
            if (inkJSON == null) { DLog("EnterDialogueMode called with null inkJSON"); return; }

            // Prevent immediate re-open due to the same key press
            if (IsInReentryCooldown)
            {
                DLog($"EnterDialogueMode blocked by cooldown ({_reentryBlockUntil - Time.time:0.00}s left)");
                return;
            }

            // If dialogue is already playing, don't restart it
            if (dialogueIsPlaying)
            {
                DLog("EnterDialogueMode called but dialogue is already playing - ignoring");
                return;
            }

            DLog($"EnterDialogueMode: {inkJSON.name}");
            currentStory = new Story(inkJSON.text);
            // Allow externals to fallback and bind teleport_to immediately
            currentStory.allowExternalFunctionFallbacks = true;
            try
            {
                var tpNow = FindFirstObjectByType<TaallamGame.Dialogue.TeleportBridge>();
                if (tpNow != null)
                {
                    currentStory.BindExternalFunction("teleport_to", new System.Action<string>(tpNow.TeleportTo));
                }
                else
                {
                    // Bind a placeholder to avoid validation failure; TeleportBridge can rebind later
                    currentStory.BindExternalFunction("teleport_to", new System.Action<string>((target) =>
                    {
                        Debug.LogWarning($"teleport_to called for '{target}' but no TeleportBridge found yet.");
                    }));
                }
            }
            catch { }

            // Bind external functions for mission integration
            var missionBridge = InkMissionBridge.Instance;
            if (missionBridge != null)
            {
                missionBridge.BindExternalFunctions(currentStory);
                // Load any saved story state
                missionBridge.LoadStoryState(currentStory, inkJSON.name);
            }

            dialogueIsPlaying = true;
            dialoguePanel.SetActive(true);

            dialogueVariables.StartListening(currentStory);
            inkExternalFunctions.Bind(currentStory, emoteAnimator);

            // Ensure critical EXTERNALS like teleport_to are bound before first Continue
            var tp = FindFirstObjectByType<TaallamGame.Dialogue.TeleportBridge>();
            if (tp != null) tp.BindForStory(currentStory);

            displayNameText.text = "???";
            layoutAnimator.Play("right");

            DLog("Starting first line");
            ContinueStory();
        }

        private IEnumerator ExitDialogueMode()
        {
            // Start cooldown immediately so re-open is blocked while we close
            _reentryBlockUntil = Time.time + reentryCooldownSeconds;

            yield return new WaitForSeconds(0.2f);

            dialogueVariables.StopListening(currentStory);
            inkExternalFunctions.Unbind(currentStory);

            // Clean up dynamic choices
            foreach (GameObject choice in activeChoices)
            {
                if (choice != null)
                {
                    Destroy(choice);
                }
            }
            activeChoices.Clear();

            dialogueIsPlaying = false;
            dialoguePanel.SetActive(false);
            dialogueText.text = "";

            SetCurrentAudioInfo(defaultAudioInfo.id);

            try { DLog("Dialogue ended"); DialogueEnded?.Invoke(); } catch (Exception ex) { Debug.LogException(ex); }
        }

        private void ContinueStory()
        {
            DLog($"ContinueStory called - canContinue: {currentStory?.canContinue}, currentChoices: {currentStory?.currentChoices?.Count}");

            if (currentStory == null)
            {
                DLog("ERROR: currentStory is null in ContinueStory!");
                return;
            }

            if (currentStory.canContinue)
            {
                if (displayLineCoroutine != null)
                {
                    StopCoroutine(displayLineCoroutine);
                }
                string nextLine = currentStory.Continue();
                DLog($"Retrieved next line: '{nextLine}' (length={nextLine?.Length ?? 0}), canContinue(after pull)={currentStory.canContinue}");

                if (string.IsNullOrEmpty(nextLine) && !currentStory.canContinue)
                {
                    DLog("Empty line and no more content; exiting dialogue");
                    StartCoroutine(ExitDialogueMode());
                }
                else
                {
                    HandleTags(currentStory.currentTags);
                    displayLineCoroutine = StartCoroutine(DisplayLine(nextLine));
                }
            }
            else
            {
                DLog($"No more content; exiting dialogue. currentChoices.Count={currentStory.currentChoices.Count}");
                StartCoroutine(ExitDialogueMode());
            }
        }

        private IEnumerator DisplayLine(string line)
        {
            currentStoryText = line; // Store the full text
            dialogueText.text = line;
            dialogueText.maxVisibleCharacters = 0;
            continueIcon.SetActive(false);
            HideChoices();

            canContinueToNextLine = false;
            DLog($"Typing line length={line?.Length ?? 0}");

            bool isAddingRichTextTag = false;

            foreach (char letter in line)
            {
                if (input && input.ConsumeInteractPressed())
                {
                    dialogueText.maxVisibleCharacters = line.Length;
                    DLog("Reveal full line (skip typewriter)");
                    break;
                }

                if (letter == '<' || isAddingRichTextTag)
                {
                    isAddingRichTextTag = true;
                    if (letter == '>')
                    {
                        isAddingRichTextTag = false;
                    }
                }
                else
                {
                    int charIndex = dialogueText.maxVisibleCharacters;
                    if (charIndex < dialogueText.text.Length)
                    {
                        PlayDialogueSound(charIndex, dialogueText.text[charIndex]);
                        dialogueText.maxVisibleCharacters = charIndex + 1;
                    }
                    yield return new WaitForSeconds(typingSpeed);
                }
            }

            // Only show continue icon if there's more content OR choices
            bool hasMoreContent = currentStory.canContinue || currentStory.currentChoices.Count > 0;
            continueIcon.SetActive(hasMoreContent && currentStory.currentChoices.Count == 0);
            DisplayChoices();

            canContinueToNextLine = true;
            DLog($"Line finished. choices={currentStory.currentChoices.Count}; canContinueToNextLine=true; hasMoreContent={hasMoreContent}; story.canContinue={currentStory.canContinue}");
        }

        private void PlayDialogueSound(int currentDisplayedCharacterCount, char currentCharacter)
        {
            AudioClip[] dialogueTypingSoundClips = currentAudioInfo.dialogueTypingSoundClips;
            int frequencyLevel = currentAudioInfo.frequencyLevel;
            float minPitch = currentAudioInfo.minPitch;
            float maxPitch = currentAudioInfo.maxPitch;
            bool stopAudioSource = currentAudioInfo.stopAudioSource;

            if (currentDisplayedCharacterCount % frequencyLevel == 0)
            {
                if (stopAudioSource)
                {
                    audioSource.Stop();
                }
                AudioClip soundClip = null;
                if (makePredictable)
                {
                    int hashCode = currentCharacter.GetHashCode();
                    int predictableIndex = hashCode % dialogueTypingSoundClips.Length;
                    soundClip = dialogueTypingSoundClips[predictableIndex];
                    int minPitchInt = (int)(minPitch * 100);
                    int maxPitchInt = (int)(maxPitch * 100);
                    int pitchRangeInt = maxPitchInt - minPitchInt;
                    if (pitchRangeInt != 0)
                    {
                        int predictablePitchInt = (hashCode % pitchRangeInt) + minPitchInt;
                        float predictablePitch = predictablePitchInt / 100f;
                        audioSource.pitch = predictablePitch;
                    }
                    else
                    {
                        audioSource.pitch = minPitch;
                    }
                }
                else
                {
                    int randomIndex = UnityEngine.Random.Range(0, dialogueTypingSoundClips.Length);
                    soundClip = dialogueTypingSoundClips[randomIndex];
                    audioSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
                }

                audioSource.PlayOneShot(soundClip);
            }
        }

        private void HideChoices()
        {
            foreach (GameObject choiceButton in activeChoices)
            {
                if (choiceButton != null)
                {
                    choiceButton.SetActive(false);
                }
            }
        }

        private void HandleTags(List<string> tags)
        {
            foreach (string rawTag in tags)
            {
                var splitTag = rawTag.Split(':');
                if (splitTag.Length != 2) continue;

                string tagKey = splitTag[0].Trim().ToLowerInvariant();
                string tagVal = splitTag[1].Trim();

                switch (tagKey)
                {
                    case SPEAKER_TAG:
                        displayNameText.text = tagVal;
                        break;
                    case "portrait":
                        // old: portraitAnimator.Play(tagVal);
                        if (portraitController != null)
                            portraitController.SetPortrait(tagVal);
                        break;
                    case LAYOUT_TAG:
                        layoutAnimator.Play(tagVal);
                        break;
                    case AUDIO_TAG:
                        SetCurrentAudioInfo(tagVal);
                        break;
                    default:
                        Debug.LogWarning("Tag came in but is not currently being handled: " + rawTag);
                        break;
                }
            }
        }

        private void DisplayChoices()
        {
            List<Choice> currentChoices = currentStory.currentChoices;

            // Check if we exceed our safety limit
            if (currentChoices.Count > maxChoices)
            {
                Debug.LogError($"More choices ({currentChoices.Count}) than max allowed ({maxChoices}). Consider increasing maxChoices or reducing dialogue choices.");
                // Truncate to max choices to prevent issues
                currentChoices = currentChoices.Take(maxChoices).ToList();
            }

            // Clear existing choices
            foreach (GameObject choice in activeChoices)
            {
                if (choice != null)
                {
                    Destroy(choice);
                }
            }
            activeChoices.Clear();

            // Create new choices dynamically
            for (int i = 0; i < currentChoices.Count; i++)
            {
                Choice choice = currentChoices[i];

                // Instantiate choice from prefab
                GameObject choiceObj = Instantiate(choicePrefab, choiceContainer);
                activeChoices.Add(choiceObj);

                // Set choice text
                RTLTextMeshPro choiceText = choiceObj.GetComponentInChildren<RTLTextMeshPro>();
                if (choiceText != null)
                {
                    choiceText.text = choice.text;
                }
                else
                {
                    Debug.LogError($"Choice prefab missing RTLTextMeshPro component!");
                }

                // Set up button functionality
                Button btn = choiceObj.GetComponent<Button>();
                if (btn != null)
                {
                    btn.interactable = true;
                    btn.onClick.RemoveAllListeners();
                    int choiceIndex = i; // Capture for closure
                    btn.onClick.AddListener(() => MakeChoice(choiceIndex));
                }
                else
                {
                    DLog($"Choice object '{choiceObj.name}' has no Button component; mouse click won't work.");
                }

                choiceObj.SetActive(true);
            }

            DLog($"DisplayChoices: {currentChoices.Count} choices created dynamically");
            StartCoroutine(SelectFirstChoice());
        }

        private IEnumerator SelectFirstChoice()
        {
            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                yield return new WaitForEndOfFrame();

                GameObject firstActive = null;
                foreach (GameObject choice in activeChoices)
                {
                    if (choice != null && choice.activeSelf)
                    {
                        firstActive = choice;
                        break;
                    }
                }

                if (firstActive != null)
                {
                    EventSystem.current.SetSelectedGameObject(firstActive);
                    DLog($"Selected first active choice: {firstActive.name}");
                }
            }
            else
            {
                yield return null;
            }
        }

        public void MakeChoice(int choiceIndex)
        {
            DLog($"MakeChoice called index={choiceIndex}, canContinue={canContinueToNextLine}");
            if (!canContinueToNextLine) { DLog("MakeChoice ignored: not ready"); return; }
            currentStory.ChooseChoiceIndex(choiceIndex);
            DLog("Choice accepted; continuing story");
            ContinueStory();
        }

        public Ink.Runtime.Object GetVariableState(string variableName)
        {
            Ink.Runtime.Object variableValue = null;
            dialogueVariables.variables.TryGetValue(variableName, out variableValue);
            if (variableValue == null)
            {
                Debug.LogWarning("Ink Variable was found to be null: " + variableName);
            }
            return variableValue;
        }

        public void OnApplicationQuit()
        {
            dialogueVariables.SaveVariables();
        }

        // Removed bidi control wrappers to avoid runtime insertion of invisible characters.
    }
}
