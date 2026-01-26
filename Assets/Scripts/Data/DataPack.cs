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
    public string packId; // Unique ID (paket adından otomatik oluşturulabilir)
    public string packVersion = "1.0.0";
    public string packAuthor = "Unknown";
    [TextArea(3, 5)]
    public string packDescription = "";
    
    [Header("Logo")]
    public Sprite packLogo; // Data Pack logosu
    
    [Header("Ligler")]
    public List<LeagueData> leagues = new List<LeagueData>();

    [Header("Turnuvalar")]
    public List<TournamentData> tournaments = new List<TournamentData>();
    
    [Header("Standalone Takımlar (Lig dışı)")]
    public List<TeamData> standaloneTeams = new List<TeamData>();

    [Header("Ezeli Rakipler (Derbiler)")]
    public List<RivalryData> rivalries = new List<RivalryData>();
    
    /// <summary>
    /// Tüm takımları getir (liglerden + standalone)
    /// </summary>
    public List<TeamData> GetAllTeams()
    {
        List<TeamData> allTeams = new List<TeamData>();
        
        // Liglerden takımları ekle
        if (leagues != null)
        {
            foreach (var league in leagues)
            {
                if (league != null && league.teams != null)
                {
                    allTeams.AddRange(league.teams);
                }
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
        if (leagues != null)
        {
            foreach (var league in leagues)
            {
                if (league != null)
                {
                    TeamData team = league.GetTeamByName(teamName);
                    if (team != null)
                        return team;
                }
            }
        }
        
        // Sonra standalone takımlarda ara
        if (standaloneTeams != null)
        {
            foreach (var team in standaloneTeams)
            {
                if (team != null && team.teamName == teamName)
                    return team;
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// Belirli bir ligi isimle bul
    /// </summary>
    public LeagueData GetLeagueByName(string leagueName)
    {
        if (leagues == null)
            return null;
        
        foreach (var league in leagues)
        {
            if (league != null && league.leagueName == leagueName)
                return league;
        }
        
        return null;
    }
    
    /// <summary>
    /// Tüm takımların güçlerini hesapla
    /// </summary>
    public void CalculateAllTeamPowers()
    {
        // Liglerdeki takımlar
        if (leagues != null)
        {
            foreach (var league in leagues)
            {
                if (league != null && league.teams != null)
                {
                    foreach (var team in league.teams)
                    {
                        if (team != null)
                        {
                            team.CalculateTeamPower();
                        }
                    }
                }
            }
        }
        
        // Standalone takımlar
        if (standaloneTeams != null)
        {
            foreach (var team in standaloneTeams)
            {
                if (team != null)
                {
                    team.CalculateTeamPower();
                }
            }
        }
    }
    
    /// <summary>
    /// Toplam takım sayısını getir
    /// </summary>
    public int GetTotalTeamCount()
    {
        int count = 0;
        
        if (leagues != null)
        {
            foreach (var league in leagues)
            {
                if (league != null)
                {
                    count += league.GetTeamCount();
                }
            }
        }
        
        if (standaloneTeams != null)
        {
            count += standaloneTeams.Count;
        }
        
        return count;
    }
    
    /// <summary>
    /// Toplam oyuncu sayısını getir
    /// </summary>
    public int GetTotalPlayerCount()
    {
        int count = 0;
        
        List<TeamData> allTeams = GetAllTeams();
        
        foreach (var team in allTeams)
        {
            if (team != null && team.players != null)
            {
                count += team.players.Count;
            }
        }
        
        return count;
    }
}

