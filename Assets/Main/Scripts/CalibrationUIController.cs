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
        if (multilingualToggle != null)
        {
            multilingualToggle.onValueChanged.RemoveListener(OnMultilingualToggled);
            multilingualToggle.onValueChanged.AddListener(OnMultilingualToggled);
        }
    }

    void OnEnable()
    {
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

        ResetForm();
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

    private void UpdateWarningText()
    {
        string lockedLanguage = PlayerPrefs.GetString("SelectedLanguage", "EN");
        if (_isEditMode)
        {
            SubjectProfileData profile = SubjectProfileService.LoadProfile(_editingSubjectId, createIfMissing: false);
            if (profile != null && !string.IsNullOrWhiteSpace(profile.lockedGameLanguage))
            {
                lockedLanguage = profile.lockedGameLanguage;
            }
        }

        if (warningText == null)
        {
            return;
        }

        string localizedWarning;
        switch (lockedLanguage)
        {
            case "ZH_CN":
                localizedWarning = _isEditMode
                    ? $"正在编辑 {_editingSubjectId}。游戏语言 [{lockedLanguage}] 保持锁定。"
                    : $"警告：本档案游戏语言 [{lockedLanguage}] 将被锁定。";
                break;
            case "JP":
                localizedWarning = _isEditMode
                    ? $"{_editingSubjectId} を編集中です。ゲーム言語 [{lockedLanguage}] は固定されています。"
                    : $"警告: ゲーム言語 [{lockedLanguage}] はこのセーブデータに固定されます。";
                break;
            case "KR":
                localizedWarning = _isEditMode
                    ? $"{_editingSubjectId} 편집 중입니다. 게임 언어 [{lockedLanguage}]은 고정됩니다."
                    : $"경고: 게임 언어 [{lockedLanguage}]은 이 저장 데이터에 고정됩니다.";
                break;
            default:
                localizedWarning = _isEditMode
                    ? $"Editing {_editingSubjectId}. Game language [{lockedLanguage}] remains locked."
                    : $"Warning: game language [{lockedLanguage}] will be locked for this subject.";
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
            string lockedLanguage = PlayerPrefs.GetString("SelectedLanguage", "EN");
            savedProfile = SubjectProfileService.CreateProfile(
                nativeLanguage,
                isMultilingual,
                secondaryLanguages,
                lockedLanguage);

            PlayerPrefs.SetInt("HasLockedLanguage", 1);
            PlayerPrefs.Save();

            if (TelemetryManager.Instance != null)
            {
                TelemetryManager.Instance.SetPlayerProfile(
                    nativeLanguage,
                    isMultilingual,
                    lockedLanguage,
                    secondaryLanguages);
            }

            if (GameSystem.Instance != null)
            {
                GameSystem.Instance.BeginNewGame();
            }
        }

        Debug.Log($"[SubjectProfile] Saved profile: {savedProfile.subjectId}");

        MainMenuController mainMenu = FindFirstObjectByType<MainMenuController>();
        if (mainMenu != null)
        {
            mainMenu.OnSubjectProfileSaved(savedProfile.subjectId);
        }
        else
        {
            gameObject.SetActive(false);
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
