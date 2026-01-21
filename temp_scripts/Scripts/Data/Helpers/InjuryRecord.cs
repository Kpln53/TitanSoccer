using System;
using UnityEngine;

/// <summary>
/// Sakatlık kaydı - Sakatlık sistemi için
/// </summary>
[System.Serializable]
public class InjuryRecord
{
    [Header("Sakatlık Bilgileri")]
    public string injuryName;      // Sakatlık adı
    public string injuryDescription; // Açıklama
    public int severity;           // Şiddet (1-10, 10 en şiddetli)
    
    [Header("Süre")]
    public int durationDays;       // Sakatlık süresi (gün)
    public int daysRemaining;      // Kalan gün
    
    [Header("Tarih")]
    public DateTime injuryDate;    // Sakatlık tarihi
    public string injuryDateString; // Tarih (string)
    public DateTime recoveryDate;  // İyileşme tarihi (beklenen)
    public string recoveryDateString; // Tarih (string)
    
    [Header("Durum")]
    public bool isRecovered;       // İyileşti mi?
    public bool isRecovering;      // İyileşiyor mu? (rehabilitasyon)
    
    public InjuryRecord()
    {
        injuryName = "Unknown Injury";
        severity = 5;
        durationDays = 7;
        daysRemaining = 7;
        injuryDate = DateTime.Now;
        injuryDateString = injuryDate.ToString("yyyy-MM-dd");
        recoveryDate = injuryDate.AddDays(durationDays);
        recoveryDateString = recoveryDate.ToString("yyyy-MM-dd");
        isRecovered = false;
        isRecovering = false;
    }
    
    /// <summary>
    /// İlerleme yüzdesini getir
    /// </summary>
    public float GetProgressPercent()
    {
        if (durationDays <= 0) return 1f;
        int daysPassed = durationDays - daysRemaining;
        return Mathf.Clamp01((float)daysPassed / durationDays);
    }
}

