using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Keeps Ink machine tags in English while presenting localized speaker labels.
/// Portrait routing continues to receive the original English identifier.
/// </summary>
public sealed class LocalizedSpeakerLabelBridge : MonoBehaviour
{
    private static LocalizedSpeakerLabelBridge _instance;
    private DialogueController[] _controllers = Array.Empty<DialogueController>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Create()
    {
        if (_instance != null)
        {
            return;
        }

        GameObject host = new GameObject(nameof(LocalizedSpeakerLabelBridge));
        DontDestroyOnLoad(host);
        _instance = host.AddComponent<LocalizedSpeakerLabelBridge>();
        SceneManager.sceneLoaded += _instance.OnSceneLoaded;
    }

    private void Start()
    {
        RefreshControllers();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshControllers();
    }

    private void RefreshControllers()
    {
        _controllers = FindObjectsByType<DialogueController>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);
    }

    private void LateUpdate()
    {
        string language = NormalizeLanguageCode(
            PlayerPrefs.GetString("SelectedLanguage", "EN"));

        foreach (DialogueController controller in _controllers)
        {
            if (controller == null || controller.txtSpeaker == null)
            {
                continue;
            }

            string current = controller.txtSpeaker.text;
            string localized = LocalizeSpeaker(current, language);
            if (!string.Equals(current, localized, StringComparison.Ordinal))
            {
                controller.txtSpeaker.text = localized;
            }
        }
    }

    private static string LocalizeSpeaker(string value, string language)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        string canonical = Canonicalize(value.Trim());
        switch (canonical)
        {
            case "AMBROSE":
                return Select(language, "Ambrose", "安布罗斯", "アンブローズ", "앰브로즈");

            case "THE_JUDGE":
                return Select(language, "The Judge", "神使", "神の使い", "신의 사자");

            case "SYSTEM":
                return Select(language, "System", "系统", "システム", "시스템");

            case "THINKING":
                return Select(language, "Thinking", "思考中", "思考中", "생각 중");

            default:
                return value;
        }
    }

    private static string Canonicalize(string value)
    {
        if (EqualsAny(value, "Ambrose", "安布罗斯", "アンブローズ", "앰브로즈"))
        {
            return "AMBROSE";
        }

        if (EqualsAny(
            value,
            "The Judge",
            "Judge",
            "神使",
            "神の使い",
            "신의 사자"))
        {
            return "THE_JUDGE";
        }

        if (EqualsAny(value, "System", "系统", "システム", "시스템"))
        {
            return "SYSTEM";
        }

        if (EqualsAny(value, "Thinking", "思考中", "생각 중"))
        {
            return "THINKING";
        }

        return string.Empty;
    }

    private static bool EqualsAny(string value, params string[] candidates)
    {
        foreach (string candidate in candidates)
        {
            if (string.Equals(value, candidate, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static string Select(
        string language,
        string english,
        string chinese,
        string japanese,
        string korean)
    {
        switch (language)
        {
            case "ZH_CN": return chinese;
            case "JP": return japanese;
            case "KR": return korean;
            default: return english;
        }
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
