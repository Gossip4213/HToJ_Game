using UnityEngine;
using UnityEngine.SceneManagement;

public class BootManager : MonoBehaviour
{
    void Start()
    {
        if (PlayerPrefs.HasKey("CurrentUser"))
        {
            string currentUser = PlayerPrefs.GetString("CurrentUser");
            Debug.Log($"检测到已有档案：{currentUser}。直接进入。");

            // 第二次进入：直接跳转主菜单
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            Debug.Log("未检测到受试者档案。进入初始录入。");

            // 第一次进入：强制跳转建立档案场景
            SceneManager.LoadScene("LoginScene");
        }
    }
}