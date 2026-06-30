using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Loads localized compiled Ink TextAssets from Resources/Story before scene Start.
/// Supports both Chapter0_EN-style names and the repository's PrologueEN naming.
/// </summary>
public static class RuntimeInkLocalizationBridge
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Register()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string languageCode = NormalizeLanguageCode(
            PlayerPrefs.GetString("SelectedLanguage", "EN"));

        if (languageCode == "EN")
        {
            return;
        }

        DialogueController[] controllers =
            UnityEngine.Object.FindObjectsByType<DialogueController>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);

        foreach (DialogueController controller in controllers)
        {
            ApplyLocalizedStory(controller, languageCode);
        }
    }

    private static void ApplyLocalizedStory(
        DialogueController controller,
        string languageCode)
    {
        if (controller == null)
        {
            return;
        }

        TextAsset reference = controller.inkJSON_EN != null
            ? controller.inkJSON_EN
            : controller.inkJSONAsset;

        if (reference == null)
        {
            return;
        }

        string baseName = GetBaseStoryName(reference.name);
        string resourceSuffix = languageCode == "ZH_CN"
            ? "ZH"
            : languageCode;

        // Existing chapters use Chapter0_ZH; prologue uses PrologueZH.
        string localizedName = baseName.Equals(
            "Prologue",
            StringComparison.OrdinalIgnoreCase)
                ? baseName + resourceSuffix
                : baseName + "_" + resourceSuffix;

        TextAsset localizedStory =
            Resources.Load<TextAsset>("Story/" + localizedName);

        if (localizedStory == null)
        {
            Debug.LogWarning(
                $"[Localization] Missing Resources/Story/{localizedName}. " +
                "English fallback will be used.");
            return;
        }

        if (languageCode == "ZH_CN")
        {
            controller.inkJSON_ZH = localizedStory;
        }
        else
        {
            // Existing DialogueController routes JP/KR through its default slot.
            controller.inkJSON_EN = localizedStory;
            controller.inkJSONAsset = localizedStory;
        }
    }

    private static string GetBaseStoryName(string assetName)
    {
        if (assetName.EndsWith("_EN", StringComparison.OrdinalIgnoreCase))
        {
            return assetName.Substring(0, assetName.Length - 3);
        }

        if (assetName.EndsWith("EN", StringComparison.OrdinalIgnoreCase))
        {
            return assetName.Substring(0, assetName.Length - 2);
        }

        return assetName;
    }

    private static string NormalizeLanguageCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return "EN";
        }

        switch (code.Trim().ToUpperInvariant())
        {
            case "ZH":
            case "ZH-CN":
            case "ZH_CN":
            case "CHINESE":
                return "ZH_CN";

            case "JA":
            case "JA-JP":
            case "JAPANESE":
            case "JP":
                return "JP";

            case "KO":
            case "KO-KR":
            case "KOREAN":
            case "KR":
                return "KR";

            default:
                return "EN";
        }
    }
}
