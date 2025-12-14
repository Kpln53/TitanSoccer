using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Takım veri yapısı - Data Pack sistemi için
/// </summary>
[System.Serializable]
public class TeamData
{
    [Header("Temel Bilgiler")]
    public string teamName;
    public string teamShortName; // Kısa isim (örn: "BAR" için Barcelona)
    public string teamCountry = "Unknown";
    
    [Header("Görsel")]
    public Color primaryColor = Color.blue;
    public Color secondaryColor = Color.white;
    
    [Header("Oyuncular")]
    public List<PlayerData> players = new List<PlayerData>();
    
    [Header("Takım Gücü (Otomatik Hesaplanır)")]
    [SerializeField] private int teamPower = 0;
    
    /// <summary>
    /// Takım gücünü hesapla (oyuncuların overall'larına göre)
    /// </summary>
    public int CalculateTeamPower()
    {
        if (players == null || players.Count == 0)
        {
            teamPower = 0;
            return 0;
        }
        
        int totalOverall = 0;
        foreach (var player in players)
        {
            totalOverall += player.overall;
        }
        
        teamPower = totalOverall / players.Count;
        return teamPower;
    }
    
    /// <summary>
    /// Takım gücünü al
    /// </summary>
    public int GetTeamPower()
    {
        if (teamPower == 0)
        {
            CalculateTeamPower();
        }
        return teamPower;
    }
    
    /// <summary>
    /// Belirli bir pozisyondaki oyuncuyu getir
    /// </summary>
    public PlayerData GetPlayerByPosition(PlayerPosition position)
    {
        foreach (var player in players)
        {
            if (player.position == position)
                return player;
        }
        return null;
    }
    
    /// <summary>
    /// Takımın ortalama overall'ını al
    /// </summary>
    public float GetAverageOverall()
    {
        if (players == null || players.Count == 0)
            return 0f;
        
        int total = 0;
        foreach (var player in players)
        {
            total += player.overall;
        }
        
        return (float)total / players.Count;
    }
}

