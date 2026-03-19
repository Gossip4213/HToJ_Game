using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioManager : MonoBehaviour
{
    // 单例模式，方便全局调用
    public static ScenarioManager Instance { get; private set; }

    [Header("Portraits")]
    public Image portraitDisplay; 
    public List<Sprite> portraitSprites; 

    [Header("Stage Props")]
    public List<GameObject> sceneProps; 

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ChangePortrait(string portraitName)
    {
        if (portraitDisplay == null) return;

        // 如果剧本传入的是 "none" 或者空，就隐藏立绘
        if (string.IsNullOrEmpty(portraitName) || portraitName.ToLower() == "none")
        {
            portraitDisplay.gameObject.SetActive(false);
            return;
        }

        foreach (Sprite s in portraitSprites)
        {
            if (s != null && s.name == portraitName) 
            {
                portraitDisplay.sprite = s;
                portraitDisplay.gameObject.SetActive(true);
                return;
            }
        }
        Debug.LogWarning($"没在列表里找到名为 '{portraitName}' 的立绘图片！");
    }

    public void ToggleProp(string propName, bool isVisible)
    {
        foreach (GameObject prop in sceneProps)
        {
            if (prop != null && prop.name == propName) 
            {
                prop.SetActive(isVisible);
                return;
            }
        }
        Debug.LogWarning($"没在列表里找到名为 '{propName}' 的场景物品！");
    }
}