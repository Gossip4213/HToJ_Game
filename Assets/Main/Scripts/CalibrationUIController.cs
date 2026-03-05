using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CalibrationUIController : MonoBehaviour
{
    [Header("第一层：基础问卷")]
    public TMP_Dropdown nativeLangDropdown;
    public Toggle multilingualToggle;
    public TextMeshProUGUI warningText;

    [Header("第二层：外语审问 (动态显示面板)")]
    public GameObject secondaryLangPanel; 
    public Toggle[] secondaryLangToggles; 
    public TMP_InputField otherLangInput; 

    [Header("场景设置")]
    public string prologueSceneName = "PrologueScene";

    void OnEnable()
    {
        UpdateWarningText(); 

        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged += UpdateWarningText;
        }
    }

    void OnDisable()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= UpdateWarningText;
        }
    }

    private void UpdateWarningText()
    {
        string currentLang = PlayerPrefs.GetString("SelectedLanguage", "EN");

        if (warningText != null)
        {
            string localizedWarning = "";
            switch (currentLang)
            {
                case "ZH_CN":
                    localizedWarning = $"警告：本存档游戏过程中[{currentLang}] 将被锁定,无法更改。";
                    break;
                case "EN":
                    localizedWarning = $"Warning: [{currentLang}] will be locked in this savedata and cannot be changed during the game.";
                    break;
                case "JP":
                    localizedWarning = $"警告: 現在の言語 [{currentLang}] はこのセーブデータ中はロックされ、変更できません。";
                    break;
                case "KR":
                    localizedWarning = $"경고: [{currentLang}]은 이 저장 데이터에 고정되어 게임 도중 변경할 수 없습니다.";
                    break;
                default:
                    localizedWarning = $"WARNING: Language [{currentLang}] will be locked.";
                    break;
            }
            warningText.text = $"<color=red>{localizedWarning}</color>";
        }
    }
    private void OnMultilingualToggled(bool isOn)
    {
        if (secondaryLangPanel != null)
        {
            secondaryLangPanel.SetActive(isOn);
        }
    }

    public void OnConfirmAndStartClicked()
    {
        string nativeLang = nativeLangDropdown.options[nativeLangDropdown.value].text;
        bool isMulti = multilingualToggle.isOn;
        string currentLang = PlayerPrefs.GetString("SelectedLanguage", "EN");

        List<string> collectedSecondaryLangs = new List<string>();
        if (isMulti)
        {

            foreach (Toggle t in secondaryLangToggles)
            {
                if (t.isOn)
                {
                    TextMeshProUGUI label = t.GetComponentInChildren<TextMeshProUGUI>();
                    if (label != null) collectedSecondaryLangs.Add(label.text);
                }
            }

            if (otherLangInput != null && !string.IsNullOrWhiteSpace(otherLangInput.text))
            {
                collectedSecondaryLangs.Add("Other: " + otherLangInput.text.Trim());
            }
        }

        if (TelemetryManager.Instance != null)
        {
            TelemetryManager.Instance.SetPlayerProfile(nativeLang, isMulti, currentLang, collectedSecondaryLangs);
        }

        PlayerPrefs.SetInt("HasLockedLanguage", 1);
        PlayerPrefs.Save();

        Debug.Log("【系统】调查完毕，数据已入库...");
        SceneManager.LoadScene(prologueSceneName);
    }

    public void OnCancelClicked()
    {
        gameObject.SetActive(false);
    }
}