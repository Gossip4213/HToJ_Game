using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;
using Ink.Runtime;

public class DialogueController : MonoBehaviour
{
    [Header("Ink Core - multi-languages")]
    public TextAsset inkJSON_EN;
    public TextAsset inkJSON_ZH;

    [Header("Choices")]
    public GameObject choiceBubblePanel;
    public UnityEngine.UI.Button[] choiceButtons;

    [Header("Ink Core")]
    public TextAsset inkJSONAsset;
    public Story story;

    [Header("UI Components")]
    public TextMeshProUGUI txtSpeaker;
    public TextMeshProUGUI txtBody;
    public GameObject continueIcon;

    [Header("Legacy Fallback Setting")]
    [Tooltip("Used only if no saved text-speed setting is available.")]
    public float typeSpeed = 0.05f;

    private bool _isTyping;
    private bool _sceneInteractionEnabled;
    private bool _hoverThoughtVisible;
    private string _currentFullText = string.Empty;
    private string _currentSpeakerName = string.Empty;
    private Coroutine _typingCoroutine;
    private float _choiceStartTime;
    private float _baseSpeakerFontSize;
    private float _baseBodyFontSize;
    private InkCommandRouter _commandRouter;

    public bool CanInteractWithSceneObjects
    {
        get
        {
            return _sceneInteractionEnabled
                && !_isTyping
                && story != null
                && story.currentChoices.Count > 0;
        }
    }

    void Awake()
    {
        if (txtSpeaker != null)
        {
            _baseSpeakerFontSize = txtSpeaker.fontSize;
        }

        if (txtBody != null)
        {
            _baseBodyFontSize = txtBody.fontSize;
        }

        _commandRouter = new InkCommandRouter(SetSpeaker);
    }

    void Start()
    {
        if (continueIcon != null)
        {
            continueIcon.SetActive(false);
        }

        SetSceneInteractionEnabled(false);
        ApplyTextSettings();
        StartStory();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            OnUserClick();
        }
    }

    private void OnEnable()
    {
        UpdateFonts();
        ApplyTextSettings();

        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged += UpdateFonts;
        }

        SettingsService.OnFontScaleChanged += ApplyFontScale;
    }

    void OnDisable()
    {
        SetSceneInteractionEnabled(false);

        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= UpdateFonts;
        }

        SettingsService.OnFontScaleChanged -= ApplyFontScale;
    }

    private void ApplyTextSettings()
    {
        ApplyFontScale(SettingsService.FontScale);
    }

    private void ApplyFontScale(float scale)
    {
        if (txtSpeaker != null && _baseSpeakerFontSize > 0f)
        {
            txtSpeaker.fontSize = _baseSpeakerFontSize * scale;
        }

        if (txtBody != null && _baseBodyFontSize > 0f)
        {
            txtBody.fontSize = _baseBodyFontSize * scale;
        }
    }

    private void UpdateFonts()
    {
        if (LocalizationManager.Instance == null)
        {
            return;
        }

        TMP_FontAsset globalFont = LocalizationManager.Instance.GetCurrentFont();
        if (globalFont == null)
        {
            return;
        }

        if (txtSpeaker != null)
        {
            txtSpeaker.font = globalFont;
        }

        if (txtBody != null)
        {
            txtBody.font = globalFont;
        }
    }

    private void SetSpeaker(string speakerName)
    {
        _currentSpeakerName = speakerName ?? string.Empty;

        if (!_hoverThoughtVisible && txtSpeaker != null)
        {
            txtSpeaker.text = _currentSpeakerName;
        }

        if (ScenarioManager.Instance != null)
        {
            ScenarioManager.Instance.ChangePortraitForSpeaker(_currentSpeakerName);
        }
    }

    public void NotifyHoverUI(string thought)
    {
        if (!CanInteractWithSceneObjects)
        {
            return;
        }

        _hoverThoughtVisible = true;

        if (txtBody != null)
        {
            txtBody.text = thought;
            txtBody.maxVisibleCharacters = 99999;
        }

        if (txtSpeaker != null)
        {
            txtSpeaker.text = "Thinking";
        }
    }

    public void NotifyExitUI()
    {
        if (!_hoverThoughtVisible)
        {
            return;
        }

        _hoverThoughtVisible = false;

        if (!_isTyping && txtBody != null)
        {
            txtBody.text = _currentFullText;
            txtBody.maxVisibleCharacters = 99999;
        }

        if (txtSpeaker != null)
        {
            txtSpeaker.text = _currentSpeakerName;
        }
    }

    public void SelectThisObject(string id)
    {
        if (!CanInteractWithSceneObjects || string.IsNullOrWhiteSpace(id))
        {
            return;
        }

        float hesitationDuration = Time.realtimeSinceStartup - _choiceStartTime;

        for (int index = 0; index < story.currentChoices.Count; index++)
        {
            Choice choice = story.currentChoices[index];
            if (choice.tags == null)
            {
                continue;
            }

            foreach (string tag in choice.tags)
            {
                if (tag.Trim() != "id:" + id.Trim())
                {
                    continue;
                }

                Debug.Log($"[Telemetry] Investigated [{id}] after {hesitationDuration:F2} seconds.");

                if (TelemetryManager.Instance != null)
                {
                    TelemetryManager.Instance.LogEvent("investigate_object", id, hesitationDuration);
                }

                SetSceneInteractionEnabled(false);
                NotifyExitUI();
                story.ChooseChoiceIndex(index);
                DisplayNextLine();
                return;
            }
        }
    }

    public void StartStory()
    {
        SetSceneInteractionEnabled(false);
        LoadStoryByLanguage();
        DisplayNextLine();
    }

    private void LoadStoryByLanguage()
    {
        string currentLanguage = LocalizationManager.Instance != null
            ? LocalizationManager.Instance.currentLanguage
            : PlayerPrefs.GetString("SelectedLanguage", "EN");

        TextAsset selectedJson = currentLanguage == "ZH_CN" && inkJSON_ZH != null
            ? inkJSON_ZH
            : inkJSON_EN;

        if (selectedJson == null)
        {
            selectedJson = inkJSONAsset;
        }

        if (selectedJson == null)
        {
            Debug.LogError("[Ink] No story JSON is assigned for the selected language.");
            return;
        }

        if (GameSystem.Instance != null && GameSystem.Instance.isLoadingFromSave)
        {
            story = new Story(selectedJson.text);
            string savedState = GameSystem.Instance.CurrentSave != null
                ? GameSystem.Instance.CurrentSave.inkStoryState
                : string.Empty;

            if (!string.IsNullOrEmpty(savedState))
            {
                try
                {
                    story.state.LoadJson(savedState);
                    Debug.Log("[Ink] Save state restored.");
                }
                catch (System.Exception exception)
                {
                    Debug.LogWarning("[Ink] Could not restore save state: " + exception.Message);
                }
            }

            GameSystem.Instance.isLoadingFromSave = false;
        }
        else if (story != null)
        {
            string previousState = story.state.ToJson();
            story = new Story(selectedJson.text);

            try
            {
                story.state.LoadJson(previousState);
            }
            catch (System.Exception exception)
            {
                Debug.LogWarning("[Ink] Story structures differ; previous state was not restored: " + exception.Message);
            }
        }
        else
        {
            story = new Story(selectedJson.text);
        }
    }

    public void DisplayNextLine()
    {
        SetSceneInteractionEnabled(false);

        if (story == null)
        {
            return;
        }

        HideChoiceBubbles();

        if (story.canContinue)
        {
            string cleanText = story.Continue().Trim();
            ParseTags(story.currentTags);

            if (string.IsNullOrWhiteSpace(cleanText))
            {
                DisplayNextLine();
                return;
            }

            if (_typingCoroutine != null)
            {
                StopCoroutine(_typingCoroutine);
            }

            _typingCoroutine = StartCoroutine(SmoothTypeWriter(cleanText));
        }
        else if (story.currentChoices.Count > 0)
        {
            bool hasInvestigation = HasInvestigationChoices();

            if (hasInvestigation)
            {
                Debug.Log("[Ink] Investigation mode active.");
            }

            _choiceStartTime = Time.realtimeSinceStartup;
            ShowChoiceBubbles();
            SetSceneInteractionEnabled(hasInvestigation);
        }
    }

    private bool HasInvestigationChoices()
    {
        if (story == null)
        {
            return false;
        }

        foreach (Choice choice in story.currentChoices)
        {
            if (IsInvestigationChoice(choice))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsInvestigationChoice(Choice choice)
    {
        if (choice == null || choice.tags == null)
        {
            return false;
        }

        foreach (string tag in choice.tags)
        {
            if (!string.IsNullOrWhiteSpace(tag)
                && tag.TrimStart().StartsWith("id:"))
            {
                return true;
            }
        }

        return false;
    }

    private void ShowChoiceBubbles()
    {
        if (choiceBubblePanel == null || choiceButtons == null || story == null)
        {
            return;
        }

        int uiButtonIndex = 0;
        bool hasNormalChoices = false;

        for (int choiceIndex = 0; choiceIndex < story.currentChoices.Count; choiceIndex++)
        {
            Choice choice = story.currentChoices[choiceIndex];

            if (IsInvestigationChoice(choice) || uiButtonIndex >= choiceButtons.Length)
            {
                continue;
            }

            UnityEngine.UI.Button button = choiceButtons[uiButtonIndex];
            button.gameObject.SetActive(true);

            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = choice.text;
            }

            int capturedChoiceIndex = choiceIndex;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnBubbleClicked(capturedChoiceIndex));

            uiButtonIndex++;
            hasNormalChoices = true;
        }

        for (int index = uiButtonIndex; index < choiceButtons.Length; index++)
        {
            choiceButtons[index].gameObject.SetActive(false);
        }

        choiceBubblePanel.SetActive(hasNormalChoices);
    }

    private void HideChoiceBubbles()
    {
        if (choiceBubblePanel != null)
        {
            choiceBubblePanel.SetActive(false);
        }
    }

    private void OnBubbleClicked(int index)
    {
        if (story == null || index < 0 || index >= story.currentChoices.Count)
        {
            return;
        }

        SetSceneInteractionEnabled(false);
        HideChoiceBubbles();

        float hesitationDuration = Time.realtimeSinceStartup - _choiceStartTime;
        string choiceText = story.currentChoices[index].text;

        if (TelemetryManager.Instance != null)
        {
            TelemetryManager.Instance.LogChoiceHesitation(choiceText, hesitationDuration);
        }

        story.ChooseChoiceIndex(index);
        DisplayNextLine();
    }

    private void ParseTags(List<string> tags)
    {
        if (_commandRouter == null)
        {
            _commandRouter = new InkCommandRouter(SetSpeaker);
        }

        _commandRouter.Execute(tags);
    }

    private IEnumerator SmoothTypeWriter(string text)
    {
        SetSceneInteractionEnabled(false);
        _isTyping = true;
        _currentFullText = text;

        if (txtBody != null)
        {
            txtBody.text = text;
            txtBody.ForceMeshUpdate();
            txtBody.maxVisibleCharacters = 0;
        }

        if (continueIcon != null)
        {
            continueIcon.SetActive(false);
        }

        int totalCharacters = txtBody != null ? txtBody.textInfo.characterCount : text.Length;
        float visibleCharacterProgress = 0f;

        while (txtBody != null && txtBody.maxVisibleCharacters < totalCharacters)
        {
            float delay = Mathf.Max(0.001f, SettingsService.TextDelay > 0f ? SettingsService.TextDelay : typeSpeed);
            visibleCharacterProgress += Time.deltaTime / delay;
            txtBody.maxVisibleCharacters = Mathf.Min(
                Mathf.FloorToInt(visibleCharacterProgress),
                totalCharacters);
            yield return null;
        }

        _isTyping = false;
        if (txtBody != null)
        {
            txtBody.maxVisibleCharacters = 99999;
        }

        if (continueIcon != null)
        {
            continueIcon.SetActive(true);
        }
    }

    private void SetSceneInteractionEnabled(bool enabled)
    {
        _sceneInteractionEnabled = enabled;

        if (!enabled)
        {
            NotifyExitUI();
        }
    }

    private void OnUserClick()
    {
        if (_isTyping)
        {
            if (_typingCoroutine != null)
            {
                StopCoroutine(_typingCoroutine);
            }

            if (txtBody != null)
            {
                txtBody.text = _currentFullText;
                txtBody.maxVisibleCharacters = 99999;
            }

            _isTyping = false;
            if (continueIcon != null)
            {
                continueIcon.SetActive(true);
            }
        }
        else
        {
            DisplayNextLine();
        }
    }
}
