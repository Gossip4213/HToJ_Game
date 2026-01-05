using UnityEngine;
using System.Collections.Generic;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;
    private Dictionary<string, Dictionary<string, string>> localizedText;

    void Awake()
    {
        Instance = this;
        LoadLanguageData();
    }

    void LoadLanguageData()
    {
        localizedText = new Dictionary<string, Dictionary<string, string>>();

        AddText("BTN_START", "开始推演", "開始推演", "Start Simulation", "シミュレーション開始");
        AddText("BTN_SETTINGS", "研讨会设置", "研討會設置", "Settings", "設定");
        AddText("BTN_QUIT", "离开", "離開", "Quit", "終了");
        AddText("LBL_VOLUME", "音量", "音量", "Volume", "音量");
    }

    void AddText(string key, string cn, string tw, string en, string jp)
    {
        var dict = new Dictionary<string, string>();
        dict["ZH_CN"] = cn;
        dict["ZH_TW"] = tw;
        dict["EN"] = en;
        dict["JP"] = jp;
        localizedText[key] = dict;
    }

    public string GetText(string key)
    {
        if (!localizedText.ContainsKey(key)) return key; 

        string currentLang = GameSystem.Instance.CurrentSave.languageCode;

        if (localizedText[key].ContainsKey(currentLang))
        {
            return localizedText[key][currentLang];
        }
        return localizedText[key]["EN"]; 
    }
}