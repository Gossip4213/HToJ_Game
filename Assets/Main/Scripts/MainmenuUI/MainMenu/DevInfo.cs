using UnityEngine;
using TMPro;
[ExecuteAlways] 
public class DevInfo : MonoBehaviour
{
    private TextMeshProUGUI _textMesh;

    void Start()
    {
        UpdateText();
    }
    void OnValidate()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        if (_textMesh == null) _textMesh = GetComponent<TextMeshProUGUI>();

        if (_textMesh != null)
        {
            string version = Application.version;
            if (string.IsNullOrEmpty(version)) version = "0.1";

            _textMesh.text = $"Ver {version} | Created by Gossip4213";
        }
    }
}