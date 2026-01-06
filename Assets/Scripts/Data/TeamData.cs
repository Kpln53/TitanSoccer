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
    
    [Header("Stadyum")]
    public string stadiumName = "Stadyum";
    public int stadiumCapacity = 25000;
    
    [Header("Finans")]
    public long transferBudget = 5000000;    // Transfer bütçesi (€)
    public long wageBudget = 500000;         // Maaş bütçesi (haftalık €)
    
    [Header("Oyuncular")]
    public List<PlayerData> players = new List<PlayerData>();
    
    [Header("Hesaplanan Değerler")]
    [SerializeField] private int cachedTeamPower = 0; // Hesaplanan takım gücü (cache)
    [SerializeField] private long cachedSquadValue = 0; // Kadro piyasa değeri (cache)
    
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
    
    /// <summary>
    /// Kadro piyasa değerini hesapla
    /// </summary>
    public long CalculateSquadValue()
    {
        if (players == null || players.Count == 0)
        {
            cachedSquadValue = 0;
            return 0;
        }
        
        long totalValue = 0;
        foreach (var player in players)
        {
            if (player != null)
            {
                player.CalculateMarketValue();
                totalValue += player.marketValue;
            }
        }
        
        cachedSquadValue = totalValue;
        return totalValue;
    }
    
    /// <summary>
    /// Kadro değerini getir (cache kullan)
    /// </summary>
    public long GetSquadValue()
    {
        if (cachedSquadValue == 0 && players != null && players.Count > 0)
        {
            CalculateSquadValue();
        }
        return cachedSquadValue;
    }
    
    /// <summary>
    /// Formatlı kadro değeri
    /// </summary>
    public string GetFormattedSquadValue()
    {
        long value = GetSquadValue();
        
        if (value >= 1000000000)
        {
            return $"€{value / 1000000000f:F2}B";
        }
        else if (value >= 1000000)
        {
            return $"€{value / 1000000f:F1}M";
        }
        else if (value >= 1000)
        {
            return $"€{value / 1000f:F0}K";
        }
        return $"€{value}";
    }
    
    /// <summary>
    /// Formatlı transfer bütçesi
    /// </summary>
    public string GetFormattedTransferBudget()
    {
        if (transferBudget >= 1000000000)
        {
            return $"€{transferBudget / 1000000000f:F2}B";
        }
        else if (transferBudget >= 1000000)
        {
            return $"€{transferBudget / 1000000f:F1}M";
        }
        else if (transferBudget >= 1000)
        {
            return $"€{transferBudget / 1000f:F0}K";
        }
        return $"€{transferBudget}";
    }
    
    /// <summary>
    /// Ortalama oyuncu yaşını getir
    /// </summary>
    public float GetAverageAge()
    {
        if (players == null || players.Count == 0) return 0f;
        
        int totalAge = 0;
        int count = 0;
        
        foreach (var player in players)
        {
            if (player != null)
            {
                totalAge += player.age;
                count++;
            }
        }
        
        return count > 0 ? (float)totalAge / count : 0f;
    }
    
    /// <summary>
    /// Tüm oyuncuları yaşlandır (sezon sonu)
    /// </summary>
    public void AgeAllPlayers()
    {
        if (players == null) return;
        
        foreach (var player in players)
        {
            player?.AgePlayer();
        }
        
        // Takım gücünü yeniden hesapla
        CalculateTeamPower();
        CalculateSquadValue();
    }
}

