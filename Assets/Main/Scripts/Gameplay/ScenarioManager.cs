using System.Collections.Generic;
using TMPro;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI txtBody;
    public TextMeshProUGUI txtSpeaker;
    public Transform choiceContainer;
    public GameObject buttonPrefab;

    [Header("avatars")]
    public Sprite iconSera;
    public Sprite iconKate;
    public Sprite iconAdams;
    public Sprite iconMiniel;
    public Sprite iconRumins;

    [Header("Test")]
    public CaseData testCase; 

    void Start()
    {
        if (testCase != null) LoadCase(testCase);
    }

    public void LoadCase(CaseData data)
    {
        txtSpeaker.text = "Sera";
        txtBody.text = data.description_CN;
        GenerateChoices(data.options);
    }

    void GenerateChoices(List<AdvancedOption> options)
    {

        foreach (Transform child in choiceContainer) Destroy(child.gameObject);

        foreach (var option in options)
        {
            GameObject btnObj = Instantiate(buttonPrefab, choiceContainer);

            var texts = btnObj.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length > 0) texts[0].text = option.text_CN;

            btnObj.GetComponent<Button>().onClick.AddListener(() => OnOptionSelected(option));
        }
    }

    void OnOptionSelected(AdvancedOption option)
    {
        Debug.Log("Ambrose choose: " + option.text_CN);

        txtBody.text = option.outcomeText_CN;

        foreach (var impact in option.impacts)
        {
            Debug.Log($"summary: {impact.target} {impact.valueChange}");
        }

        foreach (Transform child in choiceContainer) Destroy(child.gameObject);
    }
}