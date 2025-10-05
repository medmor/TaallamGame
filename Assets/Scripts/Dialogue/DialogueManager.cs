using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TaallamGame.Player;
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
        [SerializeField] private Animator portraitAnimator;
        private Animator layoutAnimator;

        [Header("Choices UI")]
        [SerializeField] private GameObject[] choices;
    private RTLTextMeshPro[] choicesText;

        [Header("Audio")]
        [SerializeField] private DialogueAudioInfoSO defaultAudioInfo;
        [SerializeField] private DialogueAudioInfoSO[] audioInfos;
        [SerializeField] private bool makePredictable;
        private DialogueAudioInfoSO currentAudioInfo;
        private Dictionary<string, DialogueAudioInfoSO> audioInfoDictionary;
        private AudioSource audioSource;

        private Story currentStory;
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

            // get all of the choices text
            choicesText = new RTLTextMeshPro[choices.Length];
            int index = 0;
            foreach (GameObject choice in choices)
            {
                choicesText[index] = choice.GetComponentInChildren<RTLTextMeshPro>();
                index++;
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
                    DLog("Advancing story from Update");
                    ContinueStory();
                }
                else if (choiceCount > 0)
                {
                    DLog("Choices are present; Interact will not advance. Use Submit/Click to choose.");
                }
                else
                {
                    DLog("Input consumed but conditions not met for advance");
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
            dialogueIsPlaying = true;
            dialoguePanel.SetActive(true);

            dialogueVariables.StartListening(currentStory);
            inkExternalFunctions.Bind(currentStory, emoteAnimator);

            displayNameText.text = "???";
            portraitAnimator.Play("default");
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
                DLog("No more content; exiting dialogue");
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
            DLog($"Line finished. choices={currentStory.currentChoices.Count}; canContinueToNextLine=true; hasMoreContent={hasMoreContent}");
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
            foreach (GameObject choiceButton in choices)
            {
                choiceButton.SetActive(false);
            }
        }

        private void HandleTags(List<string> currentTags)
        {
            foreach (string tag in currentTags)
            {
                string[] splitTag = tag.Split(':');
                if (splitTag.Length != 2)
                {
                    Debug.LogError("Tag could not be appropriately parsed: " + tag);
                }
                string tagKey = splitTag[0].Trim();
                string tagValue = splitTag[1].Trim();

                switch (tagKey)
                {
                    case SPEAKER_TAG:
                        displayNameText.text = tagValue;
                        break;
                    case PORTRAIT_TAG:
                        portraitAnimator.Play(tagValue);
                        break;
                    case LAYOUT_TAG:
                        layoutAnimator.Play(tagValue);
                        break;
                    case AUDIO_TAG:
                        SetCurrentAudioInfo(tagValue);
                        break;
                    default:
                        Debug.LogWarning("Tag came in but is not currently being handled: " + tag);
                        break;
                }
            }
        }

        private void DisplayChoices()
        {
            List<Choice> currentChoices = currentStory.currentChoices;

            if (currentChoices.Count > choices.Length)
            {
                Debug.LogError("More choices were given than the UI can support. Number of choices given: "
                    + currentChoices.Count);
            }

            int index = 0;
            foreach (Choice choice in currentChoices)
            {
                choices[index].gameObject.SetActive(true);
                choicesText[index].text = WrapLatinIfNeeded(choice.text);
                var btn = choices[index].GetComponent<Button>();
                if (btn != null)
                {
                    btn.interactable = true;
                    btn.onClick.RemoveAllListeners();
                    int captured = index;
                    btn.onClick.AddListener(() => MakeChoice(captured));
                }
                else
                {
                    DLog($"Choice object '{choices[index].name}' has no Button component; mouse click won't work.");
                }
                index++;
            }
            for (int i = index; i < choices.Length; i++)
            {
                choices[i].gameObject.SetActive(false);
            }

            DLog($"DisplayChoices: {currentChoices.Count} choices");
            StartCoroutine(SelectFirstChoice());
        }

        private IEnumerator SelectFirstChoice()
        {
            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                yield return new WaitForEndOfFrame();

                GameObject firstActive = null;
                for (int i = 0; i < choices.Length; i++)
                {
                    if (choices[i] != null && choices[i].activeSelf)
                    {
                        firstActive = choices[i];
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

        private static readonly Regex LatinRun = new Regex("[A-Za-z0-9]", RegexOptions.Compiled);
        private static string WrapLatinIfNeeded(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            // If the string contains Latin, wrap to keep LTR inside RTL context.
            return LatinRun.IsMatch(s) ? "\u2066" + s + "\u2069" : s; // LRI ... PDI
        }
    }
}
