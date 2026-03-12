using UnityEngine;

public class SceneBGMController : MonoBehaviour
{
    [Header("LOCAL BGM MP3)]")]
    public AudioClip sceneMusic;

    void Start()
    {
     
        if (GameSystem.Instance != null && GameSystem.Instance.bgmSource != null)
        {
            if (GameSystem.Instance.bgmSource.clip != sceneMusic)
            {
                GameSystem.Instance.bgmSource.clip = sceneMusic;
                GameSystem.Instance.bgmSource.Play();
            }
        }
        else
        {
            Debug.LogWarning("YOU SHOULD START MainMenu ");
        }
    }
}