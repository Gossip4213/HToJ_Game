using System.Collections.Generic;

[System.Serializable]
public class PlayerSaveProfile
{
    public string playerName = "Observer";
    public string languageCode = "ZH_CN";
    public float bgmVolume = 1.0f;
    public float sfxVolume = 1.0f;

    public string currentSceneName; 
    public string currentScriptID;  
    public string saveTime;         

    public int currentChapterIndex = 0;
    public Dictionary<string, string> choicesHistory = new Dictionary<string, string>();
}

[System.Serializable]
public class ObservationLog
{
    public string userId;
    public string choiceId;
    public string timestamp;
}