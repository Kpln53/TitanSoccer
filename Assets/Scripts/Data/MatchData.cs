using System;
using UnityEngine;

/// <summary>
/// Maç veri yapısı - Fikstür ve maç sonuçları için
/// </summary>
[System.Serializable]
public class MatchData
{
    [Header("Maç Bilgileri")]
    public string homeTeamName;        // Ev sahibi takım adı
    public string awayTeamName;        // Deplasman takım adı
    public DateTime matchDate;         // Maç tarihi
    public string matchDateString;      // Tarih (string format)
    
    [Header("Maç Sonucu")]
    public bool isPlayed = false;      // Maç oynandı mı?
    public int homeScore = 0;          // Ev sahibi skor
    public int awayScore = 0;          // Deplasman skor
    
    [Header("Oyuncu İstatistikleri")]
    public float playerRating = 0f;    // Oyuncunun maç rating'i (0-10)
    public int playerGoals = 0;        // Oyuncunun attığı gol sayısı
    public int playerAssists = 0;      // Oyuncunun yaptığı asist sayısı
    public int playerShots = 0;        // Oyuncunun şut sayısı
    
    [Header("Maç Tipi")]
    public MatchType matchType = MatchType.League; // Maç tipi
    
    public enum MatchType
    {
        League,      // Lig maçı
        Cup,         // Kupa maçı
        Friendly,    // Hazırlık maçı
        Tournament   // Turnuva maçı
    }
    
    public MatchData()
    {
        homeTeamName = "";
        awayTeamName = "";
        matchDate = DateTime.Now;
        matchDateString = matchDate.ToString("yyyy-MM-dd");
        isPlayed = false;
        homeScore = 0;
        awayScore = 0;
        playerRating = 0f;
        playerGoals = 0;
        playerAssists = 0;
        playerShots = 0;
        matchType = MatchType.League;
    }
    
    public MatchData(string homeTeam, string awayTeam, DateTime date, MatchType type = MatchType.League)
    {
        homeTeamName = homeTeam;
        awayTeamName = awayTeam;
        matchDate = date;
        matchDateString = matchDate.ToString("yyyy-MM-dd");
        isPlayed = false;
        homeScore = 0;
        awayScore = 0;
        playerRating = 0f;
        playerGoals = 0;
        playerAssists = 0;
        playerShots = 0;
        matchType = type;
    }
    
    /// <summary>
    /// Maç sonucunu ayarla
    /// </summary>
    public void SetResult(int home, int away)
    {
        homeScore = home;
        awayScore = away;
        isPlayed = true;
    }
    
    /// <summary>
    /// Oyuncu istatistiklerini ayarla
    /// </summary>
    public void SetPlayerStats(float rating, int goals, int assists, int shots)
    {
        playerRating = rating;
        playerGoals = goals;
        playerAssists = assists;
        playerShots = shots;
    }
    
    /// <summary>
    /// Maç kazananını getir (null = beraberlik)
    /// </summary>
    public string GetWinner()
    {
        if (!isPlayed) return null;
        if (homeScore > awayScore) return homeTeamName;
        if (awayScore > homeScore) return awayTeamName;
        return null; // Beraberlik
    }
    
    /// <summary>
    /// Maç berabere mi?
    /// </summary>
    public bool IsDraw()
    {
        return isPlayed && homeScore == awayScore;
    }
}

