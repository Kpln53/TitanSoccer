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
    
    [Header("Logo")]
    public Sprite leagueLogo; // Lig logosu
    
    [Header("Lig Gücü")]
    [Range(0, 100)] public int leaguePower = 50; // Lig gücü (0-100, transfer sistemi için kullanılacak)
    
    [Header("Takımlar")]
    public List<TeamData> teams = new List<TeamData>();
    
    /// <summary>
    /// Lig gücünü hesapla (tüm takımların ortalama gücü) - leaguePower field'ını günceller
    /// </summary>
    public void CalculateLeaguePower()
    {
        if (teams == null || teams.Count == 0)
        {
            leaguePower = 0;
            return;
        }
        
        int totalPower = 0;
        int validTeams = 0;
        
        foreach (var team in teams)
        {
            if (team != null)
            {
                totalPower += team.GetTeamPower();
                validTeams++;
            }
        }
        
        if (validTeams > 0)
        {
            leaguePower = Mathf.RoundToInt((float)totalPower / validTeams);
        }
        else
        {
            leaguePower = 0;
        }
    }
    
    /// <summary>
    /// Lig gücünü getir (float olarak - eski uyumluluk için)
    /// </summary>
    public float GetLeaguePowerFloat()
    {
        return (float)leaguePower;
    }
    
    /// <summary>
    /// Belirli bir takımı isimle bul
    /// </summary>
    public TeamData GetTeamByName(string teamName)
    {
        if (teams == null)
            return null;
        
        foreach (var team in teams)
        {
            if (team != null && team.teamName == teamName)
            {
                return team;
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// Takım sayısını getir
    /// </summary>
    public int GetTeamCount()
    {
        return teams != null ? teams.Count : 0;
    }
    
    /// <summary>
    /// Tüm oyuncuları getir (tüm takımlardan)
    /// </summary>
    public List<PlayerData> GetAllPlayers()
    {
        List<PlayerData> allPlayers = new List<PlayerData>();
        
        if (teams == null)
            return allPlayers;
        
        foreach (var team in teams)
        {
            if (team != null && team.players != null)
            {
                allPlayers.AddRange(team.players);
            }
        }
        
        return allPlayers;
    }
}

