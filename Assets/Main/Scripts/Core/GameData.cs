using System.Collections.Generic;

[System.Serializable]
public class PlayerSaveProfile
{
    public string playerName = "Observer";
    public int currentChapterIndex = 0;

    public Dictionary<string, string> choicesHistory = new Dictionary<string, string>(); 
    
    public float bgmVolume = 1.0f;
    public float sfxVolume = 1.0f;
    public string languageCode = "ZH_CN"; 
}

[System.Serializable]
public class ObservationLog
{
    public string userId; 
    public string choiceId; 
    public string timestamp; 
}