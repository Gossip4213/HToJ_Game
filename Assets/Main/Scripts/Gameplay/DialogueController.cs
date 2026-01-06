using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

[System.Serializable]

public class DialogueLine

{
    public string speakerName; // ("Speaker")
    [TextArea(2, 5)]
    public string content;     
    public CharacterType characterID; 
}
public class DialogueController : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI txtSpeaker; 
    public TextMeshProUGUI txtBody;    
    public GameObject continueIcon;    

    [Header("Character Images")]
    public Image[] characterImages;
    public Color activeColor = Color.white;
    public Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 1f); 

    [Header("Settings")]
    public float typeSpeed = 0.05f; 

    private Queue<DialogueLine> _linesQueue = new Queue<DialogueLine>();
    private bool _isTyping = false;
    private string _currentFullText = "";
    private Coroutine _typingCoroutine;

    void Start()
    {
        if (continueIcon != null) continueIcon.SetActive(false);
        List<DialogueLine> testLines = new List<DialogueLine>()
        {
            new DialogueLine(){ speakerName="Miniel", content="Folks!....", characterID=CharacterType.Miniel },
            new DialogueLine(){ speakerName="Adams", content="Hmm...Not fair!", characterID=CharacterType.Adams },
            new DialogueLine(){ speakerName="Sera", content="Calm down.", characterID=CharacterType.Sera },
            new DialogueLine(){ speakerName="Miniel", content="Thanks....Sera.There is...", characterID=CharacterType.Miniel },
        };


        StartDialogue(testLines);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            OnUserClick();
        }
    }

    public void StartDialogue(List<DialogueLine> lines)
    {
        _linesQueue.Clear();
        foreach (var line in lines)
        {
            _linesQueue.Enqueue(line);
        }
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (_linesQueue.Count == 0)
        {
            if (continueIcon != null) continueIcon.SetActive(false);
            EndDialogue();
            return;
        }

        DialogueLine currentLine = _linesQueue.Dequeue();

        txtSpeaker.text = currentLine.speakerName;
        UpdateCharacterHighlights(currentLine.characterID);
        if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
        _typingCoroutine = StartCoroutine(TypeWriterEffect(currentLine.content));
    }

    IEnumerator TypeWriterEffect(string text)
    {
        _isTyping = true;
        _currentFullText = text;
        txtBody.text = ""; 

        if (continueIcon != null) continueIcon.SetActive(false); 

        foreach (char c in text)
        {
            txtBody.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        _isTyping = false;
        if (continueIcon != null) continueIcon.SetActive(true); 
    }

    void OnUserClick()
    {
        if (_isTyping)
        {
            if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
            txtBody.text = _currentFullText;
            _isTyping = false;
            if (continueIcon != null) continueIcon.SetActive(true);
        }
        else
        {
            DisplayNextLine();
        }
    }

    void UpdateCharacterHighlights(CharacterType activeChar)
    {
        // 假设 images 数组顺序对应枚举: 0=None, 1=Miniel, 2=Kate... 
        // 枚举里 None 是 0，所以要注意索引偏移，或者把 None 放在最后
        // 注意：这里需要你根据实际 Image 数组来写逻辑
        // 比如 Image[0] 是 Miniel (对应枚举1)

        // 这里只是个示例逻辑：
        for (int i = 0; i < characterImages.Length; i++)
        {
            // 如果 Image[0] 是 Miniel
            if (activeChar == CharacterType.None)
            {
                characterImages[i].color = inactiveColor;
            }
            else
            {
                // 简单的判断逻辑，具体看数组怎么拖
                // 比如：如果 (i+1) == (int)activeChar，则亮起
                bool isActive = (i + 1) == (int)activeChar;
                characterImages[i].color = isActive ? activeColor : inactiveColor;
            }
        }
    }

    void EndDialogue()
    {
        Debug.Log("对话结束！这里应该弹出选项 (CaseData) 了");
        // TODO:  ChoiceManager.ShowChoices(currentCase);
    }
}
