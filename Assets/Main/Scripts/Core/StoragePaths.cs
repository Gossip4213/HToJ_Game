using System;
using System.IO;
using UnityEngine;

public static class StoragePaths
{
    private const string ProfilesDirectoryName = "profiles";
    private const string SavesDirectoryName = "saves";
    private const string TelemetryDirectoryName = "telemetry";
    private const string DefaultProfileId = "DefaultProfile";

    public static string CurrentProfileId
    {
        get
        {
            string profileId = PlayerPrefs.GetString("CurrentUser", DefaultProfileId);
            return SanitizeProfileId(profileId);
        }
    }

    public static string GetProfileDirectory(string profileId = null)
    {
        string safeProfileId = string.IsNullOrWhiteSpace(profileId)
            ? CurrentProfileId
            : SanitizeProfileId(profileId);

        string path = Path.Combine(Application.persistentDataPath, ProfilesDirectoryName, safeProfileId);
        Directory.CreateDirectory(path);
        return path;
    }

    public static string GetSaveDirectory(string profileId = null)
    {
        string path = Path.Combine(GetProfileDirectory(profileId), SavesDirectoryName);
        Directory.CreateDirectory(path);
        return path;
    }

    public static string GetSavePath(int slotIndex, string profileId = null)
    {
        if (slotIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(slotIndex), "Save slot index cannot be negative.");
        }

        return Path.Combine(GetSaveDirectory(profileId), $"slot_{slotIndex}.json");
    }

    public static string GetTelemetryDirectory(string profileId = null)
    {
        string path = Path.Combine(GetProfileDirectory(profileId), TelemetryDirectoryName);
        Directory.CreateDirectory(path);
        return path;
    }

    public static string GetTelemetrySessionPath(string sessionId, string profileId = null)
    {
        string safeSessionId = string.IsNullOrWhiteSpace(sessionId)
            ? Guid.NewGuid().ToString("N")
            : SanitizeFileName(sessionId);

        return Path.Combine(GetTelemetryDirectory(profileId), $"session_{safeSessionId}.json");
    }

    private static string SanitizeProfileId(string profileId)
    {
        string value = string.IsNullOrWhiteSpace(profileId) ? DefaultProfileId : profileId.Trim();
        return SanitizeFileName(value);
    }

    private static string SanitizeFileName(string value)
    {
        foreach (char invalidChar in Path.GetInvalidFileNameChars())
        {
            value = value.Replace(invalidChar, '_');
        }

        return string.IsNullOrWhiteSpace(value) ? DefaultProfileId : value;
    }
}
