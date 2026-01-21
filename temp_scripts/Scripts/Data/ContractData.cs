using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sözleşme verisi - Oyuncunun kulüp sözleşmesi
/// </summary>
[System.Serializable]
public class ContractData
{
    [Header("Sözleşme Temel Bilgileri")]
    public int salary;                     // Maaş (haftalık veya aylık)
    public int contractDuration;           // Sözleşme süresi (yıl)
    public DateTime startDate;             // Başlangıç tarihi
    public string startDateString;         // Tarih (string)
    public DateTime endDate;               // Bitiş tarihi
    public string endDateString;           // Tarih (string)
    
    [Header("Rol ve Oynama Süresi")]
    public ContractRole role;              // Sözleşmedeki rol (Starter, Rotation, Substitute)
    public PlayingTime playingTime;        // Oynama süresi garantisi
    
    [Header("Primler")]
    public List<ContractBonus> bonuses;    // Bonus listesi
    
    [Header("Sözleşme Maddeleri")]
    public List<ClauseType> clauses;       // Sözleşme maddeleri
    
    [Header("Ek Bilgiler")]
    public int signingBonus;               // İmza parası
    public string clubName;                // Kulüp adı
    
    public ContractData()
    {
        salary = 0;
        contractDuration = 1;
        startDate = DateTime.Now;
        startDateString = startDate.ToString("yyyy-MM-dd");
        endDate = startDate.AddYears(contractDuration);
        endDateString = endDate.ToString("yyyy-MM-dd");
        role = ContractRole.Rotation;
        playingTime = PlayingTime.Rotation;
        bonuses = new List<ContractBonus>();
        clauses = new List<ClauseType>();
        signingBonus = 0;
    }
    
    /// <summary>
    /// Belirli bir bonus türünün miktarını getir
    /// </summary>
    public int GetBonusAmount(BonusType bonusType)
    {
        foreach (var bonus in bonuses)
        {
            if (bonus.type == bonusType)
            {
                return bonus.amount;
            }
        }
        return 0;
    }
    
    /// <summary>
    /// Belirli bir bonus türü var mı kontrol et
    /// </summary>
    public bool HasBonus(BonusType bonusType)
    {
        foreach (var bonus in bonuses)
        {
            if (bonus.type == bonusType)
            {
                return true;
            }
        }
        return false;
    }
    
    /// <summary>
    /// Belirli bir madde var mı kontrol et
    /// </summary>
    public bool HasClause(ClauseType clauseType)
    {
        return clauses.Contains(clauseType);
    }
    
    /// <summary>
    /// Sözleşme süresi doldu mu kontrol et
    /// </summary>
    public bool IsExpired()
    {
        return DateTime.Now > endDate;
    }
    
    /// <summary>
    /// Kalan gün sayısını getir
    /// </summary>
    public int GetDaysRemaining()
    {
        if (IsExpired())
            return 0;
        
        return (endDate - DateTime.Now).Days;
    }
}

/// <summary>
/// Sözleşme bonusu
/// </summary>
[System.Serializable]
public class ContractBonus
{
    public BonusType type;     // Bonus türü
    public int amount;         // Bonus miktarı
    
    public ContractBonus()
    {
        type = BonusType.MatchFee;
        amount = 0;
    }
    
    public ContractBonus(BonusType bonusType, int bonusAmount)
    {
        type = bonusType;
        amount = bonusAmount;
    }
}

