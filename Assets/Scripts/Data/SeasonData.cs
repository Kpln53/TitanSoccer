using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Takım puan durumu verisi
/// </summary>
[System.Serializable]
public class TeamStandingData
{
    public string teamName;
    public int played = 0;       // Oynanan maç
    public int wins = 0;         // Galibiyet
    public int draws = 0;        // Beraberlik
    public int losses = 0;       // Mağlubiyet
    public int goalsFor = 0;     // Atılan gol
    public int goalsAgainst = 0; // Yenilen gol
    public int points = 0;       // Puan
    
    public int GoalDifference => goalsFor - goalsAgainst;
    
    public TeamStandingData() { }
    
    public TeamStandingData(string name)
    {
        teamName = name;
    }
    
    /// <summary>
    /// Maç sonucunu kaydet
    /// </summary>
    public void RecordMatch(int scored, int conceded)
    {
        played++;
        goalsFor += scored;
        goalsAgainst += conceded;
        
        if (scored > conceded)
        {
            wins++;
            points += 3;
        }
        else if (scored == conceded)
        {
            draws++;
            points += 1;
        }
        else
        {
            losses++;
        }
    }
}

/// <summary>
/// Sezon verileri - Mevcut sezon istatistikleri
/// </summary>
[System.Serializable]
public class SeasonData
{
    [Header("Sezon Bilgileri")]
    public int seasonNumber;           // Sezon numarası (1, 2, 3...)
    public string seasonName;          // Sezon adı (örn: "2025-2026")
    
    [Header("Maç İstatistikleri")]
    public int matchesPlayed = 0;      // Oynanan maç sayısı
    public int goals = 0;              // Gol sayısı
    public int assists = 0;            // Asist sayısı
    public int yellowCards = 0;        // Sarı kart sayısı
    public int redCards = 0;           // Kırmızı kart sayısı
    
    [Header("Rating")]
    public List<float> matchRatings;   // Maç rating'leri (her maç için)
    public float averageRating = 0f;   // Ortalama rating
    
    [Header("Fikstür")]
    public List<MatchData> fixtures;   // Sezon maç programı
    public int currentWeek = 1;        // Mevcut hafta
    
    [Header("Lig Puan Durumu")]
    public List<TeamStandingData> standings; // Lig puan durumu
    
    [Header("Takım İstatistikleri")]
    public int leaguePosition = 0;     // Lig pozisyonu
    public int leaguePoints = 0;       // Lig puanı
    public int wins = 0;               // Galibiyet
    public int draws = 0;              // Beraberlik
    public int losses = 0;             // Mağlubiyet
    
    [Header("Tarih")]
    public DateTime seasonStartDate;   // Sezon başlangıç tarihi
    public string seasonStartDateString;
    public DateTime seasonEndDate;     // Sezon bitiş tarihi
    public string seasonEndDateString;

    [Header("Oyuncu Durumu")]
    public float energy = 100f;        // Oyuncu enerjisi (0-100)
    public float moral = 100f;         // Oyuncu morali (0-100)
    
    public SeasonData()
    {
        seasonNumber = 1;
        seasonName = "2025-2026";
        matchRatings = new List<float>();
        fixtures = new List<MatchData>();
        standings = new List<TeamStandingData>();
        seasonStartDate = DateTime.Now;
        seasonStartDateString = seasonStartDate.ToString("yyyy-MM-dd");
        seasonEndDate = seasonStartDate.AddMonths(9); // ~9 ay sezon
        seasonEndDateString = seasonEndDate.ToString("yyyy-MM-dd");
        currentWeek = 1;
    }
    
    /// <summary>
    /// Puan durumunu takım listesinden başlat
    /// </summary>
    public void InitializeStandings(List<TeamData> teams)
    {
        standings = new List<TeamStandingData>();
        foreach (var team in teams)
        {
            if (team != null && !string.IsNullOrEmpty(team.teamName))
            {
                standings.Add(new TeamStandingData(team.teamName));
            }
        }
        Debug.Log($"[SeasonData] Standings initialized for {standings.Count} teams.");
    }
    
    /// <summary>
    /// Maç sonucunu puan durumuna kaydet
    /// </summary>
    public void RecordMatchResult(string homeTeam, string awayTeam, int homeScore, int awayScore)
    {
        if (standings == null) standings = new List<TeamStandingData>();
        
        // Ev sahibi takım
        var homeSt = standings.FirstOrDefault(s => s.teamName == homeTeam);
        if (homeSt == null)
        {
            homeSt = new TeamStandingData(homeTeam);
            standings.Add(homeSt);
        }
        homeSt.RecordMatch(homeScore, awayScore);
        
        // Deplasman takım
        var awaySt = standings.FirstOrDefault(s => s.teamName == awayTeam);
        if (awaySt == null)
        {
            awaySt = new TeamStandingData(awayTeam);
            standings.Add(awaySt);
        }
        awaySt.RecordMatch(awayScore, homeScore);
        
        Debug.Log($"[SeasonData] Match result recorded: {homeTeam} {homeScore}-{awayScore} {awayTeam}");
    }
    
    /// <summary>
    /// Sıralanmış puan durumunu getir
    /// </summary>
    public List<TeamStandingData> GetSortedStandings()
    {
        if (standings == null) return new List<TeamStandingData>();
        
        return standings
            .OrderByDescending(s => s.points)
            .ThenByDescending(s => s.GoalDifference)
            .ThenByDescending(s => s.goalsFor)
            .ToList();
    }
    
    /// <summary>
    /// Oyuncunun takımının lig pozisyonunu güncelle
    /// </summary>
    public void UpdatePlayerLeaguePosition(string playerTeamName)
    {
        var sorted = GetSortedStandings();
        for (int i = 0; i < sorted.Count; i++)
        {
            if (sorted[i].teamName == playerTeamName)
            {
                leaguePosition = i + 1;
                leaguePoints = sorted[i].points;
                wins = sorted[i].wins;
                draws = sorted[i].draws;
                losses = sorted[i].losses;
                break;
            }
        }
    }
    
    /// <summary>
    /// Ortalama rating'i hesapla
    /// </summary>
    public void CalculateAverageRating()
    {
        if (matchRatings == null || matchRatings.Count == 0)
        {
            averageRating = 0f;
            return;
        }
        
        float sum = 0f;
        foreach (var rating in matchRatings)
        {
            sum += rating;
        }
        
        averageRating = sum / matchRatings.Count;
    }
    
    /// <summary>
    /// Maç rating'i ekle
    /// </summary>
    public void AddMatchRating(float rating)
    {
        if (matchRatings == null)
        {
            matchRatings = new List<float>();
        }
        
        matchRatings.Add(rating);
        CalculateAverageRating();
    }
}
