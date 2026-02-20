using UnityEngine;
using TMPro;

public class LocalizeUI : MonoBehaviour
{
    public string key;

    [Header("font")]
    public TMP_FontAsset englishFont; 
    public TMP_FontAsset chineseFont; 

    private TextMeshProUGUI tmpText;

    void Start()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
        UpdateText();

        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged += UpdateText;
        }
    }

    void OnDestroy()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= UpdateText;
        }
    }

    void UpdateText()
    {
        if (tmpText != null && LocalizationManager.Instance != null)
        {
            tmpText.text = LocalizationManager.Instance.GetText(key);

            if (englishFont != null && chineseFont != null)
            {
                if (LocalizationManager.Instance.currentLanguage == "ZH_CN")
                {
                    tmpText.font = chineseFont;
                }
                else
                {
                    tmpText.font = englishFont;
                }
            }
        }
    }
}