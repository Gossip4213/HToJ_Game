using UnityEngine;
using TMPro;

public class LocalizeUI : MonoBehaviour
{
    public string key;

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
        }
    }
}