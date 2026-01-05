using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class StatImpact
{
    public CharacterType target; 
    public int valueChange;      
    public string reason;        
}

[System.Serializable]
public class AdvancedOption
{
    public string optionID;
    public string text_CN; 

    public List<CharacterType> supporters;

    [TextArea(2, 5)]
    public string outcomeText_CN; 

    public List<StatImpact> impacts;
}

[CreateAssetMenu(fileName = "NewCase", menuName = "Messenger/Case Data")]
public class CaseData : ScriptableObject
{
    public string caseID;
    public string title_CN;

    [TextArea]
    public string description_CN;

    public List<AdvancedOption> options;
}