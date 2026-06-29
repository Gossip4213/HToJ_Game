using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

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
    public string session_id;
    public string profile_id;
    public string game_version;
    public string session_start;
    public string session_end;
    public List<TelemetryEvent> events = new List<TelemetryEvent>();
}

[System.Serializable]
public class PlayerProfile
{
    public string user_uuid;
    public string subject_id;
    public string native_language;
    public bool is_multilingual;
    public List<string> secondary_languages = new List<string>();
    public string locked_game_language;
    public int total_playthroughs;
}

[System.Serializable]
public class TelemetryPayload
{
    public int schema_version = 1;
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
    private string _activeProfileId;
    private bool _profileConfiguredThisSession;

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
        EnsureAnonymousInstallId();
        StartNewSession(StoragePaths.CurrentProfileId);
    }

    private void EnsureAnonymousInstallId()
    {
        if (!PlayerPrefs.HasKey("PlayerUUID"))
        {
            PlayerPrefs.SetString("PlayerUUID", Guid.NewGuid().ToString());
            PlayerPrefs.SetInt("Playthroughs", 0);
            PlayerPrefs.Save();
        }
    }

    private void StartNewSession(string profileId)
    {
        _activeProfileId = string.IsNullOrWhiteSpace(profileId)
            ? StoragePaths.CurrentProfileId
            : profileId;

        string sessionId = $"{DateTime.UtcNow:yyyyMMdd_HHmmss}_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
        _localSavePath = StoragePaths.GetTelemetrySessionPath(sessionId, _activeProfileId);
        _profileConfiguredThisSession = false;

        _payload = new TelemetryPayload
        {
            schema_version = 1,
            profile = new PlayerProfile
            {
                user_uuid = PlayerPrefs.GetString("PlayerUUID"),
                subject_id = _activeProfileId,
                total_playthroughs = PlayerPrefs.GetInt("Playthroughs", 0)
            },
            current_session = new GameSession
            {
                session_id = sessionId,
                profile_id = _activeProfileId,
                game_version = Application.version,
                session_start = DateTime.UtcNow.ToString("o")
            }
        };

        SaveToLocal();
        Debug.Log($"[Telemetry] Session started: {_localSavePath}");
    }

    public void RefreshForCurrentProfile()
    {
        string currentProfileId = StoragePaths.CurrentProfileId;
        if (currentProfileId == _activeProfileId)
        {
            return;
        }

        FinalizeCurrentSession();
        StartNewSession(currentProfileId);
    }

    public void UpdateSubjectProfile(SubjectProfileData profile)
    {
        if (profile == null)
        {
            return;
        }

        RefreshForCurrentProfile();
        _payload.profile.subject_id = profile.subjectId;
        _payload.profile.native_language = profile.nativeLanguage;
        _payload.profile.is_multilingual = profile.isMultilingual;
        _payload.profile.secondary_languages = profile.secondaryLanguages != null
            ? new List<string>(profile.secondaryLanguages)
            : new List<string>();
        _payload.profile.locked_game_language = profile.lockedGameLanguage;
        SaveToLocal();
    }

    public void LogEvent(string type, string target, float duration = 0f, string extra = "")
    {
        if (_payload == null || _payload.current_session == null)
        {
            InitTelemetry();
        }

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
        if (_payload == null || string.IsNullOrEmpty(_localSavePath))
        {
            return;
        }

        try
        {
            string json = JsonUtility.ToJson(_payload, true);
            WriteTextSafely(_localSavePath, json);
        }
        catch (Exception exception)
        {
            Debug.LogError($"[Telemetry] Local backup failed: {exception.Message}");
        }
    }

    public void UploadDataToServer()
    {
        if (_payload == null)
        {
            Debug.LogWarning("[Telemetry] No payload is available to upload.");
            return;
        }

        _payload.current_session.session_end = DateTime.UtcNow.ToString("o");
        SaveToLocal();

        string anonymousInstallId = _payload.profile.user_uuid;
        string subjectId = _payload.profile.subject_id;
        string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
        string fullJsonData = JsonUtility.ToJson(_payload);

        StartCoroutine(PostToGoogleForm(anonymousInstallId, subjectId, timestamp, fullJsonData));
    }

    private IEnumerator PostToGoogleForm(string anonymousId, string subjectId, string time, string json)
    {
        Debug.Log("[Telemetry] Uploading session data...");

        WWWForm form = new WWWForm();
        form.AddField(entryID_DeviceID, anonymousId);
        form.AddField(entryID_SubjectID, subjectId);
        form.AddField(entryID_Timestamp, time);
        form.AddField(entryID_FullData, json);

        using (UnityWebRequest request = UnityWebRequest.Post(googleFormUrl, form))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[Telemetry] Upload failed: {request.error}. Local backup remains available.");
            }
            else
            {
                Debug.Log("[Telemetry] Upload completed successfully.");
            }
        }
    }

    private void OnApplicationQuit()
    {
        if (_payload == null)
        {
            return;
        }

        _payload.current_session.events.Add(new TelemetryEvent
        {
            timestamp = DateTime.UtcNow.ToString("o"),
            event_type = "game_quit",
            target_id = "app_closed",
            duration_sec = 0f,
            extra_data = ""
        });

        FinalizeCurrentSession();
    }

    public void SetPlayerProfile(string nativeLang, bool isMultilingual, string lockedLang, List<string> secondaryLangs)
    {
        RefreshForCurrentProfile();

        _payload.profile.native_language = nativeLang;
        _payload.profile.is_multilingual = isMultilingual;
        _payload.profile.locked_game_language = lockedLang;
        _payload.profile.secondary_languages = secondaryLangs != null
            ? new List<string>(secondaryLangs)
            : new List<string>();

        if (!_profileConfiguredThisSession)
        {
            int currentPlaythroughs = PlayerPrefs.GetInt("Playthroughs", 0) + 1;
            PlayerPrefs.SetInt("Playthroughs", currentPlaythroughs);
            PlayerPrefs.Save();
            _payload.profile.total_playthroughs = currentPlaythroughs;
            _profileConfiguredThisSession = true;
        }

        SaveToLocal();
    }

    private void FinalizeCurrentSession()
    {
        if (_payload == null || _payload.current_session == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(_payload.current_session.session_end))
        {
            _payload.current_session.session_end = DateTime.UtcNow.ToString("o");
        }

        SaveToLocal();
    }

    private static void WriteTextSafely(string path, string content)
    {
        string directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string temporaryPath = path + ".tmp";
        File.WriteAllText(temporaryPath, content, Encoding.UTF8);

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        File.Move(temporaryPath, path);
    }
}
