using System.Collections.Generic;
using TMPro;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioManager : MonoBehaviour
{
    [Header("UI 组件")]
    public TextMeshProUGUI txtBody;
    public TextMeshProUGUI txtSpeaker;
    public Transform choiceContainer;
    public GameObject buttonPrefab;

    [Header("角色头像库")]
    public Sprite iconSera;
    public Sprite iconKate;
    public Sprite iconAdams;
    public Sprite iconMiniel;
    public Sprite iconRumins;

    [Header("测试数据")]
    public CaseData testCase; 

    void Start()
    {
        if (testCase != null) LoadCase(testCase);
    }

    public void LoadCase(CaseData data)
    {
        txtSpeaker.text = "Sera (记录员)";
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
        Debug.Log("Ambrose 选择了: " + option.text_CN);

        txtBody.text = option.outcomeText_CN;

        foreach (var impact in option.impacts)
        {
            Debug.Log($"影响结算: {impact.target} {impact.valueChange}");
        }

        foreach (Transform child in choiceContainer) Destroy(child.gameObject);
    }
}