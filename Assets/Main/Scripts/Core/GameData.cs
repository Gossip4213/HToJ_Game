using System.Collections.Generic;

[System.Serializable]
public class ChoiceRecord
{
    public string choiceId;
    public string choiceText;
    public string chapterId;
    public string timestamp;
}

[System.Serializable]
public class PlayerSaveProfile
{
    public int schemaVersion = 1;
    public string gameVersion;

    public string playerName = "Observer";
    public string languageCode = "ZH_CN";
    public float bgmVolume = 1.0f;
    public float sfxVolume = 1.0f;

    public string currentSceneName;
    public string currentScriptID;
    public string saveTime;
    public string inkStoryState;
    public int currentChapterIndex = 0;
    public List<ChoiceRecord> choicesHistory = new List<ChoiceRecord>();
}

[System.Serializable]
public class ObservationLog
{
    public string userId;
    public string choiceId;
    public string timestamp;
}
