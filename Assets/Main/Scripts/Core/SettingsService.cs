using System;
using UnityEngine;

public static class SettingsService
{
    public const string MusicVolumeKey = "MusicVol";
    public const string SfxVolumeKey = "SFXVol";
    public const string TextSpeedLevelKey = "TextSpeedLevel";
    public const string FontSizeLevelKey = "FontSizeLevel";
    public const string FontScaleKey = "FontScale";
    public const string SkipUnreadKey = "SkipUnread";

    public static readonly Vector2Int[] SupportedResolutions =
    {
        new Vector2Int(3840, 2160),
        new Vector2Int(2560, 1440),
        new Vector2Int(1920, 1080),
        new Vector2Int(1600, 900),
        new Vector2Int(1280, 720)
    };

    public static event Action<float> OnTextDelayChanged;
    public static event Action<float> OnFontScaleChanged;
    public static event Action<float> OnMusicVolumeChanged;
    public static event Action<float> OnSfxVolumeChanged;
    public static event Action<bool> OnSkipUnreadChanged;

    public static float MusicVolume => Mathf.Clamp01(PlayerPrefs.GetFloat(MusicVolumeKey, 0.75f));
    public static float SfxVolume => Mathf.Clamp01(PlayerPrefs.GetFloat(SfxVolumeKey, 0.75f));
    public static int TextSpeedLevel => Mathf.Clamp(PlayerPrefs.GetInt(TextSpeedLevelKey, 1), 0, 2);
    public static int FontSizeLevel => Mathf.Clamp(PlayerPrefs.GetInt(FontSizeLevelKey, 1), 0, 2);
    public static bool SkipUnread => PlayerPrefs.GetInt(SkipUnreadKey, 0) == 1;

    public static float TextDelay
    {
        get
        {
            switch (TextSpeedLevel)
            {
                case 0: return 0.10f;
                case 2: return 0.02f;
                default: return 0.05f;
            }
        }
    }

    public static float FontScale
    {
        get
        {
            switch (FontSizeLevel)
            {
                case 0: return 0.90f;
                case 2: return 1.10f;
                default: return 1.00f;
            }
        }
    }

    public static void ApplyRuntimeSettings()
    {
        if (GameSystem.Instance != null)
        {
            GameSystem.Instance.SetMusicVolume(MusicVolume);
            GameSystem.Instance.SetSFXVolume(SfxVolume);
        }

        OnTextDelayChanged?.Invoke(TextDelay);
        OnFontScaleChanged?.Invoke(FontScale);
        OnMusicVolumeChanged?.Invoke(MusicVolume);
        OnSfxVolumeChanged?.Invoke(SfxVolume);
        OnSkipUnreadChanged?.Invoke(SkipUnread);
    }

    public static void SetMusicVolume(float value)
    {
        float clampedValue = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(MusicVolumeKey, clampedValue);

        if (GameSystem.Instance != null)
        {
            GameSystem.Instance.SetMusicVolume(clampedValue);
        }

        OnMusicVolumeChanged?.Invoke(clampedValue);
    }

    public static void SetSfxVolume(float value)
    {
        float clampedValue = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(SfxVolumeKey, clampedValue);

        if (GameSystem.Instance != null)
        {
            GameSystem.Instance.SetSFXVolume(clampedValue);
        }

        OnSfxVolumeChanged?.Invoke(clampedValue);
    }

    public static float SetTextSpeedLevel(float sliderValue)
    {
        int level = Mathf.Clamp(Mathf.RoundToInt(sliderValue), 0, 2);
        PlayerPrefs.SetInt(TextSpeedLevelKey, level);
        float delay = TextDelay;
        OnTextDelayChanged?.Invoke(delay);
        return delay;
    }

    public static float SetFontSizeLevel(float sliderValue)
    {
        int level = Mathf.Clamp(Mathf.RoundToInt(sliderValue), 0, 2);
        PlayerPrefs.SetInt(FontSizeLevelKey, level);
        float scale = FontScale;
        PlayerPrefs.SetFloat(FontScaleKey, scale);
        OnFontScaleChanged?.Invoke(scale);
        return scale;
    }

    public static void SetSkipUnread(bool isEnabled)
    {
        PlayerPrefs.SetInt(SkipUnreadKey, isEnabled ? 1 : 0);
        OnSkipUnreadChanged?.Invoke(isEnabled);
    }

    public static void SetWindowed(bool isWindowed)
    {
        Screen.fullScreenMode = isWindowed
            ? FullScreenMode.Windowed
            : FullScreenMode.FullScreenWindow;
    }

    public static void SetResolution(int index)
    {
        if (index < 0 || index >= SupportedResolutions.Length)
        {
            return;
        }

        Vector2Int resolution = SupportedResolutions[index];
        Screen.SetResolution(resolution.x, resolution.y, Screen.fullScreenMode);
    }

    public static int FindCurrentResolutionIndex()
    {
        for (int index = 0; index < SupportedResolutions.Length; index++)
        {
            if (Screen.width == SupportedResolutions[index].x &&
                Screen.height == SupportedResolutions[index].y)
            {
                return index;
            }
        }

        return 2;
    }
}
