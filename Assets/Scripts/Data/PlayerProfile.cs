using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Oyuncu profil verileri - Tüm statlar ve gizli değerler
/// </summary>
[System.Serializable]
public class PlayerProfile
{
    [Header("Temel Bilgiler")]
    public string playerName;
    public string surname;
    public string nationality;
    public int age;
    public PlayerPosition position;
    
    [Header("Statlar (0-100)")]
    public int overall;
    public int pace;
    public int shooting;
    public int passing;
    public int dribbling;
    public int defense;
    public int stamina;
    
    [Header("Gizli Değerler")]
    [Range(0, 100)] public int injuryProne = 50; // Sakatlık eğilimi (0 = hiç, 100 = çok)
    [Range(0, 100)] public int morality = 75; // Ahlak/İtibar (0 = kötü, 100 = mükemmel)
    [Range(0, 100)] public int discipline = 75; // Disiplin (0 = kötü, 100 = mükemmel)
    
    [Header("Form & Durum")]
    [Range(0f, 1f)] public float form = 0.7f; // Form (0-1)
    [Range(0f, 1f)] public float morale = 0.7f; // Moral (0-1)
    [Range(0f, 1f)] public float energy = 1f; // Enerji (0-1)
    
    [Header("İstatistikler")]
    public SeasonStats seasonStats = new SeasonStats();
    public CareerStats careerStats = new CareerStats();
    
    [Header("Disiplin")]
    public int yellowCards = 0;
    public int redCards = 0;
    public int totalSuspensionDays = 0;
    
    [Header("Sakatlık Geçmişi")]
    public List<InjuryRecord> injuryHistory = new List<InjuryRecord>();
    
    [Header("Piyasa Değeri")]
    public int marketValue = 100000; // Euro cinsinden
    
    [Header("Karakter Görünümü")]
    public CharacterAppearance characterAppearance = new CharacterAppearance();
    
    /// <summary>
    /// Overall'ı statlardan hesapla
    /// </summary>
    public void CalculateOverall()
    {
        // Pozisyona göre ağırlıklı ortalama
        float weightedAvg = 0f;
        
        switch (position)
        {
            case PlayerPosition.KL: // Kaleci
                weightedAvg = (defense * 0.3f + stamina * 0.2f + passing * 0.15f + 
                              shooting * 0.1f + dribbling * 0.1f + pace * 0.1f + defense * 0.05f);
                break;
            case PlayerPosition.STP:
            case PlayerPosition.SĞB:
            case PlayerPosition.SLB:
            case PlayerPosition.MDO:
                weightedAvg = (defense * 0.3f + stamina * 0.2f + passing * 0.2f + 
                              physicalStrength() * 0.15f + pace * 0.1f + dribbling * 0.05f);
                break;
            case PlayerPosition.MOO:
                weightedAvg = (passing * 0.25f + dribbling * 0.2f + stamina * 0.2f + 
                              shooting * 0.15f + pace * 0.1f + defense * 0.1f);
                break;
            case PlayerPosition.SĞK:
            case PlayerPosition.SLK:
            case PlayerPosition.SĞO:
            case PlayerPosition.SLO:
                weightedAvg = (pace * 0.25f + dribbling * 0.2f + passing * 0.2f + 
                              shooting * 0.15f + stamina * 0.1f + defense * 0.1f);
                break;
            case PlayerPosition.SF: // Santrafor
                weightedAvg = (shooting * 0.3f + physicalStrength() * 0.2f + dribbling * 0.2f + 
                              pace * 0.15f + passing * 0.1f + stamina * 0.05f);
                break;
            default:
                weightedAvg = (pace + shooting + passing + dribbling + defense + stamina) / 6f;
                break;
        }
        
        overall = Mathf.RoundToInt(weightedAvg);
        overall = Mathf.Clamp(overall, 0, 100);
    }
    
    private int physicalStrength()
    {
        // Physical strength için stamina kullan
        return stamina;
    }
    
    /// <summary>
    /// Form trendini al (son 5 maç)
    /// </summary>
    public List<float> GetFormTrend()
    {
        return seasonStats.GetLastRatings(5);
    }
}

/// <summary>
/// Sezon istatistikleri
/// </summary>
[System.Serializable]
public class SeasonStats
{
    public int matchesPlayed = 0;
    public int minutesPlayed = 0;
    public int goals = 0;
    public int assists = 0;
    public int shots = 0;
    public int shotsOnTarget = 0;
    public int passes = 0;
    public int passesCompleted = 0;
    public float averageRating = 0f;
    
    private List<float> lastRatings = new List<float>(); // Son maç rating'leri
    
    public void AddMatchRating(float rating)
    {
        lastRatings.Add(rating);
        if (lastRatings.Count > 10) // Son 10 maçı tut
        {
            lastRatings.RemoveAt(0);
        }
        
        // Ortalama rating'i güncelle
        float total = 0f;
        foreach (var r in lastRatings)
        {
            total += r;
        }
        averageRating = lastRatings.Count > 0 ? total / lastRatings.Count : 0f;
    }
    
    public List<float> GetLastRatings(int count)
    {
        int start = Mathf.Max(0, lastRatings.Count - count);
        return lastRatings.GetRange(start, Mathf.Min(count, lastRatings.Count - start));
    }
    
    public float GetPassSuccessRate()
    {
        if (passes == 0) return 0f;
        return (float)passesCompleted / passes * 100f;
    }
}

/// <summary>
/// Kariyer istatistikleri
/// </summary>
[System.Serializable]
public class CareerStats
{
    public int totalMatches = 0;
    public int totalMinutes = 0;
    public int totalGoals = 0;
    public int totalAssists = 0;
    public int totalShots = 0;
    public int totalPasses = 0;
    public float careerAverageRating = 0f;
}

/// <summary>
/// Sakatlık kaydı
/// </summary>
[System.Serializable]
public class InjuryRecord
{
    public string injuryType; // Hafif, Orta, Ağır
    public int durationDays;
    public string date; // Tarih string olarak
    public bool isRecovered = true;
}

