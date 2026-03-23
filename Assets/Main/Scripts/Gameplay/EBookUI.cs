using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using Ink.Runtime; // 

public class EnlightenmentBookUI : MonoBehaviour
{
    [Header("UI 组件")]
    public GameObject bookPanel;
    public TextMeshProUGUI bookText;

    private enum EndingPath { Kate, Adams, Miniel, Rumins, Sera }

    [Header("色彩配置")]
    private readonly string COL_PROLOGUE = "#000000"; // 黑色
    private readonly string COL_KATE = "#004d00"; // 墨绿
    private readonly string COL_ADAMS = "#8b0000"; // 暗红
    private readonly string COL_RUMINS = "#004080"; // 冷蓝
    private readonly string COL_MINIEL = "#b8860b"; // 暗黄
    private readonly string COL_SERA = "#4b0082"; // 深紫
    private readonly string COL_HIDDEN = "#aaaaaa"; // 占位符灰色

    private const int MAX_SCORE = 10;
    private const int TOTAL_CHAPTERS = 6;

    public void OpenBook()
    {
        if (bookPanel == null || bookText == null) return;
        Time.timeScale = 0f;
        bookPanel.SetActive(true);
        EvaluateEnlightenment();
    }

    private void EvaluateEnlightenment()
    {
        string currentUser = PlayerPrefs.GetString("CurrentUser", "Guest");

        var dc = UnityEngine.Object.FindFirstObjectByType<DialogueController>();
        if (dc == null) { Debug.LogError("找不到 DialogueController"); return; }

        var story = dc.story;
        if (story == null) return;

        int currentChapter = story.variablesState.GlobalVariableExistsWithName("current_chapter") ? (int)story.variablesState["current_chapter"] : 1;

        Dictionary<EndingPath, int> scores = new Dictionary<EndingPath, int>
        {
            { EndingPath.Kate,   story.variablesState.GlobalVariableExistsWithName("score_kate") ? (int)story.variablesState["score_kate"] : 0 },
            { EndingPath.Adams,  story.variablesState.GlobalVariableExistsWithName("score_adams") ? (int)story.variablesState["score_adams"] : 0 },
            { EndingPath.Miniel, story.variablesState.GlobalVariableExistsWithName("score_miniel") ? (int)story.variablesState["score_miniel"] : 0 },
            { EndingPath.Rumins, story.variablesState.GlobalVariableExistsWithName("score_rumins") ? (int)story.variablesState["score_rumins"] : 0 },
            { EndingPath.Sera,   story.variablesState.GlobalVariableExistsWithName("score_sera") ? (int)story.variablesState["score_sera"] : 0 }
        };

        bool hasEverKilled = PlayerPrefs.GetInt($"{currentUser}_meta_has_killed", 0) == 1;

        var leading = scores.OrderByDescending(x => x.Value).First();

        string finalPoem = GenerateMultilingualPoem(currentChapter, leading.Key, leading.Value);

        if (hasEverKilled)
        {
            finalPoem += "\n\n<color=#FF0000>观察者记得你手上的血，无论你如何重置时间。</color>";
        }

        bookText.text = finalPoem;
    }

    private string GenerateMultilingualPoem(int chapter, EndingPath path, int score)
    {
        string prologueStr = GetLocalizedText("POEM_PROLOGUE");
        string pathStr = GetLocalizedText($"POEM_{path.ToString().ToUpper()}");

        string prologueRev = UnveilText(prologueStr, (float)chapter / TOTAL_CHAPTERS, COL_PROLOGUE);
        string pathRev = score > 0 ? UnveilText(pathStr, (float)score / MAX_SCORE, GetPathHexColor(path)) : "...";

        return $"{prologueRev}\n\n{pathRev}";
    }

    private string UnveilText(string fullText, float percent, string hexColor)
    {
        if (string.IsNullOrEmpty(fullText)) return "...";

        percent = Mathf.Clamp01(percent);
        int charCount = Mathf.FloorToInt(fullText.Length * percent);

        string revealedPart = fullText.Substring(0, charCount);
        string hiddenPart = new string('·', fullText.Length - charCount);

        return $"<color={hexColor}>{revealedPart}</color><color={COL_HIDDEN}>{hiddenPart}</color>";
    }

    private string GetLocalizedText(string key)
    {
        if (LocalizationManager.Instance != null)
        {
            return LocalizationManager.Instance.GetText(key);
        }
        return $"[{key}_MISSING]";
    }

    private string GetPathHexColor(EndingPath path)
    {
        return path switch
        {
            EndingPath.Kate => COL_KATE,
            EndingPath.Adams => COL_ADAMS,
            EndingPath.Miniel => COL_MINIEL,
            EndingPath.Rumins => COL_RUMINS, 
            EndingPath.Sera => COL_SERA,
            _ => "#FFFFFF"
        };
    }

    public void CloseBook()
    {
        bookPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}