using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TaallamGame.Player;

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
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI displayNameText;
    [SerializeField] private Animator portraitAnimator;
    private Animator layoutAnimator;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

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

    private static DialogueManager instance;

    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";
    private const string LAYOUT_TAG = "layout";
    private const string AUDIO_TAG = "audio";

    private DialogueVariables dialogueVariables;
    private InkExternalFunctions inkExternalFunctions;

    [SerializeField] private PlayerInputHandler input;

    // Emits when a dialogue finishes (after UI hides)
    public event Action DialogueEnded;

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

    public static DialogueManager GetInstance() 
    {
        return instance;
    }

    private void Start() 
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);

        // get the layout animator
        layoutAnimator = dialoguePanel.GetComponent<Animator>();

        // get all of the choices text 
        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices) 
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
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
        DialogueAudioInfoSO audioInfo = null;
        audioInfoDictionary.TryGetValue(id, out audioInfo);
        if (audioInfo != null) 
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
        // return right away if dialogue isn't playing
        if (!dialogueIsPlaying) 
        {
            return;
        }

        // Only advance when there are no choices on screen
        if (input != null && input.ConsumeInteractPressed())
        {
            int choiceCount = currentStory.currentChoices.Count;
            DLog($"Interact pressed in Update. canContinue={canContinueToNextLine}, choices={choiceCount}");
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
                DLog("Line still typing; input consumed but not advancing.");
            }
        }
    }

    public void EnterDialogueMode(TextAsset inkJSON, Animator emoteAnimator) 
    {
        if (inkJSON == null) { DLog("EnterDialogueMode called with null inkJSON"); return; }
        DLog($"EnterDialogueMode: {inkJSON.name}");
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        dialogueVariables.StartListening(currentStory);
        inkExternalFunctions.Bind(currentStory, emoteAnimator);

        // reset portrait, layout, and speaker
        displayNameText.text = "???";
        portraitAnimator.Play("default");
        layoutAnimator.Play("right");

        DLog("Starting first line");
        ContinueStory();
    }

    private IEnumerator ExitDialogueMode() 
    {
        yield return new WaitForSeconds(0.2f);

        dialogueVariables.StopListening(currentStory);
        inkExternalFunctions.Unbind(currentStory);

        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";

        // go back to default audio
        SetCurrentAudioInfo(defaultAudioInfo.id);

        // Notify listeners that dialogue ended
        try { DLog("Dialogue ended"); DialogueEnded?.Invoke(); } catch (Exception ex) { Debug.LogException(ex); }
    }

    private void ContinueStory() 
    {
        DLog("ContinueStory called");
        if (currentStory.canContinue) 
        {
            // set text for the current dialogue line
            if (displayLineCoroutine != null) 
            {
                StopCoroutine(displayLineCoroutine);
            }
            string nextLine = currentStory.Continue();
            DLog($"Next line length={nextLine?.Length ?? 0}, canContinue(after pull)={currentStory.canContinue}");
            // handle case where the last line is an external function
            if (nextLine.Equals("") && !currentStory.canContinue)
            {
                DLog("Last step was external function; exiting dialogue");
                StartCoroutine(ExitDialogueMode());
            }
            // otherwise, handle the normal case for continuing the story
            else 
            {
                // handle tags
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
        // set the text to the full line, but set the visible characters to 0
        dialogueText.text = line;
        dialogueText.maxVisibleCharacters = 0;
        // hide items while text is typing
        continueIcon.SetActive(false);
        HideChoices();

        canContinueToNextLine = false;
        DLog($"Typing line length={line?.Length ?? 0}");

        bool isAddingRichTextTag = false;

        // display each letter one at a time
        foreach (char letter in line)
        {
            if (input && input.ConsumeInteractPressed())
            {
                dialogueText.maxVisibleCharacters = line.Length;
                DLog("Reveal full line (skip typewriter)");
                break;
            }

            // check for rich text tag, if found, add it without waiting
            if (letter == '<' || isAddingRichTextTag) 
            {
                isAddingRichTextTag = true;
                if (letter == '>')
                {
                    isAddingRichTextTag = false;
                }
            }
            // if not rich text, add the next letter and wait a small time
            else 
            {
                // Safely compute the next character index to avoid IndexOutOfRange
                int charIndex = dialogueText.maxVisibleCharacters;
                if (charIndex < dialogueText.text.Length)
                {
                    PlayDialogueSound(charIndex, dialogueText.text[charIndex]);
                    dialogueText.maxVisibleCharacters = charIndex + 1;
                }
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        // actions to take after the entire line has finished displaying
        continueIcon.SetActive(true);
        DisplayChoices();

        canContinueToNextLine = true;
        DLog($"Line finished. choices={currentStory.currentChoices.Count}; canContinueToNextLine=true");
    }

    private void PlayDialogueSound(int currentDisplayedCharacterCount, char currentCharacter)
    {
        // set variables for the below based on our config
        AudioClip[] dialogueTypingSoundClips = currentAudioInfo.dialogueTypingSoundClips;
        int frequencyLevel = currentAudioInfo.frequencyLevel;
        float minPitch = currentAudioInfo.minPitch;
        float maxPitch = currentAudioInfo.maxPitch;
        bool stopAudioSource = currentAudioInfo.stopAudioSource;

        // play the sound based on the config
        if (currentDisplayedCharacterCount % frequencyLevel == 0)
        {
            if (stopAudioSource) 
            {
                audioSource.Stop();
            }
            AudioClip soundClip = null;
            // create predictable audio from hashing
            if (makePredictable) 
            {
                int hashCode = currentCharacter.GetHashCode();
                // sound clip
                int predictableIndex = hashCode % dialogueTypingSoundClips.Length;
                soundClip = dialogueTypingSoundClips[predictableIndex];
                // pitch
                int minPitchInt = (int) (minPitch * 100);
                int maxPitchInt = (int) (maxPitch * 100);
                int pitchRangeInt = maxPitchInt - minPitchInt;
                // cannot divide by 0, so if there is no range then skip the selection
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
            // otherwise, randomize the audio
            else 
            {
                // sound clip
                int randomIndex = UnityEngine.Random.Range(0, dialogueTypingSoundClips.Length);
                soundClip = dialogueTypingSoundClips[randomIndex];
                // pitch
                audioSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
            }
            
            // play sound
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
        // loop through each tag and handle it accordingly
        foreach (string tag in currentTags) 
        {
            // parse the tag
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2) 
            {
                Debug.LogError("Tag could not be appropriately parsed: " + tag);
            }
            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();
            
            // handle the tag
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

        // defensive check to make sure our UI can support the number of choices coming in
        if (currentChoices.Count > choices.Length)
        {
            Debug.LogError("More choices were given than the UI can support. Number of choices given: " 
                + currentChoices.Count);
        }

        int index = 0;
        // enable and initialize the choices up to the amount of choices for this line of dialogue
        foreach(Choice choice in currentChoices) 
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            // Auto-bind button click to MakeChoice(index)
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
        // go through the remaining choices the UI supports and make sure they're hidden
        for (int i = index; i < choices.Length; i++) 
        {
            choices[i].gameObject.SetActive(false);
        }

        DLog($"DisplayChoices: {currentChoices.Count} choices");
        StartCoroutine(SelectFirstChoice());
    }

    private IEnumerator SelectFirstChoice() 
    {
        // Event System requires we clear it first, then wait
        // for at least one frame before we set the current selected object.
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            yield return new WaitForEndOfFrame();

            // Pick the first active choice button if available
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

    // This method will get called anytime the application exits.
    // Depending on your game, you may want to save variable state in other places.
    public void OnApplicationQuit() 
    {
        dialogueVariables.SaveVariables();
    }

}
