using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Text;


[System.Serializable]
public class TelemetryEvent
{
    public string timestamp;      
    public string event_type;     
    public string target_id;      
    public float duration_sec;    
    public string extra_data;     
}

[System.Serializable]
public class GameSession
{
    public string session_start;
    public string session_end;
    public List<TelemetryEvent> events = new List<TelemetryEvent>();
}

[System.Serializable]
public class PlayerProfile
{
    public string user_uuid;             
    public string native_language;       
    public bool is_multilingual;
    public List<string> secondary_languages;
    public string locked_game_language;  
    public int total_playthroughs;       
}

[System.Serializable]
public class TelemetryPayload
{
    public PlayerProfile profile;
    public GameSession current_session;
}


public class TelemetryManager : MonoBehaviour
{
    public static TelemetryManager Instance;

    [Header("服务器配置")]

    public string serverEndpoint = "https://webhook.site/29d67cce-cc16-4891-8bc0-5351a4b65d0e";

    private TelemetryPayload _payload;
    private string _localSavePath;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitTelemetry();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitTelemetry()
    {
        _localSavePath = Application.persistentDataPath + "/telemetry_log.json";
        _payload = new TelemetryPayload();
        _payload.profile = new PlayerProfile();
        _payload.current_session = new GameSession();

        if (!PlayerPrefs.HasKey("PlayerUUID"))
        {
            PlayerPrefs.SetString("PlayerUUID", System.Guid.NewGuid().ToString());
            PlayerPrefs.SetInt("Playthroughs", 1);
        }
        _payload.profile.user_uuid = PlayerPrefs.GetString("PlayerUUID");
        _payload.profile.total_playthroughs = PlayerPrefs.GetInt("Playthroughs");

        _payload.current_session.session_start = System.DateTime.UtcNow.ToString("o");

        Debug.Log($"【观测者启动】UUID: {_payload.profile.user_uuid} | 数据将保存在: {_localSavePath}");
    }


    public void LogEvent(string type, string target, float duration = 0f, string extra = "")
    {
        TelemetryEvent newEvent = new TelemetryEvent
        {
            timestamp = System.DateTime.UtcNow.ToString("o"),
            event_type = type,
            target_id = target,
            duration_sec = duration,
            extra_data = extra
        };
        _payload.current_session.events.Add(newEvent);

        SaveToLocal();
    }

    /// <summary>
    /// selection and time
    /// </summary>
    public void LogChoiceHesitation(string choiceText, float hesitationSeconds)
    {
        LogEvent("choice_made", choiceText, hesitationSeconds, "ink_choice");
        Debug.Log($"【学术记录】玩家面对 [{choiceText}] 犹豫了 {hesitationSeconds:F2} 秒。");
    }


    public void SaveToLocal()
    {
        string json = JsonUtility.ToJson(_payload, true);
        File.WriteAllText(_localSavePath, json);
    }

    public void UploadDataToServer()
    {
        _payload.current_session.session_end = System.DateTime.UtcNow.ToString("o");
        string json = JsonUtility.ToJson(_payload);
        StartCoroutine(PostJsonData(serverEndpoint, json));
    }

    private IEnumerator PostJsonData(string url, string json)
    {
        Debug.Log("【尝试上传观测数据】...");
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"【数据上传失败】: {request.error} | 数据已保留在本地。");
        }
        else
        {
            Debug.Log("【数据上传成功！】服务器已接收玩家行为图谱。");
        }
    }

    private void OnApplicationQuit()
    {
        LogEvent("game_quit", "app_closed");
        SaveToLocal();
    }
    /// <summary>
    /// new savedata needs to claim the languages background.
    /// </summary>
    public void SetPlayerProfile(string nativeLang, bool isMultilingual, string lockedLang, List<string> secondaryLangs)
    {
        _payload.profile.native_language = nativeLang;
        _payload.profile.is_multilingual = isMultilingual;
        _payload.profile.locked_game_language = lockedLang;
        _payload.profile.secondary_languages = secondaryLangs != null ? new List<string>(secondaryLangs) : new List<string>();

        int currentPlaythroughs = PlayerPrefs.GetInt("Playthroughs", 0) + 1;
        PlayerPrefs.SetInt("Playthroughs", currentPlaythroughs);
        _payload.profile.total_playthroughs = currentPlaythroughs;

        Debug.Log($"【系统校准完毕】母语: {nativeLang} | 多语言者: {isMultilingual} | 锁定语言: {lockedLang} | 当前周目: {currentPlaythroughs}");
        SaveToLocal();
    }
    void Update()
    {
        // for test only
        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log("【test】 U ...");
            UploadDataToServer();
        }
    }
}