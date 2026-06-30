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
    private string _currentFullText = string.Empty;
    private Coroutine _typingCoroutine;
    private float _choiceStartTime;
    private float _baseSpeakerFontSize;
    private float _baseBodyFontSize;

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
    }

    void Start()
    {
        if (continueIcon != null)
        {
            continueIcon.SetActive(false);
        }

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

    public void NotifyHoverUI(string thought)
    {
        _isTyping = false;
        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
        }

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
        if (!_isTyping && txtBody != null)
        {
            txtBody.text = _currentFullText;
            txtBody.maxVisibleCharacters = 99999;
        }
    }

    public void SelectThisObject(string id)
    {
        if (story == null || story.currentChoices.Count == 0)
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

                story.ChooseChoiceIndex(index);
                NotifyExitUI();
                DisplayNextLine();
                return;
            }
        }
    }

    public void StartStory()
    {
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
        if (story == null)
        {
            return;
        }

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
            bool hasInvestigation = false;

            foreach (Choice choice in story.currentChoices)
            {
                if (choice.tags == null)
                {
                    continue;
                }

                foreach (string tag in choice.tags)
                {
                    if (tag.StartsWith("id:"))
                    {
                        hasInvestigation = true;
                    }
                }
            }

            if (hasInvestigation)
            {
                Debug.Log("[Ink] Investigation mode active.");
            }

            _choiceStartTime = Time.realtimeSinceStartup;
            ShowChoiceBubbles();
        }
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
            bool isInvestigation = false;
            Choice choice = story.currentChoices[choiceIndex];

            if (choice.tags != null)
            {
                foreach (string tag in choice.tags)
                {
                    if (tag.StartsWith("id:"))
                    {
                        isInvestigation = true;
                    }
                }
            }

            if (isInvestigation || uiButtonIndex >= choiceButtons.Length)
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

    private void OnBubbleClicked(int index)
    {
        if (story == null || index < 0 || index >= story.currentChoices.Count)
        {
            return;
        }

        if (choiceBubblePanel != null)
        {
            choiceBubblePanel.SetActive(false);
        }

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
        if (tags == null)
        {
            return;
        }

        foreach (string tag in tags)
        {
            int separatorIndex = tag.IndexOf(':');
            if (separatorIndex < 0)
            {
                continue;
            }

            string tagKey = tag.Substring(0, separatorIndex).Trim();
            string tagValue = tag.Substring(separatorIndex + 1).Trim();

            if (tagKey == "speaker")
            {
                if (txtSpeaker != null)
                {
                    txtSpeaker.text = tagValue;
                }
            }
            else if (tagKey == "load_scene")
            {
                Time.timeScale = 1f;
                if (GameSystem.Instance != null)
                {
                    GameSystem.Instance.SaveGame(0);
                }

                UnityEngine.SceneManagement.SceneManager.LoadScene(tagValue);
            }
            else if (tagKey == "action")
            {
                if (tagValue == "upload_data")
                {
                    if (TelemetryManager.Instance != null)
                    {
                        TelemetryManager.Instance.UploadDataToServer();
                    }
                }
                else if (tagValue.StartsWith("meta_"))
                {
                    string currentUser = PlayerPrefs.GetString("CurrentUser", "Guest");
                    PlayerPrefs.SetInt($"{currentUser}_{tagValue}", 1);
                    PlayerPrefs.Save();
                }
            }
            else if (tagKey == "portrait" && ScenarioManager.Instance != null)
            {
                ScenarioManager.Instance.ChangePortrait(tagValue);
            }
            else if (tagKey == "show" && ScenarioManager.Instance != null)
            {
                ScenarioManager.Instance.ToggleProp(tagValue, true);
            }
            else if (tagKey == "hide" && ScenarioManager.Instance != null)
            {
                ScenarioManager.Instance.ToggleProp(tagValue, false);
            }
            else if (tagKey == "bg" && ScenarioManager.Instance != null)
            {
                ScenarioManager.Instance.ChangeBG(tagValue);
            }
            else if (tagKey == "bgm" && ScenarioManager.Instance != null)
            {
                ScenarioManager.Instance.ChangeBGM(tagValue);
            }
            else if (tagKey == "sfx" && ScenarioManager.Instance != null)
            {
                ScenarioManager.Instance.PlaySFX(tagValue);
            }
        }
    }

    private IEnumerator SmoothTypeWriter(string text)
    {
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
