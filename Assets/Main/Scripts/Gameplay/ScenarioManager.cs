using System.Collections.Generic;
using System.Text;
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

    private readonly HashSet<string> _missingSpeakerPortraits = new HashSet<string>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ChangePortrait(string portraitName)
    {
        if (portraitDisplay == null) return;
        if (string.IsNullOrEmpty(portraitName) || portraitName.ToLowerInvariant() == "none")
        {
            portraitDisplay.gameObject.SetActive(false);
            return;
        }

        Sprite portrait = FindPortraitByName(portraitName);
        if (portrait != null)
        {
            ShowPortrait(portrait);
            return;
        }

        Debug.LogWarning($"没在列表里找到名为 '{portraitName}' 的立绘");
    }

    /// <summary>
    /// Automatically resolves an Ink speaker name to a sprite that follows the
    /// character-name_test convention. Text after the first colon is treated as
    /// a speaker subtitle, e.g. "The Judge: Third Deliberation" -> Judge_test.
    /// </summary>
    public void ChangePortraitForSpeaker(string speakerName)
    {
        if (portraitDisplay == null || string.IsNullOrWhiteSpace(speakerName))
        {
            return;
        }

        string baseSpeakerName = speakerName;
        int subtitleSeparator = baseSpeakerName.IndexOf(':');
        if (subtitleSeparator >= 0)
        {
            baseSpeakerName = baseSpeakerName.Substring(0, subtitleSeparator);
        }
        baseSpeakerName = baseSpeakerName.Trim();

        if (string.IsNullOrEmpty(baseSpeakerName))
        {
            return;
        }

        List<string> candidates = BuildSpeakerPortraitCandidates(baseSpeakerName);
        foreach (string candidate in candidates)
        {
            Sprite portrait = FindPortraitByName(candidate);
            if (portrait != null)
            {
                ShowPortrait(portrait);
                return;
            }
        }

        string missingKey = NormalizeAssetName(baseSpeakerName);
        if (_missingSpeakerPortraits.Add(missingKey))
        {
            Debug.LogWarning(
                $"[Portrait] No portrait found for speaker '{speakerName}'. " +
                $"Expected one of: {string.Join(", ", candidates)}");
        }
    }

    private List<string> BuildSpeakerPortraitCandidates(string speakerName)
    {
        string normalizedSpeaker = NormalizeAssetName(speakerName);
        List<string> candidates = new List<string>();

        switch (normalizedSpeaker)
        {
            case "thejudge":
            case "judge":
            case "神使":
                candidates.Add("Judge_test");
                candidates.Add("TheJudge_test");
                candidates.Add("Sera_test");
                break;
            case "sera":
                candidates.Add("Sera_test");
                candidates.Add("Judge_test");
                break;
            case "ambrose":
            case "主角":
                candidates.Add("Ambrose_test");
                break;
            case "adams":
            case "亚当斯":
                candidates.Add("Adams_test");
                break;
            case "kate":
            case "凯特":
                candidates.Add("Kate_test");
                break;
            case "miniel":
            case "明伊尔":
                candidates.Add("Miniel_test");
                break;
            case "rumins":
            case "陆明斯":
                candidates.Add("Rumins_test");
                break;
            default:
                candidates.Add(speakerName + "_test");
                break;
        }

        candidates.Add(speakerName);
        return candidates;
    }

    private Sprite FindPortraitByName(string portraitName)
    {
        if (portraitSprites == null || string.IsNullOrWhiteSpace(portraitName))
        {
            return null;
        }

        string normalizedTarget = NormalizeAssetName(portraitName);
        foreach (Sprite sprite in portraitSprites)
        {
            if (sprite != null && NormalizeAssetName(sprite.name) == normalizedTarget)
            {
                return sprite;
            }
        }

        return null;
    }

    private void ShowPortrait(Sprite portrait)
    {
        portraitDisplay.sprite = portrait;
        portraitDisplay.gameObject.SetActive(true);
    }

    private static string NormalizeAssetName(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        StringBuilder builder = new StringBuilder(value.Length);
        foreach (char character in value)
        {
            if (char.IsLetterOrDigit(character) || character > 127)
            {
                builder.Append(char.ToLowerInvariant(character));
            }
        }

        return builder.ToString();
    }

    public void ChangeBG(string bgName)
    {
        if (bgDisplay == null) return;
        if (string.IsNullOrEmpty(bgName) || bgName.ToLowerInvariant() == "none")
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
