using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSystem : MonoBehaviour
{
    public static GameSystem Instance { get; private set; }

    private const int SaveSlotCount = 6;
    private const int CurrentSaveSchemaVersion = 1;
    private const string LegacyMigrationKey = "LegacySavesMigrated_v1";

    public PlayerSaveProfile CurrentSave;
    public bool isLoadingFromSave = false;

    public delegate void LanguageChangeHandler();
    public event LanguageChangeHandler OnLanguageChanged;

    [Header("Audio")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

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
        SetMusicVolume(PlayerPrefs.GetFloat("MusicVol", 0.75f));
        SetSFXVolume(PlayerPrefs.GetFloat("SFXVol", 0.75f));

        if (bgmSource != null && !bgmSource.isPlaying)
        {
            bgmSource.Play();
        }
    }

    void InitializeSystem()
    {
        CurrentSave = CreateNewSaveProfile();
        MigrateLegacySavesForCurrentProfile();

        Debug.Log($"[System] Save directory: {StoragePaths.GetSaveDirectory()}");
        Debug.Log($"[System] System initialized. Default Language: {CurrentSave.languageCode}");
    }

    private PlayerSaveProfile CreateNewSaveProfile()
    {
        return new PlayerSaveProfile
        {
            schemaVersion = CurrentSaveSchemaVersion,
            gameVersion = Application.version,
            playerName = PlayerPrefs.GetString("CurrentUser", "Observer"),
            languageCode = PlayerPrefs.GetString("SelectedLanguage", "EN"),
            bgmVolume = PlayerPrefs.GetFloat("MusicVol", 0.75f),
            sfxVolume = PlayerPrefs.GetFloat("SFXVol", 0.75f)
        };
    }

    public void BeginNewGame()
    {
        CurrentSave = CreateNewSaveProfile();
        isLoadingFromSave = false;
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = volume;
        }

        if (CurrentSave != null)
        {
            CurrentSave.sfxVolume = volume;
        }
    }

    public void SetMusicVolume(float volume)
    {
        if (bgmSource != null)
        {
            bgmSource.volume = volume;
        }

        if (CurrentSave != null)
        {
            CurrentSave.bgmVolume = volume;
        }
    }

    public string GetSavePath(int slotIndex)
    {
        return StoragePaths.GetSavePath(slotIndex);
    }

    public bool HasSaveFile(int slotIndex)
    {
        return File.Exists(GetSavePath(slotIndex));
    }

    public bool HasAnySaveFile()
    {
        for (int i = 0; i < SaveSlotCount; i++)
        {
            if (HasSaveFile(i))
            {
                return true;
            }
        }

        return false;
    }

    public PlayerSaveProfile GetSaveProfile(int slotIndex)
    {
        string path = GetSavePath(slotIndex);
        if (!File.Exists(path))
        {
            return null;
        }

        try
        {
            string json = File.ReadAllText(path, Encoding.UTF8);
            PlayerSaveProfile profile = JsonUtility.FromJson<PlayerSaveProfile>(json);
            return UpgradeSaveProfile(profile);
        }
        catch (Exception exception)
        {
            Debug.LogError($"[System] Failed to read save slot {slotIndex}: {exception.Message}");
            return null;
        }
    }

    public void SaveGame(int slotIndex)
    {
        if (CurrentSave == null)
        {
            CurrentSave = CreateNewSaveProfile();
        }

        CurrentSave.schemaVersion = CurrentSaveSchemaVersion;
        CurrentSave.gameVersion = Application.version;
        CurrentSave.playerName = PlayerPrefs.GetString("CurrentUser", CurrentSave.playerName);
        CurrentSave.currentSceneName = SceneManager.GetActiveScene().name;
        CurrentSave.saveTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
        CurrentSave.languageCode = PlayerPrefs.GetString("SelectedLanguage", CurrentSave.languageCode);
        CurrentSave.bgmVolume = PlayerPrefs.GetFloat("MusicVol", CurrentSave.bgmVolume);
        CurrentSave.sfxVolume = PlayerPrefs.GetFloat("SFXVol", CurrentSave.sfxVolume);

        DialogueController dialogueController = UnityEngine.Object.FindFirstObjectByType<DialogueController>();
        if (dialogueController != null && dialogueController.story != null)
        {
            CurrentSave.inkStoryState = dialogueController.story.state.ToJson();
        }
        else
        {
            Debug.LogWarning("[System] No active DialogueController was found. Ink state was not updated.");
        }

        string path = GetSavePath(slotIndex);
        string json = JsonUtility.ToJson(CurrentSave, true);

        try
        {
            WriteTextSafely(path, json);
            Debug.Log($"[System] Game saved to slot {slotIndex}: {path}");
        }
        catch (Exception exception)
        {
            Debug.LogError($"[System] Failed to save slot {slotIndex}: {exception.Message}");
        }
    }

    public void LoadAndStartGame(int slotIndex)
    {
        PlayerSaveProfile loadedProfile = GetSaveProfile(slotIndex);
        if (loadedProfile == null)
        {
            Debug.LogWarning($"[System] Slot {slotIndex} is empty or unreadable.");
            return;
        }

        CurrentSave = loadedProfile;
        isLoadingFromSave = true;

        if (!string.IsNullOrEmpty(CurrentSave.languageCode))
        {
            PlayerPrefs.SetString("SelectedLanguage", CurrentSave.languageCode);
            PlayerPrefs.Save();
        }

        string sceneToLoad = string.IsNullOrEmpty(CurrentSave.currentSceneName)
            ? "Prologue"
            : CurrentSave.currentSceneName;

        Debug.Log($"[System] Loading slot {slotIndex}. Scene: {sceneToLoad}");
        SceneManager.LoadScene(sceneToLoad);
    }

    public void UploadChoice(string choiceID)
    {
        if (TelemetryManager.Instance != null)
        {
            TelemetryManager.Instance.LogEvent("legacy_choice", choiceID, 0f, "GameSystem.UploadChoice");
        }
        else
        {
            Debug.LogWarning("[System] TelemetryManager is unavailable. Choice was not logged.");
        }
    }

    public void SwitchLanguage(string langCode)
    {
        if (CurrentSave == null)
        {
            CurrentSave = CreateNewSaveProfile();
        }

        CurrentSave.languageCode = langCode;
        PlayerPrefs.SetString("SelectedLanguage", langCode);
        PlayerPrefs.Save();
        OnLanguageChanged?.Invoke();
    }

    public string GetLocalizedString(string key)
    {
        return "L10N_" + key;
    }

    private PlayerSaveProfile UpgradeSaveProfile(PlayerSaveProfile profile)
    {
        if (profile == null)
        {
            return null;
        }

        if (profile.schemaVersion <= 0)
        {
            profile.schemaVersion = CurrentSaveSchemaVersion;
        }

        if (string.IsNullOrEmpty(profile.gameVersion))
        {
            profile.gameVersion = "legacy";
        }

        if (profile.choicesHistory == null)
        {
            profile.choicesHistory = new System.Collections.Generic.List<ChoiceRecord>();
        }

        return profile;
    }

    private void MigrateLegacySavesForCurrentProfile()
    {
        string currentUser = PlayerPrefs.GetString("CurrentUser", "");
        if (string.IsNullOrWhiteSpace(currentUser) || PlayerPrefs.HasKey(LegacyMigrationKey))
        {
            return;
        }

        bool migratedAnySave = false;

        for (int slotIndex = 0; slotIndex < SaveSlotCount; slotIndex++)
        {
            string legacyPath = Path.Combine(Application.persistentDataPath, $"save_data_{slotIndex}.json");
            string profilePath = GetSavePath(slotIndex);

            if (!File.Exists(legacyPath) || File.Exists(profilePath))
            {
                continue;
            }

            try
            {
                File.Copy(legacyPath, profilePath, false);
                migratedAnySave = true;
            }
            catch (Exception exception)
            {
                Debug.LogError($"[System] Failed to migrate legacy slot {slotIndex}: {exception.Message}");
                return;
            }
        }

        PlayerPrefs.SetString(LegacyMigrationKey, currentUser);
        PlayerPrefs.Save();

        if (migratedAnySave)
        {
            Debug.Log($"[System] Legacy saves were migrated to profile '{currentUser}'.");
        }
    }

    private static void WriteTextSafely(string path, string content)
    {
        string directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string temporaryPath = path + ".tmp";
        string backupPath = path + ".bak";
        File.WriteAllText(temporaryPath, content, Encoding.UTF8);

        if (File.Exists(path))
        {
            File.Replace(temporaryPath, path, backupPath, true);
            if (File.Exists(backupPath))
            {
                File.Delete(backupPath);
            }
        }
        else
        {
            File.Move(temporaryPath, path);
        }
    }
}
