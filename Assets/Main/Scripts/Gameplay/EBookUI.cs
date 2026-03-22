using UnityEngine;
using TMPro;

public class EnlightenmentBookUI : MonoBehaviour
{
    [Header("UI ")]
    public GameObject bookPanel;
    public TextMeshProUGUI bookText;

    void Start()
    {
        if (bookPanel != null) bookPanel.SetActive(false);
    }

    public void OpenBook()
    {
        if (bookPanel == null || bookText == null) return;

        Time.timeScale = 0f;
        bookPanel.SetActive(true);

        EvaluateEnlightenment();
    }

    public void CloseBook()
    {
        if (bookPanel != null) bookPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void EvaluateEnlightenment()
    {
        string currentUser = PlayerPrefs.GetString("CurrentUser", "Guest");
        string currentLang = PlayerPrefs.GetString("GameLanguage", "CN"); 

        bool stoleDrug = PlayerPrefs.GetInt($"{currentUser}_meta_stole_drug", 0) == 1;
        bool flippedCoin = PlayerPrefs.GetInt($"{currentUser}_meta_flipped_coin", 0) == 1;


        if (stoleDrug)
        {
            bookText.color = new Color(0.8f, 0.1f, 0.1f); // 暗红色
            if (currentLang == "EN")
                bookText.text = "You broke the fragile rules of their society to save a life.\nJustice is a construct. Only survival remains.";
            else
                bookText.text = "为了挽救生命，你击碎了社会脆弱的规则。\n所谓的正义不过是人为的建构，唯有生存才是绝对的。";
        }
        else if (flippedCoin)
        {
            // nihilism
            bookText.color = new Color(0.6f, 0.2f, 0.8f); // 紫色
            if (currentLang == "EN")
                bookText.text = "You outsourced your morality to a piece of metal.\nA true disciple of absurdity.";
            else if (currentLang == "CN")
                bookText.text = "你将自己的道德外包给了一枚冰冷的金属。\n荒诞世界里最虔诚的信徒。";
        }
        else
        {
            // default
            bookText.color = Color.gray;
            if (currentLang == "EN")
                bookText.text = "The pages are blank. The void awaits your choices.";
            else
                bookText.text = "书页一片空白。虚空正在等待你的抉择。";
        }
    }
}