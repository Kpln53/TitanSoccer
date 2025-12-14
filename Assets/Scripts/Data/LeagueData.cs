using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Lig veri yapısı - Data Pack sistemi için
/// </summary>
[System.Serializable]
public class LeagueData
{
    [Header("Temel Bilgiler")]
    public string leagueName;
    public string leagueCountry = "Unknown";
    public int leagueTier = 1; // 1 = En üst lig, 2 = İkinci lig, vs.
    
    [Header("Takımlar")]
    public List<TeamData> teams = new List<TeamData>();
    
    /// <summary>
    /// Lig gücünü hesapla (tüm takımların ortalama gücü)
    /// </summary>
    public float CalculateLeaguePower()
    {
        if (teams == null || teams.Count == 0)
            return 0f;
        
        int totalPower = 0;
        foreach (var team in teams)
        {
            totalPower += team.GetTeamPower();
        }
        
        return (float)totalPower / teams.Count;
    }
    
    /// <summary>
    /// Belirli bir takımı isimle bul
    /// </summary>
    public TeamData GetTeamByName(string teamName)
    {
        foreach (var team in teams)
        {
            if (team.teamName == teamName)
                return team;
        }
        return null;
    }
}

