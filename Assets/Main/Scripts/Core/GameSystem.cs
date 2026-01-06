using UnityEngine;
using System.IO;
using System.Collections;
using UnityEngine.Networking;
using System.Text;
using UnityEngine.SceneManagement;

public class GameSystem : MonoBehaviour
{
    public static GameSystem Instance { get; private set; }

    public PlayerSaveProfile CurrentSave;
    public bool isLoadingFromSave = false;

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
        CurrentSave = new PlayerSaveProfile();
        CurrentSave.languageCode = PlayerPrefs.GetString("SelectedLanguage", "EN");
        Debug.Log($"<color=red>[Critical] Save File: {Application.persistentDataPath}</color>");

        Debug.Log($"[System] System initialized. Default Language: {CurrentSave.languageCode}");
    }

    public string GetSavePath(int slotIndex)
    {
        return Path.Combine(Application.persistentDataPath, $"save_data_{slotIndex}.json");
    }

    public bool HasSaveFile(int slotIndex)
    {
        return File.Exists(GetSavePath(slotIndex));
    }
    public bool HasAnySaveFile()
    {
        Debug.Log("searching saves");
        for (int i = 0; i < 6; i++)
        {
            string path = GetSavePath(i);
            bool exists = File.Exists(path);
            if (exists)
            {
                Debug.Log($"slot found{i} | path: {path}");
                return true;
            }
            else
            {
                if (i == 0) Debug.Log($"slot 0 empty | path: {path}");
            }
        }
        Debug.Log("no save");
        return false;
    }
    public PlayerSaveProfile GetSaveProfile(int slotIndex)
    {
        string path = GetSavePath(slotIndex);
        if (File.Exists(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                return JsonUtility.FromJson<PlayerSaveProfile>(json);
            }
            catch
            {
                return null;
            }
        }
        return null;
    }

    public void SaveGame(int slotIndex)
    {
        CurrentSave.saveTime = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm");

        string json = JsonUtility.ToJson(CurrentSave, true);
        File.WriteAllText(GetSavePath(slotIndex), json);

        Debug.Log($"[System] Game Saved to Slot {slotIndex}");
    }

    public void LoadAndStartGame(int slotIndex)
    {
        string path = GetSavePath(slotIndex);
        if (!File.Exists(path))
        {
            Debug.LogWarning($"Slot {slotIndex} is empty!");
            return;
        }

        string json = File.ReadAllText(path);
        CurrentSave = JsonUtility.FromJson<PlayerSaveProfile>(json);

        isLoadingFromSave = true;

        string sceneToLoad = string.IsNullOrEmpty(CurrentSave.currentSceneName) ? "GameScene" : CurrentSave.currentSceneName;
        Debug.Log($"[System] Loading Slot {slotIndex}... Jumping to: {sceneToLoad}");

        SceneManager.LoadScene(sceneToLoad);
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