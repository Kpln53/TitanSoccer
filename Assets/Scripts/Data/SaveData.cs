using System;
using UnityEngine;

/// <summary>
/// Genişletilmiş Save Data - Tüm oyun verilerini içerir
/// </summary>
[Serializable]
public class SaveData
{
    [Header("Player Profile")]
    public PlayerProfile playerProfile = new PlayerProfile();
    
    [Header("Club Data")]
    public ClubData clubData = new ClubData();
    
    [Header("Season Data")]
    public SeasonData seasonData = new SeasonData();
    
    [Header("Relations Data")]
    public RelationsData relationsData = new RelationsData();
    
    [Header("Economy Data")]
    public EconomyData economyData = new EconomyData();
    
    [Header("Media Data")]
    public MediaData mediaData = new MediaData();
    
    [Header("Data Pack References")]
    public string activeDataPackId = "";
    public string dataPackVersion = "";
    
    // Eski uyumluluk için (deprecated - kullanılmayacak)
    [System.Obsolete("Use playerProfile instead")]
    public string playerName;
    [System.Obsolete("Use playerProfile instead")]
    public string position;
    [System.Obsolete("Use clubData instead")]
    public string clubName;
    [System.Obsolete("Use clubData instead")]
    public string leagueName;
    [System.Obsolete("Use seasonData instead")]
    public int season;
    [System.Obsolete("Use seasonData instead")]
    public int leaguePosition;
    [System.Obsolete("Use playerProfile instead")]
    public int overall;
}
