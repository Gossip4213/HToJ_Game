using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject panelMenu;
    public GameObject panelSettings;
    public Button btnContinue;
    public TMP_Dropdown langDropdown;
    public TMP_Dropdown resDropdown;
    public Toggle windowedToggle;
    public Toggle skipUnreadToggle;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider textSpeedSlider;
    public Slider fontSizeSlider;
    public TextMeshProUGUI speedPreviewText;
    public TextMeshProUGUI sizePreviewText;
    public SaveLoadMenuController saveLoadMenu;

    private Resolution[] resolutions;

    public static System.Action<float> OnFontSizeChanged;

    private Coroutine _typingCoroutine;
    private string _previewContent = "Hmm... is it heads or tails this time? I am not sure....";

    void Start()
    {
        ShowMenu();
        InitSettingsUI();
        if (langDropdown != null)
        {
            langDropdown.onValueChanged.AddListener(OnLanguageChanged);
            string savedLang = PlayerPrefs.GetString("SelectedLanguage", "EN");
        }
        CheckContinueButton();
        if (langDropdown != null)
        {
            langDropdown.onValueChanged.AddListener(OnLanguageChanged);
        }
    }
    void CheckContinueButton()
    {
        if (btnContinue != null)
        {
            bool hasSave = (GameSystem.Instance != null && GameSystem.Instance.HasAnySaveFile());
            btnContinue.interactable = hasSave;
            var text = btnContinue.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null) text.alpha = hasSave ? 1f : 0.5f;
        }
    }
    public void OnBtnContinueClick()
    {
        if (saveLoadMenu != null)
        {
            saveLoadMenu.ShowMenu(false);
        }
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
    }

    public void SetTextSpeed(float val)
    {
        int level = Mathf.RoundToInt(val);
        float charDelay = 0.05f;
        float actualSpeed = 1.0f;

        switch (level)
        {
            case 0:
                actualSpeed = 0.5f;
                charDelay = 0.1f;
                break;
            case 1:
                actualSpeed = 1.0f;
                charDelay = 0.05f;
                break;
            case 2:
                actualSpeed = 2.0f;
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

        if (OnFontSizeChanged != null) OnFontSizeChanged.Invoke(scaleFactor);
    }
    public void OnBtnStartClick()
    {
        Debug.Log("Start New Game");
        if (GameSystem.Instance != null) GameSystem.Instance.isLoadingFromSave = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
    void ShowMenu()
    {
        if (panelMenu != null) panelMenu.SetActive(true);
        if (panelSettings != null) panelSettings.SetActive(false);
    }

    public void OnBtnSettingsClick()
    {
        if (panelMenu != null) panelMenu.SetActive(false);
        if (panelSettings != null) panelSettings.SetActive(true);
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

    public void OnBtnCloseSettingsClick()
    {
        ShowMenu();
    }

    public void OnLanguageChanged(int index)
    {
        string code = "EN";
        switch (index)
        {
            case 0: code = "ZH_CN"; break;
            case 1: code = "KR"; break;
            case 2: code = "EN"; break;
            case 3: code = "JP"; break;
        }

        PlayerPrefs.SetString("SelectedLanguage", code);
        PlayerPrefs.Save();

        if (GameSystem.Instance != null)
        {
            GameSystem.Instance.SwitchLanguage(code);
            var allLocalizers = Object.FindObjectsByType<LocalizeText>(FindObjectsSortMode.None);
            foreach (var localizer in allLocalizers)
            {
                localizer.RefreshText();
            }
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