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
    private Story story;

    [Header("UI Components")]
    public TextMeshProUGUI txtSpeaker;
    public TextMeshProUGUI txtBody;
    public GameObject continueIcon;

    [Header("Settings")]
    public float typeSpeed = 0.05f; 

    private bool _isTyping = false;
    private string _currentFullText = "";
    private Coroutine _typingCoroutine;

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

        Debug.Log($"【寻人启事】UI 传来的目标 ID 是: [{id}]");

        for (int i = 0; i < story.currentChoices.Count; i++)
        {
            Choice choice = story.currentChoices[i];

            string allTags = choice.tags != null ? string.Join(", ", choice.tags) : "没贴标签";
            Debug.Log($"【排查选项 {i}】文本: '{choice.text}' | 身上的标签: [{allTags}]");

            if (choice.tags != null)
            {
                foreach (string tag in choice.tags)
                {
                    if (tag.Trim() == "id:" + id.Trim())
                    {
                        Debug.Log("【配对成功】跳转！");
                        story.ChooseChoiceIndex(i);
                        NotifyExitUI();
                        DisplayNextLine();
                        return;
                    }
                }
            }
        }
        Debug.LogWarning("【结论】剧本没找着配对的标签。请对比上方的【寻人启事】和【排查选项】！");
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
            Debug.LogError("【Ink 错误】没有找到对应的语言剧本！");
            return;
        }

        if (story == null)
        {
            story = new Story(selectedJSON.text);
        }
        else
        {
            string savedState = story.state.ToJson();
            story = new Story(selectedJSON.text);

            try
            {
                story.state.LoadJson(savedState);
                Debug.Log("转移成功！进度已继承。");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("剧本结构不一致，无法继承进度，将重新开始。错误: " + e.Message);
            }
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
            bool isInvestigation = false;

            if (story.currentChoices[0].tags != null)
            {
                foreach (string tag in story.currentChoices[0].tags)
                {
                    if (tag.StartsWith("id:")) isInvestigation = true;
                }
            }

            if (isInvestigation)
            {
                Debug.Log("调查，等待点击场景物品");
            }
            else
            {
                ShowChoiceBubbles();
            }
        }
    }

    private void ShowChoiceBubbles()
    {
        if (choiceBubblePanel == null || choiceButtons == null) return;

        choiceBubblePanel.SetActive(true);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < story.currentChoices.Count)
            {
                choiceButtons[i].gameObject.SetActive(true);

                TextMeshProUGUI btnText = choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (btnText != null) btnText.text = story.currentChoices[i].text;
                int choiceIndex = i;
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => OnBubbleClicked(choiceIndex));
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnBubbleClicked(int index)
    {
        if (choiceBubblePanel != null) choiceBubblePanel.SetActive(false);
        story.ChooseChoiceIndex(index);
        DisplayNextLine();
    }

    private void ParseTags(List<string> tags)
    {
        if (tags == null) return;
        foreach (string tag in tags)
        {
            string[] split = tag.Split(':');
            if (split.Length == 2 && split[0].Trim() == "speaker")
            {
                txtSpeaker.text = split[1].Trim();
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