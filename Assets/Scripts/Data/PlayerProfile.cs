using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Oyuncu profili - Oyun içi oyuncu verileri
/// </summary>
[Serializable]
public class PlayerProfile
{
    [Header("Temel Bilgiler")]
    public string playerName;          // Oyuncu adı
    public PlayerPosition position;    // Pozisyon
    public int age;                    // Yaş
    public string nationality;         // Uyruk
    
    [Header("Yetenekler (0-100)")]
    public int passingSkill = 50;
    public int shootingSkill = 50;
    public int dribblingSkill = 50;
    public int falsoSkill = 50;        // Falso/Çalım yeteneği
    public int speed = 50;
    public int stamina = 50;
    public int defendingSkill = 50;
    public int physicalStrength = 50;
    
    [Header("Kaleci Özel Yetenekleri")]
    public int saveReflex = 0;
    public int goalkeeperPositioning = 0;
    public int aerialAbility = 0;
    public int oneOnOne = 0;
    public int handling = 0;
    
    [Header("Overall")]
    public int overall;                // Genel yetenek (0-100)
    
    [Header("Kariyer İstatistikleri")]
    public int careerGoals = 0;        // Kariyer toplam gol
    public int careerAssists = 0;      // Kariyer toplam asist
    public int careerMatches = 0;      // Kariyer toplam maç
    public float careerAverageRating = 0f; // Kariyer ortalama rating
    
    [Header("Form ve Durum")]
    public float form = 50f;           // Form (0-100)
    public float moral = 50f;          // Moral (0-100)
    public float energy = 100f;        // Enerji (0-100)
    
    [Header("Sakatlık")]
    public InjuryRecord currentInjury; // Mevcut sakatlık (null = sakatlık yok)
    public List<InjuryRecord> injuryHistory; // Sakatlık geçmişi
    
    [Header("Takım Bilgileri")]
    public string currentClubName;     // Mevcut kulüp adı
    public string currentLeagueName;   // Mevcut lig adı
    
    public PlayerProfile()
    {
        playerName = "Unknown Player";
        position = PlayerPosition.MOO;
        age = 25;
        nationality = "Unknown";
        overall = 50;
        currentInjury = null;
        injuryHistory = new List<InjuryRecord>();
    }
    
    /// <summary>
    /// Overall'ı yeteneklerden hesapla
    /// </summary>
    public void CalculateOverall()
    {
        if (position == PlayerPosition.KL)
        {
            // Kaleci için özel hesaplama
            float avg = (saveReflex + goalkeeperPositioning + aerialAbility + oneOnOne + handling) / 5f;
            overall = Mathf.RoundToInt(avg);
        }
        else
        {
            // Diğer pozisyonlar için normal hesaplama
            float avg = (passingSkill + shootingSkill + dribblingSkill + falsoSkill + speed + stamina + defendingSkill + physicalStrength) / 8f;
            overall = Mathf.RoundToInt(avg);
        }
    }
    
    /// <summary>
    /// Kaleci mi kontrol et
    /// </summary>
    public bool IsGoalkeeper()
    {
        return position == PlayerPosition.KL;
    }
    
    /// <summary>
    /// Sakatlık var mı kontrol et
    /// </summary>
    public bool IsInjured()
    {
        return currentInjury != null && !currentInjury.isRecovered;
    }
}

