using UnityEngine;

/// <summary>
/// Kulüp verileri
/// </summary>
[System.Serializable]
public class ClubData
{
    public string clubId; // Data Pack'teki takım ID'si
    public string clubName;
    public string leagueId;
    public string leagueName;
    
    [Header("Sözleşme")]
    public ContractData contract = new ContractData();
    
    [Header("Kulüp Hedefi")]
    public ClubObjective objective = ClubObjective.StayInLeague;
    
    [Header("Yönetim Memnuniyeti")]
    [Range(0f, 1f)] public float managementSatisfaction = 0.7f;
}

/// <summary>
/// Sözleşme verileri
/// </summary>
[System.Serializable]
public class ContractData
{
    public int monthlySalary = 10000; // Aylık maaş (Euro)
    public int contractDuration = 12; // Ay cinsinden
    public int remainingMonths = 12;
    
    [Header("Bonuslar")]
    public int goalBonus = 1000;
    public int assistBonus = 500;
    public int winBonus = 2000;
    public int cleanSheetBonus = 1500; // Kaleci/defans için
    
    [Header("Bonus Hedefleri")]
    public int goalTarget = 10;
    public int assistTarget = 5;
    public int winTarget = 15;
    public int cleanSheetTarget = 5;
    
    [Header("Oynama Zamanı")]
    public PlayingTime playingTime = PlayingTime.Rotation;
}

public enum PlayingTime
{
    Starter,      // İlk 11
    Rotation,     // Rotasyon
    Substitute    // Yedek
}

public enum ClubObjective
{
    StayInLeague,  // Kümede kal
    Playoff,       // Playoff
    Top5,          // İlk 5
    Championship,  // Şampiyonluk
    YouthDevelopment // Genç oynat
}


