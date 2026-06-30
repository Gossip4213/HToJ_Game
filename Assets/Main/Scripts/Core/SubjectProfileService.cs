using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class SubjectProfileData
{
    public int schemaVersion = 1;
    public string subjectId;
    public string nativeLanguage;
    public bool isMultilingual;
    public List<string> secondaryLanguages = new List<string>();
    public string lockedGameLanguage = "EN";
    public string createdAtUtc;
    public string updatedAtUtc;
    public bool archived;
}

public static class SubjectProfileService
{
    private const string AllProfilesKey = "AllProfiles";
    private const string CurrentUserKey = "CurrentUser";
    private const string ProfileFileName = "profile.json";

    public static void EnsureLegacyProfiles()
    {
        List<string> registeredIds = ReadRegisteredProfileIds();
        string currentUser = PlayerPrefs.GetString(CurrentUserKey, string.Empty);

        if (!string.IsNullOrWhiteSpace(currentUser) && !registeredIds.Contains(currentUser))
        {
            registeredIds.Add(currentUser);
            WriteRegisteredProfileIds(registeredIds);
        }

        foreach (string subjectId in registeredIds)
        {
            if (!File.Exists(GetProfilePath(subjectId)))
            {
                SaveProfile(CreateLegacyProfile(subjectId));
            }
        }

        List<string> activeIds = GetActiveProfileIds();
        if (activeIds.Count == 0)
        {
            PlayerPrefs.DeleteKey(CurrentUserKey);
        }
        else if (string.IsNullOrWhiteSpace(currentUser) || !activeIds.Contains(currentUser))
        {
            PlayerPrefs.SetString(CurrentUserKey, activeIds[0]);
        }

        PlayerPrefs.Save();
    }

    public static bool HasAnyActiveProfile()
    {
        return GetActiveProfileIds().Count > 0;
    }

    public static List<string> GetActiveProfileIds()
    {
        List<string> activeIds = new List<string>();

        foreach (string subjectId in ReadRegisteredProfileIds())
        {
            SubjectProfileData profile = LoadProfile(subjectId, createIfMissing: true);
            if (profile != null && !profile.archived)
            {
                activeIds.Add(subjectId);
            }
        }

        return activeIds;
    }

    public static string GetCurrentProfileId()
    {
        string currentUser = PlayerPrefs.GetString(CurrentUserKey, string.Empty);
        if (!string.IsNullOrWhiteSpace(currentUser) && GetActiveProfileIds().Contains(currentUser))
        {
            return currentUser;
        }

        List<string> activeIds = GetActiveProfileIds();
        return activeIds.Count > 0 ? activeIds[0] : string.Empty;
    }

    public static bool SetCurrentProfile(string subjectId)
    {
        if (string.IsNullOrWhiteSpace(subjectId))
        {
            return false;
        }

        SubjectProfileData profile = LoadProfile(subjectId, createIfMissing: false);
        if (profile == null || profile.archived)
        {
            return false;
        }

        PlayerPrefs.SetString(CurrentUserKey, subjectId);
        PlayerPrefs.Save();
        return true;
    }

    public static SubjectProfileData CreateProfile(
        string nativeLanguage,
        bool isMultilingual,
        IEnumerable<string> secondaryLanguages,
        string lockedGameLanguage)
    {
        string now = DateTime.UtcNow.ToString("o");
        SubjectProfileData profile = new SubjectProfileData
        {
            schemaVersion = 1,
            subjectId = GenerateNextSubjectId(),
            nativeLanguage = nativeLanguage ?? string.Empty,
            isMultilingual = isMultilingual,
            secondaryLanguages = NormalizeLanguages(secondaryLanguages),
            lockedGameLanguage = string.IsNullOrWhiteSpace(lockedGameLanguage) ? "EN" : lockedGameLanguage,
            createdAtUtc = now,
            updatedAtUtc = now,
            archived = false
        };

        RegisterProfileId(profile.subjectId);
        SaveProfile(profile);
        SetCurrentProfile(profile.subjectId);
        return profile;
    }

    public static SubjectProfileData LoadProfile(string subjectId, bool createIfMissing = true)
    {
        if (string.IsNullOrWhiteSpace(subjectId))
        {
            return null;
        }

        string path = GetProfilePath(subjectId);
        if (!File.Exists(path))
        {
            if (!createIfMissing)
            {
                return null;
            }

            SubjectProfileData legacyProfile = CreateLegacyProfile(subjectId);
            SaveProfile(legacyProfile);
            return legacyProfile;
        }

        try
        {
            string json = File.ReadAllText(path, Encoding.UTF8);
            SubjectProfileData profile = JsonUtility.FromJson<SubjectProfileData>(json);
            if (profile == null)
            {
                return null;
            }

            profile.subjectId = subjectId;
            profile.secondaryLanguages ??= new List<string>();
            return profile;
        }
        catch (Exception exception)
        {
            Debug.LogError($"[SubjectProfile] Failed to read '{subjectId}': {exception.Message}");
            return null;
        }
    }

    public static bool SaveProfile(SubjectProfileData profile)
    {
        if (profile == null || string.IsNullOrWhiteSpace(profile.subjectId))
        {
            return false;
        }

        profile.schemaVersion = 1;
        profile.secondaryLanguages = NormalizeLanguages(profile.secondaryLanguages);
        profile.updatedAtUtc = DateTime.UtcNow.ToString("o");
        if (string.IsNullOrWhiteSpace(profile.createdAtUtc))
        {
            profile.createdAtUtc = profile.updatedAtUtc;
        }

        try
        {
            RegisterProfileId(profile.subjectId);
            string json = JsonUtility.ToJson(profile, true);
            WriteTextSafely(GetProfilePath(profile.subjectId), json);
            return true;
        }
        catch (Exception exception)
        {
            Debug.LogError($"[SubjectProfile] Failed to save '{profile.subjectId}': {exception.Message}");
            return false;
        }
    }

    public static bool ArchiveProfile(string subjectId)
    {
        SubjectProfileData profile = LoadProfile(subjectId, createIfMissing: false);
        if (profile == null)
        {
            return false;
        }

        profile.archived = true;
        if (!SaveProfile(profile))
        {
            return false;
        }

        if (PlayerPrefs.GetString(CurrentUserKey, string.Empty) == subjectId)
        {
            List<string> remaining = GetActiveProfileIds();
            if (remaining.Count > 0)
            {
                PlayerPrefs.SetString(CurrentUserKey, remaining[0]);
            }
            else
            {
                PlayerPrefs.DeleteKey(CurrentUserKey);
            }

            PlayerPrefs.Save();
        }

        return true;
    }

    private static SubjectProfileData CreateLegacyProfile(string subjectId)
    {
        string now = DateTime.UtcNow.ToString("o");
        return new SubjectProfileData
        {
            schemaVersion = 1,
            subjectId = subjectId,
            nativeLanguage = string.Empty,
            isMultilingual = false,
            secondaryLanguages = new List<string>(),
            lockedGameLanguage = PlayerPrefs.GetString("SelectedLanguage", "EN"),
            createdAtUtc = now,
            updatedAtUtc = now,
            archived = false
        };
    }

    private static string GenerateNextSubjectId()
    {
        int highestId = 0;
        foreach (string subjectId in ReadRegisteredProfileIds())
        {
            const string prefix = "Subject_";
            if (subjectId.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) &&
                int.TryParse(subjectId.Substring(prefix.Length), out int parsedId))
            {
                highestId = Math.Max(highestId, parsedId);
            }
        }

        return $"Subject_{highestId + 1:D3}";
    }

    private static string GetProfilePath(string subjectId)
    {
        return Path.Combine(StoragePaths.GetProfileDirectory(subjectId), ProfileFileName);
    }

    private static List<string> ReadRegisteredProfileIds()
    {
        string rawProfiles = PlayerPrefs.GetString(AllProfilesKey, string.Empty);
        return rawProfiles
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(profileId => profileId.Trim())
            .Where(profileId => !string.IsNullOrWhiteSpace(profileId))
            .Distinct(StringComparer.Ordinal)
            .ToList();
    }

    private static void RegisterProfileId(string subjectId)
    {
        List<string> ids = ReadRegisteredProfileIds();
        if (!ids.Contains(subjectId))
        {
            ids.Add(subjectId);
            WriteRegisteredProfileIds(ids);
        }
    }

    private static void WriteRegisteredProfileIds(IEnumerable<string> subjectIds)
    {
        string serializedIds = string.Join(",", subjectIds.Distinct(StringComparer.Ordinal));
        PlayerPrefs.SetString(AllProfilesKey, serializedIds);
        PlayerPrefs.Save();
    }

    private static List<string> NormalizeLanguages(IEnumerable<string> languages)
    {
        if (languages == null)
        {
            return new List<string>();
        }

        return languages
            .Where(language => !string.IsNullOrWhiteSpace(language))
            .Select(language => language.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
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
