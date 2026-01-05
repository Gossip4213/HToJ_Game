using UnityEngine;
using TMPro;

public class LocalizeText : MonoBehaviour
{
    private TextMeshProUGUI _textMesh;
    [Header("Multi_languages_Key")]
    public string localizationKey;

    void Awake()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        RefreshText();
    }

    public void RefreshText()
    {
        if (_textMesh == null) _textMesh = GetComponent<TextMeshProUGUI>();

        if (GameSystem.Instance != null)
        {
            _textMesh.text = GameSystem.Instance.GetLocalizedString(localizationKey);
        }
    }
}