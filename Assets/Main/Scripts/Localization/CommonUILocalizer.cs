using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Localizes common interface labels without requiring a localization component
/// on every existing TextMeshPro object. Exact text matches are converted to a
/// canonical key, allowing language switching in either direction.
///
/// The periodic refresh is intentional: several existing panels are activated
/// at runtime and some controllers write English labels after the scene-loaded
/// callback. Refreshing at a low frequency keeps those late-created/overwritten
/// labels synchronized without requiring new Inspector references.
/// </summary>
public sealed class CommonUILocalizer : MonoBehaviour
{
    private static CommonUILocalizer _instance;
    private string _lastLanguage;
    private float _nextRefreshTime;

    private const float RefreshIntervalSeconds = 0.25f;

    private static readonly Dictionary<string, string[]> Entries =
        new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            // EN, ZH_CN, JP, KR
            ["START"] = new[] { "Start", "开始", "スタート", "시작" },
            ["EXIT"] = new[] { "Exit", "退出", "終了", "종료" },
            ["NEW_GAME"] = new[] { "New Game", "新游戏", "ニューゲーム", "새 게임" },
            ["CONTINUE"] = new[] { "Continue", "继续游戏", "続きから", "이어하기" },
            ["SETTINGS"] = new[] { "Settings", "设置", "設定", "설정" },
            ["QUIT"] = new[] { "Quit", "退出游戏", "ゲーム終了", "게임 종료" },
            ["BACK"] = new[] { "Back", "返回", "戻る", "뒤로" },
            ["CANCEL"] = new[] { "Cancel", "取消", "キャンセル", "취소" },
            ["CONFIRM"] = new[] { "Confirm", "确认", "決定", "확인" },
            ["SAVE"] = new[] { "Save", "保存", "セーブ", "저장" },
            ["LOAD"] = new[] { "Load", "读取", "ロード", "불러오기" },
            ["SAVE_LOAD"] = new[] { "Save & Load", "保存与读取", "セーブ／ロード", "저장 및 불러오기" },
            ["LANGUAGE"] = new[] { "Language", "语言", "言語", "언어" },
            ["RESOLUTION"] = new[] { "Resolution", "分辨率", "解像度", "해상도" },
            ["WINDOWED"] = new[] { "Windowed", "窗口模式", "ウィンドウ表示", "창 모드" },
            ["MUSIC"] = new[] { "Music Volume", "音乐音量", "BGM音量", "음악 음량" },
            ["SFX"] = new[] { "SFX Volume", "音效音量", "効果音音量", "효과음 음량" },
            ["TEXT_SPEED"] = new[] { "Text Speed", "文字速度", "文字送り速度", "텍스트 속도" },
            ["FONT_SIZE"] = new[] { "Font Size", "字体大小", "文字サイズ", "글자 크기" },
            ["SKIP_UNREAD"] = new[] { "Skip Unread Text", "跳过未读文本", "未読もスキップ", "읽지 않은 텍스트도 건너뛰기" },
            ["PROFILE"] = new[] { "Subject Profile", "受试者档案", "参加者プロフィール", "참가자 프로필" },
            ["CREATE_SUBJECT"] = new[] { "Create Subject", "创建受试者", "参加者を作成", "참가자 생성" },
            ["EDIT_SUBJECT"] = new[] { "Edit Subject", "编辑受试者", "参加者を編集", "참가자 편집" },
            ["CREATE"] = new[] { "Create", "创建", "作成", "생성" },
            ["SAVE_CHANGES"] = new[] { "Save Changes", "保存修改", "変更を保存", "변경 사항 저장" },
            ["NATIVE_LANGUAGE"] = new[] { "Native Language", "母语", "母語", "모국어" },
            ["MULTILINGUAL"] = new[] { "Multilingual", "使用多种语言", "複数言語を使用", "다중 언어 사용" },
            ["MULTILANGUAGE"] = new[] { "Multilanguage", "多语言", "多言語", "다국어" },
            ["OTHER_LANGUAGE"] = new[] { "Other Language", "其他语言", "その他の言語", "기타 언어" },
            ["OTHER_LANGUAGES"] = new[] { "Other Languages", "其他语言", "その他の言語", "기타 언어" },
            ["SELECT_OTHER_LANGUAGES"] = new[] { "Select other languages", "选择其他语言", "その他の言語を選択", "기타 언어 선택" },
            ["WHICH_OTHER_LANGUAGES"] = new[] { "Which other languages do you speak?", "您还会使用哪些语言？", "ほかに使用できる言語はありますか？", "그 밖에 사용할 수 있는 언어가 있습니까?" },
            ["PLEASE_SPECIFY"] = new[] { "Please specify", "请注明", "入力してください", "직접 입력" },
            ["GAME_LANGUAGE"] = new[] { "Game Language", "游戏语言", "ゲーム言語", "게임 언어" },
            ["ENGLISH"] = new[] { "English", "英语", "英語", "영어" },
            ["CHINESE"] = new[] { "Chinese", "中文", "中国語", "중국어" },
            ["JAPANESE"] = new[] { "Japanese", "日语", "日本語", "일본어" },
            ["KOREAN"] = new[] { "Korean", "韩语", "韓国語", "한국어" },
            ["FRENCH"] = new[] { "French", "法语", "フランス語", "프랑스어" },
            ["GERMAN"] = new[] { "German", "德语", "ドイツ語", "독일어" },
            ["SPANISH"] = new[] { "Spanish", "西班牙语", "スペイン語", "스페인어" },
            ["OTHER"] = new[] { "Other", "其他", "その他", "기타" },
            ["DONATE"] = new[] { "Donation", "支持作者", "支援", "후원" },
            ["THINKING"] = new[] { "Thinking", "思考中", "思考中", "생각 중" },
            ["SYSTEM"] = new[] { "System", "系统", "システム", "시스템" }
        };

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Create()
    {
        if (_instance != null)
        {
            return;
        }

        GameObject host = new GameObject(nameof(CommonUILocalizer));
        DontDestroyOnLoad(host);
        _instance = host.AddComponent<CommonUILocalizer>();
        SceneManager.sceneLoaded += _instance.OnSceneLoaded;
    }

    public static void RefreshNow()
    {
        if (_instance != null)
        {
            _instance.ApplyCurrentLanguage();
        }
    }

    public static string GetLocalizedText(string key)
    {
        if (!Entries.TryGetValue(key, out string[] values))
        {
            return key;
        }

        string language = Normalize(
            PlayerPrefs.GetString("SelectedLanguage", "EN"));
        return values[LanguageIndex(language)];
    }

    private void Start()
    {
        ApplyCurrentLanguage();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        string language = Normalize(
            PlayerPrefs.GetString("SelectedLanguage", "EN"));

        bool languageChanged = !string.Equals(
            language,
            _lastLanguage,
            StringComparison.Ordinal);

        if (languageChanged || Time.unscaledTime >= _nextRefreshTime)
        {
            Apply(language);
            _nextRefreshTime = Time.unscaledTime + RefreshIntervalSeconds;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyCurrentLanguage();
    }

    private void ApplyCurrentLanguage()
    {
        Apply(Normalize(PlayerPrefs.GetString("SelectedLanguage", "EN")));
    }

    private void Apply(string language)
    {
        _lastLanguage = language;
        int targetIndex = LanguageIndex(language);

        TMP_Text[] labels = FindObjectsByType<TMP_Text>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);

        foreach (TMP_Text label in labels)
        {
            if (label == null || string.IsNullOrWhiteSpace(label.text))
            {
                continue;
            }

            string trimmedText = label.text.Trim();
            string canonicalKey = FindCanonicalKey(trimmedText);
            if (canonicalKey != null)
            {
                label.text = Entries[canonicalKey][targetIndex];
                continue;
            }

            if (TryLocalizeDynamicEditTitle(trimmedText, language, out string localizedTitle))
            {
                label.text = localizedTitle;
            }
        }
    }

    private static bool TryLocalizeDynamicEditTitle(
        string text,
        string language,
        out string localizedText)
    {
        localizedText = null;

        if (!text.StartsWith("Edit ", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        string subjectId = text.Substring("Edit ".Length).Trim();
        if (string.IsNullOrWhiteSpace(subjectId))
        {
            return false;
        }

        switch (language)
        {
            case "ZH_CN":
                localizedText = $"编辑 {subjectId}";
                break;
            case "JP":
                localizedText = $"{subjectId} を編集";
                break;
            case "KR":
                localizedText = $"{subjectId} 편집";
                break;
            default:
                localizedText = $"Edit {subjectId}";
                break;
        }

        return true;
    }

    private static string FindCanonicalKey(string text)
    {
        foreach (KeyValuePair<string, string[]> entry in Entries)
        {
            foreach (string localizedValue in entry.Value)
            {
                if (string.Equals(
                    text,
                    localizedValue,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return entry.Key;
                }
            }
        }

        return null;
    }

    private static int LanguageIndex(string language)
    {
        switch (language)
        {
            case "ZH_CN": return 1;
            case "JP": return 2;
            case "KR": return 3;
            default: return 0;
        }
    }

    private static string Normalize(string code)
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
