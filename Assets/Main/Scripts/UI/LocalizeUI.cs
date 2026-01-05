using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class LocalizeUI : MonoBehaviour
{
    public string key; 
    private TextMeshProUGUI tmpText;

    void Start()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
        UpdateText();

        GameSystem.Instance.OnLanguageChanged += UpdateText;
    }

    void OnDestroy()
    {
        if (GameSystem.Instance != null)
            GameSystem.Instance.OnLanguageChanged -= UpdateText;
    }

    void UpdateText()
    {
        if (tmpText != null && LocalizationManager.Instance != null)
        {
            tmpText.text = LocalizationManager.Instance.GetText(key);
        }
    }
}