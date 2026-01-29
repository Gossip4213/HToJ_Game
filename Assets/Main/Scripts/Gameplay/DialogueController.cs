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

    [Header("Settings")]
    public float typeSpeed = 0.05f;

    [Header("Interaction State")]
    [SerializeField] private InteractableObject currentHoveredObj; // 记录当前悬停对象
    private bool _isTyping = false;
    private string _currentFullText = "";
    private Coroutine _typingCoroutine;

    #region Unity Lifecycle

    void Start()
    {
        if (continueIcon != null) continueIcon.SetActive(false);
        StartStory();
    }

    void Update()
    {
        // 1. 只有在非打字状态下才处理检测（可选，保持你原本逻辑）
        HandleHoverInput();

        // 2. 处理点击输入
        HandleClickInput();
    }

    #endregion

    #region Interaction Logic (2D Raycasting)

    private void HandleHoverInput()
    {
        if (Camera.main == null) return;

        // 获取鼠标在 2D 世界中的坐标
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

        // 检测 2D 碰撞体
        Collider2D hit = Physics2D.OverlapPoint(mousePos2D);

        if (hit != null)
        {
            InteractableObject obj = hit.GetComponent<InteractableObject>();

            if (obj != null)
            {
                if (currentHoveredObj != obj)
                {
                    NotifyHover(obj); // 使用统一的通知函数
                }
                return;
            }
        }

        // 没打中任何东西
        if (currentHoveredObj != null)
        {
            NotifyExit();
        }
    }

    private void HandleClickInput()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (currentHoveredObj != null)
            {
                SelectThisObject(currentHoveredObj.objectID); // 触发物体选择
            }
            else
            {
                OnUserClick(); // 普通翻页
            }
        }
    }

    // --- 供外部或内部调用的统一接口 ---

    public void NotifyHover(InteractableObject obj)
    {
        if (currentHoveredObj != null && currentHoveredObj != obj)
        {
            currentHoveredObj.OnHoverExit();
        }

        currentHoveredObj = obj;
        currentHoveredObj.OnHoverEnter();
        UpdateUIForHover(obj);
    }

    public void NotifyExit()
    {
        if (currentHoveredObj != null)
        {
            currentHoveredObj.OnHoverExit();
            currentHoveredObj = null;

            // 恢复原本的剧情文本
            if (!_isTyping && txtBody != null)
            {
                txtBody.text = _currentFullText;
            }
        }
    }

    public void SelectThisObject(string id)
    {
        if (story == null || story.currentChoices.Count == 0) return;

        for (int i = 0; i < story.currentChoices.Count; i++)
        {
            Choice choice = story.currentChoices[i];
            if (choice.tags != null && choice.tags.Contains("id:" + id))
            {
                story.ChooseChoiceIndex(i);
                NotifyExit(); // 清理状态
                DisplayNextLine();
                return;
            }
        }
        Debug.Log($"尝试点击 {id}，但当前 Ink 中没有匹配选项。");
    }

    private void UpdateUIForHover(InteractableObject obj)
    {
        if (txtBody != null)
        {
            if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
            txtBody.text = obj.hoverThought;
        }
        if (txtSpeaker != null)
        {
            txtSpeaker.text = "Ambrose (思考)";
        }
    }

    #endregion

    #region Ink Core Logic (所有原始功能已找回)

    public void StartStory()
    {
        if (inkJSONAsset == null) return;
        story = new Story(inkJSONAsset.text);
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (story.canContinue)
        {
            string text = story.Continue().Trim();
            ParseTags(story.currentTags); // 找回标签解析

            if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
            _typingCoroutine = StartCoroutine(TypeWriterEffect(text)); // 找回打字机
        }
        else if (story.currentChoices.Count > 0)
        {
            Debug.Log("等待探索...");
        }
    }

    private void ParseTags(List<string> tags)
    {
        if (tags == null) return;
        foreach (string tag in tags)
        {
            string[] split = tag.Split(':');
            if (split.Length == 2 && split[0].Trim() == "speaker")
            {
                txtSpeaker.text = split[1].Trim();
            }
        }
    }

    private IEnumerator TypeWriterEffect(string text)
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

    private void OnUserClick()
    {
        if (_isTyping)
        {
            // 打字中点击：直接显示全文本
            if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
            txtBody.text = _currentFullText;
            _isTyping = false;
            if (continueIcon != null) continueIcon.SetActive(true);
        }
        else
        {
            // 非打字点击：翻下一页
            DisplayNextLine();
        }
    }

    #endregion
}