using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject panelMenu;
    public GameObject panelSettings;
    public TMP_Dropdown langDropdown;
    [Header("components")]
    public TMP_Dropdown resDropdown;    
    public Toggle windowedToggle;       
    public Slider musicSlider;          
    public Slider sfxSlider;            
    public Slider textSpeedSlider;
    public AudioSource bgmSource;

    private Resolution[] resolutions;

    void Start()
    {
        ShowMenu();
        InitSettingsUI();
        string savedLang = PlayerPrefs.GetString("SelectedLanguage", "EN");
        if (langDropdown != null)
        {
            langDropdown.onValueChanged.AddListener(OnLanguageChanged);
        }
        IEnumerator FadeInBGM(float targetVolume, float duration)
        {
            bgmSource.volume = 0;
            float savedVolume = PlayerPrefs.GetFloat("MusicVol", 0.75f);
            if (bgmSource != null)
            {
                bgmSource.volume = savedVolume;
            }
            float timer = 0;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                bgmSource.volume = Mathf.Lerp(0, targetVolume, timer / duration);
                yield return null;
            }
        }
    }
    void InitSettingsUI()
    {
        Vector2Int[] targetRes = new Vector2Int[]
    {
        new Vector2Int(3840, 2160), // 4K
        new Vector2Int(2560, 1440), // 2K
        new Vector2Int(1920, 1080), // 1080P
        new Vector2Int(1600, 900),  // 900P
        new Vector2Int(1280, 720)   // 720P
    };
        resDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResIndex = 2;
        List<Resolution> customResList = new List<Resolution>(); for (int i = 0; i < targetRes.Length; i++)
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

        windowedToggle.isOn = !Screen.fullScreen;

        musicSlider.value = PlayerPrefs.GetFloat("MusicVol", 0.75f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVol", 0.75f);
        textSpeedSlider.value = PlayerPrefs.GetFloat("TextSpeed", 1.0f);
    }
    public void SetResolution(int index)
    {
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
        if (GameSystem.Instance != null && GameSystem.Instance.bgmSource != null)
        {
            GameSystem.Instance.bgmSource.volume = val;
        }
    }
    public void SetSFXVolume(float val)
    {
        PlayerPrefs.SetFloat("SFXVol", val);
    }

    public void SetTextSpeed(float val)
    {
        PlayerPrefs.SetFloat("TextSpeed", val);
    }

    void ShowMenu()
    {
        if (panelMenu != null) panelMenu.SetActive(true);
        if (panelSettings != null) panelSettings.SetActive(false);
    }

    public void OnBtnStartClick()
    {
        Debug.Log("Start Game");
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
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
}

public class UISmoothPopup : MonoBehaviour
{
    public CanvasGroup canvasGroup; 
    public float speed = 5f;
    private bool _isOpening = false;

    void OnEnable()
    {
        canvasGroup.alpha = 0f;
        transform.localScale = Vector3.one * 0.9f; 
        _isOpening = true;
    }

    void Update()
    {
        if (_isOpening)
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 1f, Time.deltaTime * speed);
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, Time.deltaTime * speed);
            if (canvasGroup.alpha >= 1f) _isOpening = false;
        }
    }
}