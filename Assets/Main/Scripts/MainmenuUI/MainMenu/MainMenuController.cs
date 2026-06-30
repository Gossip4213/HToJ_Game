using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject panelMenu;
    public GameObject panelSettings;
    public GameObject panelCalibration;

    [Header("UI Elements")]
    public Button btnContinue;
    public TMP_Dropdown langDropdown;
    public TMP_Dropdown resDropdown;
    public TMP_Dropdown profileDropdown;
    public Toggle windowedToggle;
    public Toggle skipUnreadToggle;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider textSpeedSlider;
    public Slider fontSizeSlider;
    public TextMeshProUGUI speedPreviewText;
    public TextMeshProUGUI sizePreviewText;

    [Header("Managers")]
    public SaveLoadMenuController saveLoadMenu;
    public CalibrationUIController calibrationController;

    public static System.Action<float> OnFontSizeChanged;

    private Coroutine _typingCoroutine;
    private readonly string _previewContent = "Hmm... is it heads or tails this time? I am not sure....";

    void Start()
    {
        SubjectProfileService.EnsureLegacyProfiles();
        InitSettingsUI();
        InitProfileSystem();

        if (langDropdown != null)
        {
            langDropdown.onValueChanged.RemoveListener(OnLanguageChanged);
            langDropdown.onValueChanged.AddListener(OnLanguageChanged);
        }

        SettingsService.ApplyRuntimeSettings();
        CheckContinueButton();
    }

    private void InitProfileSystem()
    {
        RefreshProfileDropdown();

        if (profileDropdown != null)
        {
            profileDropdown.onValueChanged.RemoveAllListeners();
            profileDropdown.onValueChanged.AddListener(OnProfileDropdownChanged);
        }

        if (!SubjectProfileService.HasAnyActiveProfile())
        {
            ShowCalibrationPanel(false, true);
            return;
        }

        ShowMenu();
        SyncRuntimeToCurrentProfile(false);
    }

    private void RefreshProfileDropdown()
    {
        if (profileDropdown == null)
        {
            return;
        }

        List<string> profiles = SubjectProfileService.GetActiveProfileIds();
        profileDropdown.ClearOptions();
        profileDropdown.AddOptions(profiles);

        string currentProfile = SubjectProfileService.GetCurrentProfileId();
        int currentIndex = profiles.IndexOf(currentProfile);
        if (currentIndex >= 0)
        {
            profileDropdown.SetValueWithoutNotify(currentIndex);
        }

        profileDropdown.RefreshShownValue();
    }

    public void OnProfileDropdownChanged(int index)
    {
        if (profileDropdown == null || index < 0 || index >= profileDropdown.options.Count)
        {
            return;
        }

        string selectedProfile = profileDropdown.options[index].text;
        if (!SubjectProfileService.SetCurrentProfile(selectedProfile))
        {
            Debug.LogError($"[SubjectProfile] Could not switch to {selectedProfile}.");
            return;
        }

        SyncRuntimeToCurrentProfile(true);
        CheckContinueButton();
        Debug.Log($"[SubjectProfile] Current subject: {selectedProfile}");
    }

    private void SyncRuntimeToCurrentProfile(bool logSwitch)
    {
        string currentProfileId = SubjectProfileService.GetCurrentProfileId();
        SubjectProfileData profile = SubjectProfileService.LoadProfile(currentProfileId, true);
        if (profile == null)
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(profile.lockedGameLanguage))
        {
            PlayerPrefs.SetString("SelectedLanguage", profile.lockedGameLanguage);
            PlayerPrefs.Save();

            if (LocalizationManager.Instance != null)
            {
                LocalizationManager.Instance.ChangeLanguage(profile.lockedGameLanguage);
            }

            SyncLanguageDropdown(profile.lockedGameLanguage);
        }

        if (GameSystem.Instance != null)
        {
            GameSystem.Instance.BeginNewGame();
        }

        if (TelemetryManager.Instance != null)
        {
            TelemetryManager.Instance.RefreshForCurrentProfile();
            TelemetryManager.Instance.UpdateSubjectProfile(profile);
            if (logSwitch)
            {
                TelemetryManager.Instance.LogEvent("profile_switched", currentProfileId);
            }
        }
    }

    public void OnBtnShowCalibrationClick()
    {
        ShowCalibrationPanel(false, false);
    }

    public void OnBtnEditCurrentProfileClick()
    {
        if (!SubjectProfileService.HasAnyActiveProfile())
        {
            ShowCalibrationPanel(false, true);
            return;
        }

        ShowCalibrationPanel(true, false);
    }

    private void ShowCalibrationPanel(bool editExisting, bool profileCreationRequired)
    {
        if (panelMenu != null) panelMenu.SetActive(false);
        if (panelSettings != null) panelSettings.SetActive(false);
        if (panelCalibration != null) panelCalibration.SetActive(true);

        CalibrationUIController controller = GetCalibrationController();
        if (controller == null)
        {
            Debug.LogError("[SubjectProfile] CalibrationUIController is not assigned or present in the calibration panel.");
            return;
        }

        if (editExisting)
        {
            controller.OpenEditMode(SubjectProfileService.GetCurrentProfileId());
        }
        else
        {
            controller.OpenCreateMode(profileCreationRequired);
        }
    }

    private CalibrationUIController GetCalibrationController()
    {
        if (calibrationController != null)
        {
            return calibrationController;
        }

        if (panelCalibration != null)
        {
            calibrationController = panelCalibration.GetComponentInChildren<CalibrationUIController>(true);
        }

        return calibrationController;
    }

    public void OnSubjectProfileSaved(string subjectId)
    {
        SubjectProfileService.SetCurrentProfile(subjectId);
        RefreshProfileDropdown();
        SyncRuntimeToCurrentProfile(false);
        ShowMenu();
        CheckContinueButton();
    }

    public void OnBtnStartClick()
    {
        string currentProfile = SubjectProfileService.GetCurrentProfileId();
        if (string.IsNullOrWhiteSpace(currentProfile))
        {
            ShowCalibrationPanel(false, true);
            return;
        }

        if (GameSystem.Instance != null)
        {
            GameSystem.Instance.BeginNewGame();
        }

        Debug.Log($"[System] Starting a new game for {currentProfile}.");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Prologue");
    }

    public void ShowMenu()
    {
        if (panelMenu != null) panelMenu.SetActive(true);
        if (panelSettings != null) panelSettings.SetActive(false);
        if (panelCalibration != null) panelCalibration.SetActive(false);
    }

    private void CheckContinueButton()
    {
        bool hasSave = GameSystem.Instance != null && GameSystem.Instance.HasAnySaveFile();
        if (btnContinue == null)
        {
            return;
        }

        btnContinue.interactable = hasSave;
        TextMeshProUGUI text = btnContinue.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            text.alpha = hasSave ? 1f : 0.5f;
        }
    }

    public void OnBtnContinueClick()
    {
        if (saveLoadMenu != null)
        {
            saveLoadMenu.ShowMenu(false);
        }
    }

    public void OnBtnSettingsClick()
    {
        RefreshSettingsControls();
        if (panelMenu != null) panelMenu.SetActive(false);
        if (panelSettings != null) panelSettings.SetActive(true);
        if (panelCalibration != null) panelCalibration.SetActive(false);
    }

    public void OnBtnCloseSettingsClick()
    {
        ShowMenu();
    }

    public void OnBtnQuitClick()
    {
        PlayerPrefs.Save();
        Debug.Log("Exiting...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void InitSettingsUI()
    {
        if (resDropdown != null)
        {
            resDropdown.ClearOptions();
            List<string> options = new List<string>();
            foreach (Vector2Int resolution in SettingsService.SupportedResolutions)
            {
                options.Add(resolution.x + " x " + resolution.y);
            }

            resDropdown.AddOptions(options);
        }

        RefreshSettingsControls();
        SyncLanguageDropdown(PlayerPrefs.GetString("SelectedLanguage", "EN"));
    }

    private void RefreshSettingsControls()
    {
        if (resDropdown != null)
        {
            resDropdown.SetValueWithoutNotify(SettingsService.FindCurrentResolutionIndex());
            resDropdown.RefreshShownValue();
        }

        if (windowedToggle != null)
        {
            windowedToggle.SetIsOnWithoutNotify(!Screen.fullScreen);
        }

        if (musicSlider != null)
        {
            musicSlider.SetValueWithoutNotify(SettingsService.MusicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.SetValueWithoutNotify(SettingsService.SfxVolume);
        }

        if (textSpeedSlider != null)
        {
            textSpeedSlider.SetValueWithoutNotify(SettingsService.TextSpeedLevel);
        }

        if (skipUnreadToggle != null)
        {
            skipUnreadToggle.SetIsOnWithoutNotify(SettingsService.SkipUnread);
        }

        if (fontSizeSlider != null)
        {
            fontSizeSlider.minValue = 0;
            fontSizeSlider.maxValue = 2;
            fontSizeSlider.wholeNumbers = true;
            fontSizeSlider.SetValueWithoutNotify(SettingsService.FontSizeLevel);
        }

        UpdateFontPreview(SettingsService.FontScale);
    }

    public void SetResolution(int index)
    {
        SettingsService.SetResolution(index);
    }

    public void SetWindowed(bool isWindowed)
    {
        SettingsService.SetWindowed(isWindowed);
    }

    public void SetMusicVolume(float value)
    {
        SettingsService.SetMusicVolume(value);
    }

    public void SetSFXVolume(float value)
    {
        SettingsService.SetSfxVolume(value);
    }

    public void SetTextSpeed(float value)
    {
        float delay = SettingsService.SetTextSpeedLevel(value);
        StartSpeedPreview(delay);
    }

    private void StartSpeedPreview(float delay)
    {
        if (speedPreviewText == null)
        {
            return;
        }

        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
        }

        _typingCoroutine = StartCoroutine(RunTypewriterEffect(delay));
    }

    private IEnumerator RunTypewriterEffect(float delay)
    {
        speedPreviewText.text = string.Empty;
        while (true)
        {
            foreach (char character in _previewContent)
            {
                speedPreviewText.text += character;
                yield return new WaitForSecondsRealtime(delay);
            }

            yield return new WaitForSecondsRealtime(1f);
            speedPreviewText.text = string.Empty;
        }
    }

    public void SetSkipUnread(bool isOn)
    {
        SettingsService.SetSkipUnread(isOn);
    }

    public void SetFontSize(float value)
    {
        float scale = SettingsService.SetFontSizeLevel(value);
        UpdateFontPreview(scale);
        OnFontSizeChanged?.Invoke(scale);
    }

    private void UpdateFontPreview(float scale)
    {
        if (sizePreviewText != null)
        {
            sizePreviewText.fontSize = 45f * scale;
        }
    }

    public void OnLanguageChanged(int index)
    {
        string code = LanguageIndexToCode(index);
        PlayerPrefs.SetString("SelectedLanguage", code);
        PlayerPrefs.Save();

        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.ChangeLanguage(code);
        }
    }

    private void SyncLanguageDropdown(string languageCode)
    {
        if (langDropdown == null)
        {
            return;
        }

        langDropdown.SetValueWithoutNotify(LanguageCodeToIndex(languageCode));
        langDropdown.RefreshShownValue();
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
        switch (code)
        {
            case "ZH_CN": return 1;
            case "JP": return 2;
            case "KR": return 3;
            default: return 0;
        }
    }

    public void OnBtnDonationClick()
    {
        Application.OpenURL("https" + "://space.bilibili.com/9039940");
    }

    public class UISmoothPopup : MonoBehaviour
    {
        public CanvasGroup canvasGroup;
        public float speed = 5f;
        private bool _isOpening;

        void OnEnable()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                transform.localScale = Vector3.one * 0.9f;
                _isOpening = true;
            }
        }

        void Update()
        {
            if (!_isOpening || canvasGroup == null)
            {
                return;
            }

            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 1f, Time.deltaTime * speed);
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, Time.deltaTime * speed);
            if (canvasGroup.alpha >= 1f)
            {
                _isOpening = false;
            }
        }
    }
}
