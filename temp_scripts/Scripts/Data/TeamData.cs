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
    public string teamShortName;
    public string teamCountry = "Unknown";
    
    [Header("Renkler")]
    public Color primaryColor = Color.blue;
    public Color secondaryColor = Color.white;
    
    [Header("Logo")]
    public Sprite teamLogo; // Takım logosu
    
    [Header("Oyuncular")]
    public List<PlayerData> players = new List<PlayerData>();
    
    [Header("Hesaplanan Değerler")]
    [SerializeField] private int cachedTeamPower = 0; // Hesaplanan takım gücü (cache)
    
    /// <summary>
    /// Takım gücünü hesapla (oyuncuların ortalama overall'ı)
    /// </summary>
    public void CalculateTeamPower()
    {
        if (players == null || players.Count == 0)
        {
            cachedTeamPower = 0;
            return;
        }
        
        int totalOverall = 0;
        int validPlayers = 0;
        
        foreach (var player in players)
        {
            if (player != null)
            {
                totalOverall += player.overall;
                validPlayers++;
            }
        }
        
        if (validPlayers > 0)
        {
            cachedTeamPower = Mathf.RoundToInt((float)totalOverall / validPlayers);
        }
        else
        {
            cachedTeamPower = 0;
        }
    }
    
    /// <summary>
    /// Takım gücünü getir (eğer hesaplanmamışsa hesapla)
    /// </summary>
    public int GetTeamPower()
    {
        if (cachedTeamPower == 0 && players != null && players.Count > 0)
        {
            CalculateTeamPower();
        }
        return cachedTeamPower;
    }
    
    /// <summary>
    /// Belirli bir pozisyondaki oyuncuları getir
    /// </summary>
    public List<PlayerData> GetPlayersByPosition(PlayerPosition position)
    {
        List<PlayerData> result = new List<PlayerData>();
        
        if (players == null)
            return result;
        
        foreach (var player in players)
        {
            if (player != null && player.position == position)
            {
                result.Add(player);
            }
        }
        
        return result;
    }
    
    /// <summary>
    /// En güçlü oyuncuyu getir
    /// </summary>
    public PlayerData GetBestPlayer()
    {
        if (players == null || players.Count == 0)
            return null;
        
        PlayerData best = null;
        int highestOverall = 0;
        
        foreach (var player in players)
        {
            if (player != null && player.overall > highestOverall)
            {
                highestOverall = player.overall;
                best = player;
            }
        }
        
        return best;
    }
    
    /// <summary>
    /// Belirli bir oyuncuyu isimle bul
    /// </summary>
    public PlayerData GetPlayerByName(string playerName)
    {
        if (players == null)
            return null;
        
        foreach (var player in players)
        {
            if (player != null && player.playerName == playerName)
            {
                return player;
            }
        }
        
        return null;
    }
}

