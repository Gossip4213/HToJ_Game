using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class InGameMenuController : MonoBehaviour
{
    [Header("UI 面板与系统")]
    public GameObject pauseMenuPanel;
    public SaveLoadMenuController saveLoadMenu; 

    [Header("设置组件 (从主菜单继承)")]
    public TMP_Dropdown resDropdown;
    public Toggle windowedToggle;
    public Toggle skipUnreadToggle;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider textSpeedSlider;
    public Slider fontSizeSlider;
    public TextMeshProUGUI speedPreviewText;
    public TextMeshProUGUI sizePreviewText;

    private bool isPaused = false;
    private Resolution[] resolutions;
    private Coroutine _typingCoroutine;
    private string _previewContent = "Hmm... is it heads or tails this time? I am not sure....";

    void Start()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;

        InitSettingsUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }


    public void PauseGame()
    {
        isPaused = true;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f; 
    }

    public void ResumeGame()
    {
        isPaused = false;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f; 
    }

 

    public void OnBtnSaveClick()
    {
        if (saveLoadMenu != null) saveLoadMenu.ShowMenu(true);
    }

    public void OnBtnLoadClick()
    {
        if (saveLoadMenu != null) saveLoadMenu.ShowMenu(false);
    }

    public void ReturnToTitle()
    {
        if (TelemetryManager.Instance != null)
        {
            TelemetryManager.Instance.LogEvent("return_to_title", "escaped_to_main_menu");
            TelemetryManager.Instance.SaveToLocal(); 
        }

        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainMenu");
    }
    public void QuitGame()
    {
        if (TelemetryManager.Instance != null)
        {
            TelemetryManager.Instance.LogEvent("quit_game", "escaped_to_desktop");
            TelemetryManager.Instance.SaveToLocal();
        }

        Time.timeScale = 1f;
        Debug.Log("Exiting Game...");

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
            new Vector2Int(3840, 2160), new Vector2Int(2560, 1440),
            new Vector2Int(1920, 1080), new Vector2Int(1600, 900), new Vector2Int(1280, 720)
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
                if (Screen.width == targetRes[i].x && Screen.height == targetRes[i].y) currentResIndex = i;
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
        }

        if (sfxSlider != null) sfxSlider.value = PlayerPrefs.GetFloat("SFXVol", 0.75f);
        if (textSpeedSlider != null) textSpeedSlider.value = PlayerPrefs.GetInt("TextSpeedLevel", 1);
        if (skipUnreadToggle != null) skipUnreadToggle.isOn = PlayerPrefs.GetInt("SkipUnread", 0) == 1;

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
    }

    public void SetMusicVolume(float val)
    {
        PlayerPrefs.SetFloat("MusicVol", val);
        if (GameSystem.Instance != null) GameSystem.Instance.SetMusicVolume(val);
    }

    public void SetSFXVolume(float val)
    {
        PlayerPrefs.SetFloat("SFXVol", val);
    }

    public void SetTextSpeed(float val)
    {
        int level = Mathf.RoundToInt(val);
        float charDelay = 0.05f;

        switch (level)
        {
            case 0: charDelay = 0.1f; break;
            case 1: charDelay = 0.05f; break;
            case 2: charDelay = 0.02f; break;
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
                yield return new WaitForSecondsRealtime(delay);
            }

            yield return new WaitForSecondsRealtime(1.0f);
            speedPreviewText.text = "";
        }
    }

    public void SetSkipUnread(bool isOn)
    {
        PlayerPrefs.SetInt("SkipUnread", isOn ? 1 : 0);
    }

    public void SetFontSize(float val)
    {
        int level = Mathf.RoundToInt(val);
        PlayerPrefs.SetInt("FontSizeLevel", level);

        float scaleFactor = 1.0f;
        float previewSize = 45f;

        switch (level)
        {
            case 0: scaleFactor = 0.9f; previewSize = 40f; break;
            case 1: scaleFactor = 1.0f; previewSize = 45f; break;
            case 2: scaleFactor = 1.1f; previewSize = 50f; break;
        }

        PlayerPrefs.SetFloat("FontScale", scaleFactor);
        if (sizePreviewText != null) sizePreviewText.fontSize = previewSize;

        if (MainMenuController.OnFontSizeChanged != null) MainMenuController.OnFontSizeChanged.Invoke(scaleFactor);
    }
}