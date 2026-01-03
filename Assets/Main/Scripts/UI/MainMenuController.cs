using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    public GameObject panelMenu;
    public GameObject panelSettings;
    public TMP_Dropdown langDropdown;

    void Start()
    {
        // 这里调用了 ShowMenu，下面必须有 ShowMenu 的定义
        ShowMenu();

        // 监听语言下拉菜单的变化
        if (langDropdown != null)
        {
            langDropdown.onValueChanged.AddListener(OnLanguageChanged);
        }
    }

    // === 修复点：这里补上了 ShowMenu 函数 ===
    void ShowMenu()
    {
        if (panelMenu != null) panelMenu.SetActive(true);
        if (panelSettings != null) panelSettings.SetActive(false);
    }

    public void OnBtnStartClick()
    {
        Debug.Log("Start Game");
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    public void OnBtnSettingsClick()
    {
        if (panelMenu != null) panelMenu.SetActive(false);
        if (panelSettings != null) panelSettings.SetActive(true);
    }

    public void OnBtnCloseSettingsClick()
    {
        ShowMenu(); // 返回主菜单
    }

    public void OnLanguageChanged(int index)
    {
        string code = "ZH_CN";
        switch (index)
        {
            case 0: code = "ZH_CN"; break;
            case 1: code = "ZH_TW"; break;
            case 2: code = "EN"; break;
            case 3: code = "JP"; break;
        }

        // 调用核心系统切换语言（加了空值检查防止报错）
        if (GameSystem.Instance != null)
        {
            GameSystem.Instance.SwitchLanguage(code);
        }
    }

    public void OnBtnDonationClick()
    {
        // 替换成你的赞助链接
        Application.OpenURL("https://space.bilibili.com/9039940");

    }
}
