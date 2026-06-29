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

    private Resolution[] resolutions;
    public static System.Action<float> OnFontSizeChanged;
    private Coroutine _typingCoroutine;
    private string _previewContent = "Hmm... is it heads or tails this time? I am not sure....";

    void Start()
    {
        InitSettingsUI();
        InitProfileSystem();

        if (langDropdown != null)
        {
            langDropdown.onValueChanged.AddListener(OnLanguageChanged);
        }

        CheckContinueButton();
    }

    void InitProfileSystem()
    {
        SubjectProfileService.EnsureLegacyProfiles();
        RefreshProfileDropdown();

        if (profileDropdown != null)
        {
            profileDropdown.onValueChanged.RemoveAllListeners();
            profileDropdown.onValueChanged.AddListener(OnProfileDropdownChanged);
        }

        if (!SubjectProfileService.HasAnyActiveProfile())
        {
            ShowCalibrationPanel(editExisting: false, profileCreationRequired: true);
            return;
        }

        ShowMenu();
        SyncRuntimeToCurrentProfile(logSwitch: false);
    }

    void RefreshProfileDropdown()
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

        SyncRuntimeToCurrentProfile(logSwitch: true);
        CheckContinueButton();
        Debug.Log($"[SubjectProfile] Current subject: {selectedProfile}");
    }

    private void SyncRuntimeToCurrentProfile(bool logSwitch)
    {
        string currentProfileId = SubjectProfileService.GetCurrentProfileId();
        SubjectProfileData profile = SubjectProfileService.LoadProfile(currentProfileId, createIfMissing: true);
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
        ShowCalibrationPanel(editExisting: false, profileCreationRequired: false);
    }

    public void OnBtnEditCurrentProfileClick()
    {
        if (!SubjectProfileService.HasAnyActiveProfile())
        {
            ShowCalibrationPanel(editExisting: false, profileCreationRequired: true);
            return;
        }

        ShowCalibrationPanel(editExisting: true, profileCreationRequired: false);
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
        SyncRuntimeToCurrentProfile(logSwitch: false);
        ShowMenu();
        CheckContinueButton();
    }

    public void OnBtnStartClick()
    {
        string currentProfile = SubjectProfileService.GetCurrentProfileId();
        if (string.IsNullOrWhiteSpace(currentProfile))
        {
            ShowCalibrationPanel(editExisting: false, profileCreationRequired: true);
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

    void CheckContinueButton()
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
        if (saveLoadMenu != null) saveLoadMenu.ShowMenu(false);
    }

    public void OnBtnSettingsClick()
    {
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
        Debug.Log("exiting.....");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void InitSettingsUI()
    {
        Vector2Int[] targetRes = new Vector2Int[]
        {
            new Vector2Int(3840, 2160),
            new Vector2Int(2560, 1440),
            new Vector2Int(1920, 1080),
            new Vector2Int(1600, 900),
            new Vector2Int(1280, 720)
        };

        if (resDropdown != null)
        {
            resDropdown.ClearOptions();
            List<string> options = new List<string>();
            int currentResIndex = 2;
            List<Resolution> customResList = new List<Resolution>();

            for (int i = 0; i < targetRes.Length; i++)
            {
                options.Add(targetRes[i].x + " x " + targetRes[i].y);
                Resolution r = new Resolution();
                r.width = targetRes[i].x;
                r.height = targetRes[i].y;
                customResList.Add(r);
                if (Screen.width == targetRes[i].x && Screen.height == targetRes[i].y)
                {
                    currentResIndex = i;
                }
            }
            resolutions = customResList.ToArray();
            resDropdown.AddOptions(options);
            resDropdown.value = currentResIndex;
            resDropdown.RefreshShownValue();
        }

        if (windowedToggle != null) windowedToggle.isOn = !Screen.fullScreen;
        if (musicSlider != null)
        {
            float savedVol = PlayerPrefs.GetFloat("MusicVol", 0.75f);
            musicSlider.value = savedVol;
            if (GameSystem.Instance != null)
            {
                GameSystem.Instance.SetMusicVolume(savedVol);
            }
        }

        if (sfxSlider != null) sfxSlider.value = PlayerPrefs.GetFloat("SFXVol", 0.75f);
        if (textSpeedSlider != null)
        {
            int savedLevel = PlayerPrefs.GetInt("TextSpeedLevel", 1);
            textSpeedSlider.value = savedLevel;
        }
        if (skipUnreadToggle != null)
        {
            skipUnreadToggle.isOn = PlayerPrefs.GetInt("SkipUnread", 0) == 1;
        }

        if (fontSizeSlider != null)
        {
            fontSizeSlider.minValue = 0;
            fontSizeSlider.maxValue = 2;
            fontSizeSlider.wholeNumbers = true;
            fontSizeSlider.value = PlayerPrefs.GetInt("FontSizeLevel", 1);
        }
    }

    public void SetResolution(int index)
    {
        if (resolutions == null || index < 0 || index >= resolutions.Length) return;
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    public void SetWindowed(bool isWindowed)
    {
        Screen.fullScreenMode = isWindowed ? FullScreenMode.Windowed : FullScreenMode.FullScreenWindow;
        Debug.Log(isWindowed ? "Windowed" : "Full screen");
    }

    public void SetMusicVolume(float val)
    {
        PlayerPrefs.SetFloat("MusicVol", val);
        if (GameSystem.Instance != null)
        {
            GameSystem.Instance.SetMusicVolume(val);
        }
    }

    public void SetSFXVolume(float val)
    {
        PlayerPrefs.SetFloat("SFXVol", val);
        if (GameSystem.Instance != null) GameSystem.Instance.SetSFXVolume(val);
    }

    public void SetTextSpeed(float val)
    {
        int level = Mathf.RoundToInt(val);
        float charDelay = 0.05f;

        switch (level)
        {
            case 0:
                charDelay = 0.1f;
                break;
            case 1:
                charDelay = 0.05f;
                break;
            case 2:
                charDelay = 0.02f;
                break;
        }

        PlayerPrefs.SetInt("TextSpeedLevel", level);

        if (speedPreviewText != null)
        {
            if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
            _typingCoroutine = StartCoroutine(RunTypewriterEffect(charDelay));
        }
    }

    IEnumerator RunTypewriterEffect(float delay)
    {
        speedPreviewText.text = "";
        while (true)
        {
            foreach (char c in _previewContent)
            {
                speedPreviewText.text += c;
                yield return new WaitForSeconds(delay);
            }

            yield return new WaitForSeconds(1.0f);
            speedPreviewText.text = "";
        }
    }

    public void SetSkipUnread(bool isOn)
    {
        PlayerPrefs.SetInt("SkipUnread", isOn ? 1 : 0);
        Debug.Log("SkipUnread: " + isOn);
    }

    public void SetFontSize(float val)
    {
        int level = Mathf.RoundToInt(val);
        PlayerPrefs.SetInt("FontSizeLevel", level);

        float scaleFactor = 1.0f;
        float previewSize = 45f;

        switch (level)
        {
            case 0:
                scaleFactor = 0.9f;
                previewSize = 40f;
                break;
            case 1:
                scaleFactor = 1.0f;
                previewSize = 45f;
                break;
            case 2:
                scaleFactor = 1.1f;
                previewSize = 50f;
                break;
        }

        PlayerPrefs.SetFloat("FontScale", scaleFactor);

        if (sizePreviewText != null)
        {
            sizePreviewText.fontSize = previewSize;
        }

        OnFontSizeChanged?.Invoke(scaleFactor);
    }

    public void OnLanguageChanged(int index)
    {
        string code = "EN";
        switch (index)
        {
            case 0: code = "EN"; break;
            case 1: code = "ZH_CN"; break;
            case 2: code = "JP"; break;
            case 3: code = "KR"; break;
        }

        PlayerPrefs.SetString("SelectedLanguage", code);
        PlayerPrefs.Save();

        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.ChangeLanguage(code);
        }
    }

    public void OnBtnDonationClick()
    {
        Application.OpenURL("https://space.bilibili.com/9039940");
    }

    public class UISmoothPopup : MonoBehaviour
    {
        public CanvasGroup canvasGroup;
        public float speed = 5f;
        private bool _isOpening = false;

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
            if (_isOpening && canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 1f, Time.deltaTime * speed);
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, Time.deltaTime * speed);
                if (canvasGroup.alpha >= 1f) _isOpening = false;
            }
        }
    }
}
