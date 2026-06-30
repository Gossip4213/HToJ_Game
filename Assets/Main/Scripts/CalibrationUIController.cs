using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CalibrationUIController : MonoBehaviour
{
    [Header("第一层：基础问卷")]
    public TMP_Dropdown nativeLangDropdown;
    public Toggle multilingualToggle;
    public TextMeshProUGUI warningText;

    [Header("档案游戏语言")]
    [Tooltip("Game-language dropdown for this subject. Options are expected in the order EN, ZH_CN, JP, KR.")]
    public TMP_Dropdown gameLanguageDropdown;

    [Header("第二层：外语审问 (动态显示面板)")]
    public GameObject secondaryLangPanel;
    public Toggle[] secondaryLangToggles;
    public TMP_InputField otherLangInput;

    [Header("可选：编辑模式文本")]
    public TextMeshProUGUI formTitleText;
    public TextMeshProUGUI confirmButtonText;

    private bool _isEditMode;
    private bool _profileCreationRequired;
    private string _editingSubjectId;

    void Start()
    {
        ResolveGameLanguageDropdown();

        if (multilingualToggle != null)
        {
            multilingualToggle.onValueChanged.RemoveListener(OnMultilingualToggled);
            multilingualToggle.onValueChanged.AddListener(OnMultilingualToggled);
        }

        if (gameLanguageDropdown != null)
        {
            gameLanguageDropdown.onValueChanged.RemoveListener(OnGameLanguagePreviewChanged);
            gameLanguageDropdown.onValueChanged.AddListener(OnGameLanguagePreviewChanged);
        }
    }

    void OnEnable()
    {
        ResolveGameLanguageDropdown();
        UpdateWarningText();

        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged += UpdateWarningText;
        }

        if (multilingualToggle != null)
        {
            OnMultilingualToggled(multilingualToggle.isOn);
        }
    }

    void OnDisable()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= UpdateWarningText;
        }
    }

    public void OpenCreateMode(bool profileCreationRequired = false)
    {
        _isEditMode = false;
        _profileCreationRequired = profileCreationRequired;
        _editingSubjectId = string.Empty;

        ResolveGameLanguageDropdown();
        ResetForm();
        SetGameLanguageDropdown(PlayerPrefs.GetString("SelectedLanguage", "EN"));
        SetModeLabels("Create Subject", "Create");
        UpdateWarningText();
    }

    public bool OpenEditMode(string subjectId)
    {
        SubjectProfileData profile = SubjectProfileService.LoadProfile(subjectId, createIfMissing: true);
        if (profile == null)
        {
            Debug.LogError($"[SubjectProfile] Cannot edit missing profile: {subjectId}");
            return false;
        }

        _isEditMode = true;
        _profileCreationRequired = false;
        _editingSubjectId = subjectId;

        ResolveGameLanguageDropdown();
        PopulateForm(profile);
        SetModeLabels($"Edit {subjectId}", "Save Changes");
        UpdateWarningText();
        return true;
    }

    private void ResetForm()
    {
        if (nativeLangDropdown != null && nativeLangDropdown.options.Count > 0)
        {
            nativeLangDropdown.SetValueWithoutNotify(0);
            nativeLangDropdown.RefreshShownValue();
        }

        if (multilingualToggle != null)
        {
            multilingualToggle.SetIsOnWithoutNotify(false);
        }

        if (secondaryLangToggles != null)
        {
            foreach (Toggle toggle in secondaryLangToggles)
            {
                if (toggle != null)
                {
                    toggle.SetIsOnWithoutNotify(false);
                }
            }
        }

        if (otherLangInput != null)
        {
            otherLangInput.SetTextWithoutNotify(string.Empty);
        }

        OnMultilingualToggled(false);
    }

    private void PopulateForm(SubjectProfileData profile)
    {
        ResetForm();
        SelectDropdownOption(nativeLangDropdown, profile.nativeLanguage);
        SetGameLanguageDropdown(profile.lockedGameLanguage);

        if (multilingualToggle != null)
        {
            multilingualToggle.SetIsOnWithoutNotify(profile.isMultilingual);
        }

        List<string> unmatchedLanguages = new List<string>();
        if (profile.secondaryLanguages != null)
        {
            foreach (string language in profile.secondaryLanguages)
            {
                string normalizedLanguage = language.StartsWith("Other: ", StringComparison.OrdinalIgnoreCase)
                    ? language.Substring("Other: ".Length).Trim()
                    : language.Trim();

                bool matched = false;
                if (!language.StartsWith("Other: ", StringComparison.OrdinalIgnoreCase) && secondaryLangToggles != null)
                {
                    foreach (Toggle toggle in secondaryLangToggles)
                    {
                        if (toggle == null)
                        {
                            continue;
                        }

                        string label = GetToggleLabel(toggle);
                        if (string.Equals(label, language, StringComparison.OrdinalIgnoreCase))
                        {
                            toggle.SetIsOnWithoutNotify(true);
                            matched = true;
                            break;
                        }
                    }
                }

                if (!matched && !string.IsNullOrWhiteSpace(normalizedLanguage))
                {
                    unmatchedLanguages.Add(normalizedLanguage);
                }
            }
        }

        if (otherLangInput != null)
        {
            otherLangInput.SetTextWithoutNotify(string.Join(", ", unmatchedLanguages));
        }

        OnMultilingualToggled(profile.isMultilingual);
    }

    private void ResolveGameLanguageDropdown()
    {
        if (gameLanguageDropdown != null)
        {
            return;
        }

        TMP_Dropdown[] dropdowns = GetComponentsInChildren<TMP_Dropdown>(true);
        foreach (TMP_Dropdown dropdown in dropdowns)
        {
            if (dropdown == null || dropdown == nativeLangDropdown)
            {
                continue;
            }

            string objectName = dropdown.gameObject.name.ToLowerInvariant();
            if (objectName.Contains("game")
                || objectName.Contains("language")
                || objectName.Contains("lang"))
            {
                gameLanguageDropdown = dropdown;
                return;
            }
        }
    }

    private void SelectDropdownOption(TMP_Dropdown dropdown, string optionText)
    {
        if (dropdown == null || string.IsNullOrWhiteSpace(optionText))
        {
            return;
        }

        for (int index = 0; index < dropdown.options.Count; index++)
        {
            if (string.Equals(dropdown.options[index].text, optionText, StringComparison.OrdinalIgnoreCase))
            {
                dropdown.SetValueWithoutNotify(index);
                dropdown.RefreshShownValue();
                return;
            }
        }
    }

    private void SetModeLabels(string title, string confirmLabel)
    {
        if (formTitleText != null)
        {
            formTitleText.text = title;
        }

        if (confirmButtonText != null)
        {
            confirmButtonText.text = confirmLabel;
        }
    }

    private void OnGameLanguagePreviewChanged(int index)
    {
        UpdateWarningText();
    }

    private void UpdateWarningText()
    {
        string selectedLanguage = GetSelectedGameLanguage();

        if (warningText == null)
        {
            return;
        }

        string localizedWarning;
        switch (selectedLanguage)
        {
            case "ZH_CN":
                localizedWarning = _isEditMode
                    ? $"正在编辑 {_editingSubjectId}。保存后，本档案将使用中文；游戏过程中不会临时切换。"
                    : "保存后，本档案将使用中文；之后可从档案管理或主菜单设置修改。";
                break;
            case "JP":
                localizedWarning = _isEditMode
                    ? $"{_editingSubjectId} を編集中です。保存後、このプロフィールの言語は日本語になります。"
                    : "保存後、このプロフィールのゲーム言語は日本語になります。";
                break;
            case "KR":
                localizedWarning = _isEditMode
                    ? $"{_editingSubjectId} 편집 중입니다. 저장 후 이 프로필의 게임 언어는 한국어가 됩니다."
                    : "저장 후 이 프로필의 게임 언어는 한국어가 됩니다.";
                break;
            default:
                localizedWarning = _isEditMode
                    ? $"Editing {_editingSubjectId}. After saving, this subject will use English."
                    : "After saving, this subject will use English. It can be changed later from profile management or main-menu settings.";
                break;
        }

        warningText.text = $"<color=red>{localizedWarning}</color>";
    }

    private void OnMultilingualToggled(bool isOn)
    {
        if (secondaryLangPanel != null)
        {
            secondaryLangPanel.SetActive(isOn);
        }
    }

    public void OnConfirmAndStartClicked()
    {
        if (nativeLangDropdown == null || nativeLangDropdown.options.Count == 0)
        {
            Debug.LogError("[SubjectProfile] Native-language dropdown is not configured.");
            return;
        }

        string nativeLanguage = nativeLangDropdown.options[nativeLangDropdown.value].text;
        bool isMultilingual = multilingualToggle != null && multilingualToggle.isOn;
        List<string> secondaryLanguages = CollectSecondaryLanguages(isMultilingual);
        string selectedGameLanguage = GetSelectedGameLanguage();
        SubjectProfileData savedProfile;

        if (_isEditMode)
        {
            savedProfile = SubjectProfileService.LoadProfile(_editingSubjectId, createIfMissing: false);
            if (savedProfile == null)
            {
                Debug.LogError($"[SubjectProfile] Failed to load {_editingSubjectId} for editing.");
                return;
            }

            savedProfile.nativeLanguage = nativeLanguage;
            savedProfile.isMultilingual = isMultilingual;
            savedProfile.secondaryLanguages = secondaryLanguages;
            savedProfile.lockedGameLanguage = selectedGameLanguage;

            if (!SubjectProfileService.SaveProfile(savedProfile))
            {
                return;
            }

            if (TelemetryManager.Instance != null)
            {
                TelemetryManager.Instance.UpdateSubjectProfile(savedProfile);
                TelemetryManager.Instance.LogEvent("profile_updated", savedProfile.subjectId);
            }
        }
        else
        {
            savedProfile = SubjectProfileService.CreateProfile(
                nativeLanguage,
                isMultilingual,
                secondaryLanguages,
                selectedGameLanguage);

            PlayerPrefs.SetInt("HasLockedLanguage", 1);
            PlayerPrefs.Save();

            if (TelemetryManager.Instance != null)
            {
                TelemetryManager.Instance.SetPlayerProfile(
                    nativeLanguage,
                    isMultilingual,
                    selectedGameLanguage,
                    secondaryLanguages);
            }

            if (GameSystem.Instance != null)
            {
                GameSystem.Instance.BeginNewGame();
            }
        }

        Debug.Log($"[SubjectProfile] Saved profile: {savedProfile.subjectId}; game language: {selectedGameLanguage}");

        MainMenuController mainMenu = FindFirstObjectByType<MainMenuController>();
        if (mainMenu != null)
        {
            mainMenu.OnSubjectProfileSaved(savedProfile.subjectId);
        }
        else
        {
            ApplyLanguageImmediately(selectedGameLanguage);
            gameObject.SetActive(false);
        }
    }

    private string GetSelectedGameLanguage()
    {
        ResolveGameLanguageDropdown();
        if (gameLanguageDropdown == null || gameLanguageDropdown.options.Count == 0)
        {
            if (_isEditMode)
            {
                SubjectProfileData profile = SubjectProfileService.LoadProfile(_editingSubjectId, createIfMissing: false);
                if (profile != null && !string.IsNullOrWhiteSpace(profile.lockedGameLanguage))
                {
                    return NormalizeLanguageCode(profile.lockedGameLanguage);
                }
            }

            return NormalizeLanguageCode(PlayerPrefs.GetString("SelectedLanguage", "EN"));
        }

        string optionText = gameLanguageDropdown.options[gameLanguageDropdown.value].text;
        string fromText = NormalizeLanguageCode(optionText, allowUnknown: true);
        return string.IsNullOrEmpty(fromText)
            ? LanguageIndexToCode(gameLanguageDropdown.value)
            : fromText;
    }

    private void SetGameLanguageDropdown(string languageCode)
    {
        ResolveGameLanguageDropdown();
        if (gameLanguageDropdown == null || gameLanguageDropdown.options.Count == 0)
        {
            return;
        }

        string normalizedCode = NormalizeLanguageCode(languageCode);
        gameLanguageDropdown.SetValueWithoutNotify(LanguageCodeToIndex(normalizedCode));
        gameLanguageDropdown.RefreshShownValue();
    }

    private static string LanguageIndexToCode(int index)
    {
        switch (index)
        {
            case 1: return "ZH_CN";
            case 2: return "JP";
            case 3: return "KR";
            default: return "EN";
        }
    }

    private static int LanguageCodeToIndex(string code)
    {
        switch (NormalizeLanguageCode(code))
        {
            case "ZH_CN": return 1;
            case "JP": return 2;
            case "KR": return 3;
            default: return 0;
        }
    }

    private static string NormalizeLanguageCode(string value, bool allowUnknown = false)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return allowUnknown ? string.Empty : "EN";
        }

        string normalized = value.Trim().ToUpperInvariant();
        if (normalized == "ZH"
            || normalized == "ZH-CN"
            || normalized == "ZH_CN"
            || normalized.Contains("CHINESE")
            || value.Contains("中文")
            || value.Contains("简体"))
        {
            return "ZH_CN";
        }

        if (normalized == "JA"
            || normalized == "JA-JP"
            || normalized == "JP"
            || normalized.Contains("JAPANESE")
            || value.Contains("日本"))
        {
            return "JP";
        }

        if (normalized == "KO"
            || normalized == "KO-KR"
            || normalized == "KR"
            || normalized.Contains("KOREAN")
            || value.Contains("한국"))
        {
            return "KR";
        }

        if (normalized == "EN" || normalized.Contains("ENGLISH") || value.Contains("英语") || value.Contains("英文"))
        {
            return "EN";
        }

        return allowUnknown ? string.Empty : "EN";
    }

    private static void ApplyLanguageImmediately(string languageCode)
    {
        string normalizedCode = NormalizeLanguageCode(languageCode);
        PlayerPrefs.SetString("SelectedLanguage", normalizedCode);
        PlayerPrefs.Save();

        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.ChangeLanguage(normalizedCode);
        }
    }

    private List<string> CollectSecondaryLanguages(bool isMultilingual)
    {
        List<string> languages = new List<string>();
        if (!isMultilingual)
        {
            return languages;
        }

        if (secondaryLangToggles != null)
        {
            foreach (Toggle toggle in secondaryLangToggles)
            {
                if (toggle != null && toggle.isOn)
                {
                    string label = GetToggleLabel(toggle);
                    if (!string.IsNullOrWhiteSpace(label))
                    {
                        languages.Add(label);
                    }
                }
            }
        }

        if (otherLangInput != null && !string.IsNullOrWhiteSpace(otherLangInput.text))
        {
            string[] otherLanguages = otherLangInput.text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string otherLanguage in otherLanguages)
            {
                string trimmedLanguage = otherLanguage.Trim();
                if (!string.IsNullOrWhiteSpace(trimmedLanguage))
                {
                    languages.Add("Other: " + trimmedLanguage);
                }
            }
        }

        return languages;
    }

    private string GetToggleLabel(Toggle toggle)
    {
        TMP_Text tmpLabel = toggle.GetComponentInChildren<TMP_Text>();
        if (tmpLabel != null)
        {
            return tmpLabel.text;
        }

        Text legacyLabel = toggle.GetComponentInChildren<Text>();
        return legacyLabel != null ? legacyLabel.text : toggle.gameObject.name;
    }

    public void OnCancelClicked()
    {
        if (_profileCreationRequired && !SubjectProfileService.HasAnyActiveProfile())
        {
            if (warningText != null)
            {
                warningText.text = "<color=red>A subject profile is required before entering the main menu.</color>";
            }
            return;
        }

        MainMenuController mainMenu = FindFirstObjectByType<MainMenuController>();
        if (mainMenu != null)
        {
            mainMenu.ShowMenu();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
