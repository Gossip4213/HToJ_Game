using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using Ink.Runtime; 

public class DialogueController : MonoBehaviour
{
    [Header("Ink Core")]
    public TextAsset inkJSONAsset; 
    private Story story;

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

    private bool _isTyping = false;
    private string _currentFullText = "";
    private Coroutine _typingCoroutine;

    void Start()
    {
        if (continueIcon != null) continueIcon.SetActive(false);
        StartStory();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            OnUserClick();
        }
    }
    public void StartStory()
    {
        if (inkJSONAsset == null)
        {
            Debug.LogError("no Ink JSON file! please put it");
            return;
        }

        story = new Story(inkJSONAsset.text);
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (story.canContinue)
        {
            string text = story.Continue(); 
            text = text.Trim(); 

            ParseTags(story.currentTags);

            if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
            _typingCoroutine = StartCoroutine(TypeWriterEffect(text));
        }
        else if (story.currentChoices.Count > 0)
        {
            Debug.Log("selection (TBC)");
        }
        else
        {
            Debug.Log("ending");
            // EndDialogue();
        }
    }

    void ParseTags(List<string> tags)
    {
        foreach (string tag in tags)
        {
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2) continue;

            string key = splitTag[0].Trim();
            string value = splitTag[1].Trim();

            if (key == "speaker")
            {
                txtSpeaker.text = value; 
                UpdateCharacterHighlightByName(value);
            }
            else if (key == "layout")
            {
                Debug.Log("changed as: " + value);
            }
        }
    }

    void UpdateCharacterHighlightByName(string name)
    {
        CharacterType type = CharacterType.None;
        try
        {
            type = (CharacterType)System.Enum.Parse(typeof(CharacterType), name);
        }
        catch
        {
            Debug.LogWarning("unknown: " + name);
        }

        UpdateCharacterHighlights(type);
    }

    void UpdateCharacterHighlights(CharacterType activeChar)
    {
        foreach (var img in characterImages) img.color = inactiveColor;

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
}
