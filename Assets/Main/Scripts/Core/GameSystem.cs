using UnityEngine;
using System.IO;
using System.Collections;
using UnityEngine.Networking;
using System.Text;

public class GameSystem : MonoBehaviour
{
    public static GameSystem Instance { get; private set; }

    public PlayerSaveProfile CurrentSave;
    private string savePath;

    public delegate void LanguageChangeHandler();
    public event LanguageChangeHandler OnLanguageChanged;

    [Header("Audio")]
    public AudioSource bgmSource; 

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        float savedVol = PlayerPrefs.GetFloat("MusicVol", 0.75f);
    SetMusicVolume(savedVol); 
        if (bgmSource != null && !bgmSource.isPlaying)
        {
            bgmSource.Play();
        }
    }

    void InitializeSystem()
    {
        savePath = Path.Combine(Application.persistentDataPath, "messenger_save.json");
        LoadGame();
        Debug.Log($"[System] Core system startup. Current language.:{CurrentSave.languageCode}");
    }
    public void SaveGame()
    {
        string json = JsonUtility.ToJson(CurrentSave, true);
        File.WriteAllText(savePath, json);
        Debug.Log("[System] Saved");
    }

    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            CurrentSave = JsonUtility.FromJson<PlayerSaveProfile>(json);
        }
        else
        {
            CurrentSave = new PlayerSaveProfile(); 
            CurrentSave.languageCode = "EN"; 
        }
    }


    public void UploadChoice(string choiceID)
    {
        StartCoroutine(PostDataCoroutine(choiceID));
    }

    IEnumerator PostDataCoroutine(string choiceID)
    {
        string url = "https://";
        ObservationLog log = new ObservationLog();
        log.userId = SystemInfo.deviceUniqueIdentifier; 
        log.choiceId = choiceID;
        log.timestamp = System.DateTime.Now.ToString();

        string json = JsonUtility.ToJson(log);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning("[Data] upload fail: " + request.error);
        }
        else
        {
            Debug.Log("[Data] upload success: " + choiceID);
        }
    }

    public void SwitchLanguage(string langCode)
    {
        CurrentSave.languageCode = langCode;
        SaveGame(); 

        if (OnLanguageChanged != null) OnLanguageChanged.Invoke();
    }
    public string GetLocalizedString(string key)
    {
        return "L10N_" + key;
    }
    public void SetMusicVolume(float volume)
    {
        if (bgmSource != null)
        {
            bgmSource.volume = volume;
        }
    }
}
