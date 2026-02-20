using UnityEngine;
using System.Collections.Generic;
using TMPro; 

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;

    public string currentLanguage = "EN";

    [Header("全局字体库")]
    public TMP_FontAsset englishFont; 
    public TMP_FontAsset chineseFont; 

    private Dictionary<string, Dictionary<string, string>> localizedText;

    public delegate void OnLanguageChangeDelegate();
    public event OnLanguageChangeDelegate OnLanguageChanged;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        LoadLanguageData();

        if (PlayerPrefs.HasKey("SelectedLanguage"))
        {
            currentLanguage = PlayerPrefs.GetString("SelectedLanguage");
        }
    }

    public TMP_FontAsset GetCurrentFont()
    {
        if (currentLanguage == "ZH_CN" && chineseFont != null)
        {
            return chineseFont;
        }
        return englishFont; 
    }

    void LoadLanguageData()
    {
        localizedText = new Dictionary<string, Dictionary<string, string>>();
        AddText("BTN_SET", "Settings", "设置");
        AddText("BTN_QUIT", "Exit", "退出");
        AddText("BTN_START", "Start", "开始游戏");
        AddText("BTN_CON", "Continue", "继续游戏");
        AddText("TITLE", "Heads, Tails, or Justice?", "Heads, Tails, or Justice?");
    }

    void AddText(string key, string en, string cn)
    {
        var dict = new Dictionary<string, string>();
        dict["ZH_CN"] = cn;
        dict["EN"] = en;
        localizedText[key] = dict;
    }

    public string GetText(string key)
    {
        if (!localizedText.ContainsKey(key)) return key;
        if (localizedText[key].ContainsKey(currentLanguage)) return localizedText[key][currentLanguage];
        return localizedText[key]["EN"];
    }

    public void ChangeLanguage(string newLang)
    {
        currentLanguage = newLang;
        if (OnLanguageChanged != null) OnLanguageChanged();
    }
}