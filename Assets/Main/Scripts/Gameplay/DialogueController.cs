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

    [Header("Settings")]
    public float typeSpeed = 0.05f; 

    private bool _isTyping = false;
    private string _currentFullText = "";
    private Coroutine _typingCoroutine;
    private float _choiceStartTime = 0f;
    void Start()
    {
        if (continueIcon != null) continueIcon.SetActive(false);
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
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged += UpdateFonts;
        }

        LoadStoryByLanguage();

        if (!_isTyping && story != null)
        {
            txtBody.text = story.currentText.Trim();
        }
    }

    void OnDisable()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= UpdateFonts;
        }
    }

    private void UpdateFonts()
    {
        if (LocalizationManager.Instance != null)
        {
            TMP_FontAsset globalFont = LocalizationManager.Instance.GetCurrentFont();
            if (globalFont != null)
            {
                if (txtSpeaker != null) txtSpeaker.font = globalFont;
                if (txtBody != null) txtBody.font = globalFont;
            }
        }
    }

    public void NotifyHoverUI(string thought)
    {
        _isTyping = false;
        if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);

        if (txtBody != null)
        {
            txtBody.text = thought;
            txtBody.maxVisibleCharacters = 99999;
        }
        if (txtSpeaker != null) txtSpeaker.text = "Thinking";
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
        
        if (story == null || story.currentChoices.Count == 0) return;

        float hesitationDuration = Time.realtimeSinceStartup - _choiceStartTime;

        for (int i = 0; i < story.currentChoices.Count; i++)
        {
            Choice choice = story.currentChoices[i];
            if (choice.tags != null)
            {
                foreach (string tag in choice.tags)
                {
                    if (tag.Trim() == "id:" + id.Trim())
                    {
                        Debug.Log($"[recording] 玩家在房间里发呆了 {hesitationDuration:F2} 秒后，调查了 [{id}]");

                        if (TelemetryManager.Instance != null)
                        {
                            TelemetryManager.Instance.LogEvent("investigate_object", id, hesitationDuration);
                        }

                        story.ChooseChoiceIndex(i);
                        NotifyExitUI();
                        DisplayNextLine();
                        return;
                    }
                }
            }
        }
    }

    // --- Ink ---

    public void StartStory()
    {
        LoadStoryByLanguage();
        DisplayNextLine();
    }

    private void LoadStoryByLanguage()
    {
        string currentLang = "EN";
        if (LocalizationManager.Instance != null)
        {
            currentLang = LocalizationManager.Instance.currentLanguage;
        }

        TextAsset selectedJSON = (currentLang == "ZH_CN" && inkJSON_ZH != null) ? inkJSON_ZH : inkJSON_EN;

        if (selectedJSON == null)
        {
            Debug.LogError("【Ink 错误】没有找到对应语言story");
            return;
        }
        if (GameSystem.Instance != null && GameSystem.Instance.isLoadingFromSave)
        {
            story = new Story(selectedJSON.text); 
            string savedState = GameSystem.Instance.CurrentSave.inkStoryState;

            if (!string.IsNullOrEmpty(savedState))
            {
                try
                {
                    story.state.LoadJson(savedState);
                    Debug.Log("【观测】完美到存档。");
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning("无法继承存档进度: " + e.Message);
                }
            }
            GameSystem.Instance.isLoadingFromSave = false;
        }
        else if (story != null)
        {
            string savedState = story.state.ToJson();
            story = new Story(selectedJSON.text);
            try
            {
                story.state.LoadJson(savedState);
                Debug.Log("多语言进度已继承");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("剧本结构不一致，无法继承; 错误: " + e.Message);
            }
        }
        else
        {
            story = new Story(selectedJSON.text);
        }
    }

    public void DisplayNextLine()
    {
        if (story.canContinue)
        {
            string cleanText = story.Continue().Trim();
            ParseTags(story.currentTags);

            if (string.IsNullOrWhiteSpace(cleanText))
            {
                DisplayNextLine();
                return;
            }

            if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);

            _typingCoroutine = StartCoroutine(SmoothTypeWriter(cleanText));
        }
        else if (story.currentChoices.Count > 0)
        {
            bool hasInvestigation = false;

            foreach (var choice in story.currentChoices)
            {
                if (choice.tags != null)
                {
                    foreach (string tag in choice.tags)
                    {
                        if (tag.StartsWith("id:")) hasInvestigation = true;
                    }
                }
            }

            if (hasInvestigation)
            {
                Debug.Log("【观测】进入调查模式，等待玩家点击场景物品...");
            }

            _choiceStartTime = Time.realtimeSinceStartup;

            ShowChoiceBubbles();
        }
    }

    private void ShowChoiceBubbles()
    {
        if (choiceBubblePanel == null || choiceButtons == null) return;

        int uiButtonIndex = 0; 
        bool hasNormalChoices = false; 

        for (int i = 0; i < story.currentChoices.Count; i++)
        {
            bool isInvestigation = false;
            if (story.currentChoices[i].tags != null)
            {
                foreach (string tag in story.currentChoices[i].tags)
                {
                    if (tag.StartsWith("id:")) isInvestigation = true;
                }
            }

            if (!isInvestigation)
            {
                if (uiButtonIndex < choiceButtons.Length)
                {
                    choiceButtons[uiButtonIndex].gameObject.SetActive(true);

                    TextMeshProUGUI btnText = choiceButtons[uiButtonIndex].GetComponentInChildren<TextMeshProUGUI>();
                    if (btnText != null) btnText.text = story.currentChoices[i].text;

                    int choiceIndex = i;
                    choiceButtons[uiButtonIndex].onClick.RemoveAllListeners();
                    choiceButtons[uiButtonIndex].onClick.AddListener(() => OnBubbleClicked(choiceIndex));

                    uiButtonIndex++;
                    hasNormalChoices = true;
                }
            }
        }

        for (int i = uiButtonIndex; i < choiceButtons.Length; i++)
        {
            choiceButtons[i].gameObject.SetActive(false);
        }

        choiceBubblePanel.SetActive(hasNormalChoices);
    }

    private void OnBubbleClicked(int index)
    {
        if (choiceBubblePanel != null) choiceBubblePanel.SetActive(false);
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
        if (tags == null) return;
        foreach (string tag in tags)
        {
            string[] split = tag.Split(':');

            if (split.Length < 2) continue;

            string tagKey = split[0].Trim();
            string tagValue = split[1].Trim();

            if (tagKey == "speaker")
            {
                txtSpeaker.text = tagValue;
            }
            else if (tagKey == "load_scene")
            {
                Debug.Log($"【系统】Ink 请求：{tagValue}");
                Time.timeScale = 1f;
                if (GameSystem.Instance != null) GameSystem.Instance.SaveGame(0);
                UnityEngine.SceneManagement.SceneManager.LoadScene(tagValue);
            }
            else if (tagKey == "action")
            {
                if (tagValue == "upload_data")
                {
                    Debug.Log("data uploading...");
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
                    Debug.Log($"[meta] {tagValue} 永久记录到 {currentUser} ");
                }
            }
            else if (tagKey == "portrait")
            {
                if (ScenarioManager.Instance != null)
                {
                    ScenarioManager.Instance.ChangePortrait(tagValue);
                }
            }
            else if (tagKey == "show")
            {
                if (ScenarioManager.Instance != null)
                {
                    ScenarioManager.Instance.ToggleProp(tagValue, true);
                }
            }
            else if (tagKey == "hide")
            {
                if (ScenarioManager.Instance != null)
                {
                    ScenarioManager.Instance.ToggleProp(tagValue, false);
                }
            }
            else if (tagKey == "bg")
            {
                if (ScenarioManager.Instance != null)
                {
                    ScenarioManager.Instance.ChangeBG(tagValue);
                }
            }
            else if (tagKey == "bgm")
            {
                if (ScenarioManager.Instance != null)
                {
                    ScenarioManager.Instance.ChangeBGM(tagValue);
                }
            }
            else if (tagKey == "sfx")
            {
                if (ScenarioManager.Instance != null)
                {
                    ScenarioManager.Instance.PlaySFX(tagValue);
                }
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

        if (continueIcon != null) continueIcon.SetActive(false);

        int totalChars = txtBody != null ? txtBody.textInfo.characterCount : text.Length;

        float timer = 0f;
        float charsPerSecond = 1f / typeSpeed;

        while (txtBody != null && txtBody.maxVisibleCharacters < totalChars)
        {
            timer += Time.deltaTime; 
            int targetChars = Mathf.FloorToInt(timer * charsPerSecond);
            txtBody.maxVisibleCharacters = targetChars;

            yield return null; 
        }

        _isTyping = false;
        if (txtBody != null) txtBody.maxVisibleCharacters = 99999;
        if (continueIcon != null) continueIcon.SetActive(true);
    }

    private void OnUserClick()
    {
        if (_isTyping)
        {
            if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
            if (txtBody != null)
            {
                txtBody.text = _currentFullText;
                txtBody.maxVisibleCharacters = 99999;
            }
            _isTyping = false;
            if (continueIcon != null) continueIcon.SetActive(true);
        }
        else
        {
            DisplayNextLine();
        }
    }
}