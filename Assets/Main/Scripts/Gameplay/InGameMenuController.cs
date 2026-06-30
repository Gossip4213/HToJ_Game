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

    private bool isPaused;
    private Coroutine _typingCoroutine;
    private readonly string _previewContent = "Hmm... is it heads or tails this time? I am not sure....";

    void Start()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        Time.timeScale = 1f;
        InitSettingsUI();
        SettingsService.ApplyRuntimeSettings();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        Time.timeScale = 1f;
    }

    public void OnBtnSaveClick()
    {
        if (saveLoadMenu != null)
        {
            saveLoadMenu.ShowMenu(true);
        }
    }

    public void OnBtnLoadClick()
    {
        if (saveLoadMenu != null)
        {
            saveLoadMenu.ShowMenu(false);
        }
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
        MainMenuController.OnFontSizeChanged?.Invoke(scale);
    }

    private void UpdateFontPreview(float scale)
    {
        if (sizePreviewText != null)
        {
            sizePreviewText.fontSize = 45f * scale;
        }
    }
}
