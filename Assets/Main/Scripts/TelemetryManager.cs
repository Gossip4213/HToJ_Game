using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Text;
using System;

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

    [Header("永久服务器配置 (Google Form)")]
    public string googleFormUrl = "https://docs.google.com/forms/d/e/1FAIpQLSdkCkfbXt42BpSWf1-iaWgH0jOTxLP5gm9BCwjGmaqz_OiJJQ/formResponse";

    [Header("表单 Entry ID 映射")]
    public string entryID_DeviceID = "entry.1219463742";
    public string entryID_SubjectID = "entry.456655523";
    public string entryID_Timestamp = "entry.591917006";
    public string entryID_FullData = "entry.1734290061";

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
            PlayerPrefs.SetString("PlayerUUID", Guid.NewGuid().ToString());
            PlayerPrefs.SetInt("Playthroughs", 1);
        }
        _payload.profile.user_uuid = PlayerPrefs.GetString("PlayerUUID");
        _payload.profile.total_playthroughs = PlayerPrefs.GetInt("Playthroughs");
        _payload.current_session.session_start = DateTime.UtcNow.ToString("o");

        Debug.Log($"【系统启动】数据备份路径: {_localSavePath}");
    }

    public void LogEvent(string type, string target, float duration = 0f, string extra = "")
    {
        TelemetryEvent newEvent = new TelemetryEvent
        {
            timestamp = DateTime.UtcNow.ToString("o"),
            event_type = type,
            target_id = target,
            duration_sec = duration,
            extra_data = extra
        };
        _payload.current_session.events.Add(newEvent);
        SaveToLocal();
    }

    public void LogChoiceHesitation(string choiceText, float hesitationSeconds)
    {
        LogEvent("choice_made", choiceText, hesitationSeconds, "ink_choice");
    }

    public void SaveToLocal()
    {
        string json = JsonUtility.ToJson(_payload, true);
        File.WriteAllText(_localSavePath, json);
    }

    public void UploadDataToServer()
    {
        _payload.current_session.session_end = DateTime.UtcNow.ToString("o");

        string deviceID = SystemInfo.deviceUniqueIdentifier; // 识别物理PC
        string subjectID = PlayerPrefs.GetString("CurrentUser", "None");
        string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
        string fullJsonData = JsonUtility.ToJson(_payload);

        StartCoroutine(PostToGoogleForm(deviceID, subjectID, timestamp, fullJsonData));
    }

    private IEnumerator PostToGoogleForm(string devID, string subID, string time, string json)
    {
        Debug.Log("【永久存储】正在同步数据至 Google Sheets...");

        WWWForm form = new WWWForm();
        form.AddField(entryID_DeviceID, devID);
        form.AddField(entryID_SubjectID, subID);
        form.AddField(entryID_Timestamp, time);
        form.AddField(entryID_FullData, json);

        using (UnityWebRequest request = UnityWebRequest.Post(googleFormUrl, form))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"【存储失败】: {request.error} | 数据已在本地备份。");
            }
            else
            {
            }
        }
    }

    private void OnApplicationQuit()
    {
        LogEvent("game_quit", "app_closed");
        SaveToLocal();
    }

    public void SetPlayerProfile(string nativeLang, bool isMultilingual, string lockedLang, List<string> secondaryLangs)
    {
        _payload.profile.native_language = nativeLang;
        _payload.profile.is_multilingual = isMultilingual;
        _payload.profile.locked_game_language = lockedLang;
        _payload.profile.secondary_languages = secondaryLangs != null ? new List<string>(secondaryLangs) : new List<string>();

        int currentPlaythroughs = PlayerPrefs.GetInt("Playthroughs", 0) + 1;
        PlayerPrefs.SetInt("Playthroughs", currentPlaythroughs);
        _payload.profile.total_playthroughs = currentPlaythroughs;

        SaveToLocal();
    }
}