using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class InkCommandRouter
{
    private readonly Action<string> _setSpeaker;

    public InkCommandRouter(Action<string> setSpeaker)
    {
        _setSpeaker = setSpeaker;
    }

    public void Execute(IEnumerable<string> tags)
    {
        if (tags == null)
        {
            return;
        }

        foreach (string rawTag in tags)
        {
            ExecuteSingle(rawTag);
        }
    }

    public void ExecuteSingle(string rawTag)
    {
        if (!TryParse(rawTag, out string key, out string value))
        {
            return;
        }

        switch (key)
        {
            case "speaker":
                _setSpeaker?.Invoke(value);
                break;
            case "load_scene":
                LoadScene(value);
                break;
            case "action":
                ExecuteAction(value);
                break;
            case "portrait":
                if (ScenarioManager.Instance != null) ScenarioManager.Instance.ChangePortrait(value);
                break;
            case "show":
                if (ScenarioManager.Instance != null) ScenarioManager.Instance.ToggleProp(value, true);
                break;
            case "hide":
                if (ScenarioManager.Instance != null) ScenarioManager.Instance.ToggleProp(value, false);
                break;
            case "bg":
                if (ScenarioManager.Instance != null) ScenarioManager.Instance.ChangeBG(value);
                break;
            case "bgm":
                if (ScenarioManager.Instance != null) ScenarioManager.Instance.ChangeBGM(value);
                break;
            case "sfx":
                if (ScenarioManager.Instance != null) ScenarioManager.Instance.PlaySFX(value);
                break;
            default:
                Debug.LogWarning($"[Ink] Unknown command tag: {rawTag}");
                break;
        }
    }

    public static bool TryParse(string rawTag, out string key, out string value)
    {
        key = string.Empty;
        value = string.Empty;

        if (string.IsNullOrWhiteSpace(rawTag))
        {
            return false;
        }

        int separatorIndex = rawTag.IndexOf(':');
        if (separatorIndex < 0)
        {
            return false;
        }

        if (separatorIndex == 0 || separatorIndex >= rawTag.Length - 1)
        {
            Debug.LogWarning($"[Ink] Ignoring malformed command tag: {rawTag}");
            return false;
        }

        key = rawTag.Substring(0, separatorIndex).Trim().ToLowerInvariant();
        value = rawTag.Substring(separatorIndex + 1).Trim();

        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
        {
            Debug.LogWarning($"[Ink] Ignoring incomplete command tag: {rawTag}");
            return false;
        }

        return true;
    }

    private static void ExecuteAction(string action)
    {
        if (action == "upload_data")
        {
            if (TelemetryManager.Instance != null)
            {
                TelemetryManager.Instance.UploadDataToServer();
            }
            return;
        }

        if (action.StartsWith("meta_", StringComparison.Ordinal))
        {
            string currentUser = PlayerPrefs.GetString("CurrentUser", "Guest");
            PlayerPrefs.SetInt($"{currentUser}_{action}", 1);
            PlayerPrefs.Save();
            Debug.Log($"[Ink] Permanent meta flag saved: {currentUser}_{action}");
            return;
        }

        Debug.LogWarning($"[Ink] Unknown action command: {action}");
    }

    private static void LoadScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning("[Ink] load_scene command has no scene name.");
            return;
        }

        Time.timeScale = 1f;
        if (GameSystem.Instance != null)
        {
            GameSystem.Instance.SaveGame(0);
        }

        SceneManager.LoadScene(sceneName);
    }
}
