using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Data Pack - Tüm takımlar, ligler ve oyuncuları içeren ana data dosyası
/// </summary>
[CreateAssetMenu(fileName = "DataPack", menuName = "TitanSoccer/Data Pack")]
public class DataPack : ScriptableObject
{
    [Header("Data Pack Bilgileri")]
    public string packName = "Default Pack";
    public string packVersion = "1.0.0";
    public string packAuthor = "Unknown";
    public string packDescription = "";
    
    [Header("Ligler")]
    public List<LeagueData> leagues = new List<LeagueData>();
    
    [Header("Standalone Takımlar (Lig dışı)")]
    public List<TeamData> standaloneTeams = new List<TeamData>();
    
    /// <summary>
    /// Tüm takımları getir (liglerden + standalone)
    /// </summary>
    public List<TeamData> GetAllTeams()
    {
        List<TeamData> allTeams = new List<TeamData>();
        
        // Liglerden takımları ekle
        foreach (var league in leagues)
        {
            if (league.teams != null)
            {
                allTeams.AddRange(league.teams);
            }
        }
        
        // Standalone takımları ekle
        if (standaloneTeams != null)
        {
            allTeams.AddRange(standaloneTeams);
        }
        
        return allTeams;
    }
    
    /// <summary>
    /// Belirli bir takımı isimle bul
    /// </summary>
    public TeamData GetTeamByName(string teamName)
    {
        // Önce liglerde ara
        foreach (var league in leagues)
        {
            TeamData team = league.GetTeamByName(teamName);
            if (team != null)
                return team;
        }
        
        // Sonra standalone takımlarda ara
        foreach (var team in standaloneTeams)
        {
            if (team.teamName == teamName)
                return team;
        }
        
        return null;
    }
    
    /// <summary>
    /// Belirli bir ligi isimle bul
    /// </summary>
    public LeagueData GetLeagueByName(string leagueName)
    {
        foreach (var league in leagues)
        {
            if (league.leagueName == leagueName)
                return league;
        }
        return null;
    }
    
    /// <summary>
    /// Tüm takımların güçlerini hesapla
    /// </summary>
    public void CalculateAllTeamPowers()
    {
        foreach (var league in leagues)
        {
            foreach (var team in league.teams)
            {
                team.CalculateTeamPower();
            }
        }
        
        foreach (var team in standaloneTeams)
        {
            team.CalculateTeamPower();
        }
    }
}

