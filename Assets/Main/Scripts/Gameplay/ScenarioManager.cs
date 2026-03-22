using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioManager : MonoBehaviour
{
    public static ScenarioManager Instance { get; private set; }

    [Header("Portraits (立绘)")]
    public Image portraitDisplay;
    public List<Sprite> portraitSprites;

    [Header("Backgrounds (背景)")]
    public Image bgDisplay;
    public List<Sprite> bgSprites;

    [Header("Stage Props (场景物品)")]
    public List<GameObject> sceneProps;

    [Header("Audio (声音）")] 
    public List<AudioClip> bgmClips;
    public List<AudioClip> sfxClips;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ChangePortrait(string portraitName)
    {
        if (portraitDisplay == null) return;
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
        Debug.LogWarning($"没在列表里找到名为 '{portraitName}' 的立绘");
    }

    public void ChangeBG(string bgName)
    {
        if (bgDisplay == null) return;
        if (string.IsNullOrEmpty(bgName) || bgName.ToLower() == "none")
        {
            bgDisplay.gameObject.SetActive(false);
            return;
        }
        foreach (Sprite s in bgSprites)
        {
            if (s != null && s.name == bgName)
            {
                bgDisplay.sprite = s;
                bgDisplay.gameObject.SetActive(true);
                return;
            }
        }
        Debug.LogWarning($"没在列表里找到名为 '{bgName}' 的背景");
    }

    public void PlaySFX(string sfxName)
    {
        if (GameSystem.Instance == null || GameSystem.Instance.sfxSource == null) return;

        foreach (AudioClip clip in sfxClips)
        {
            if (clip != null && clip.name == sfxName)
            {
                GameSystem.Instance.sfxSource.PlayOneShot(clip);
                return;
            }
        }
        Debug.LogWarning($"没在列表里找到名为 '{sfxName}' ");
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

    public void ChangeBGM(string bgmName)
    {
        if (GameSystem.Instance == null || GameSystem.Instance.bgmSource == null) return;

        foreach (AudioClip clip in bgmClips)
        {
            if (clip != null && clip.name == bgmName)
            {
                if (GameSystem.Instance.bgmSource.clip != clip)
                {
                    GameSystem.Instance.bgmSource.clip = clip;
                    GameSystem.Instance.bgmSource.Play();
                }
                return;
            }
        }
        Debug.LogWarning($"没在列表里找到名为 '{bgmName}' 的音频文件！");
    }
}