using System;
using System.Collections.Generic;
using UnityEngine;

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
    
    public SeasonData()
    {
        seasonNumber = 1;
        seasonName = "2025-2026";
        matchRatings = new List<float>();
        seasonStartDate = DateTime.Now;
        seasonStartDateString = seasonStartDate.ToString("yyyy-MM-dd");
        seasonEndDate = seasonStartDate.AddMonths(9); // ~9 ay sezon
        seasonEndDateString = seasonEndDate.ToString("yyyy-MM-dd");
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

