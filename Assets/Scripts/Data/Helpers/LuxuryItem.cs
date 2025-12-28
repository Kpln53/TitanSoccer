using System;
using UnityEngine;

/// <summary>
/// Lüks eşya - Ekonomi sistemi için
/// </summary>
[System.Serializable]
public class LuxuryItem
{
    [Header("Eşya Bilgileri")]
    public LuxuryType luxuryType;  // Lüks eşya türü
    public string itemName;        // Eşya adı
    public string description;     // Açıklama
    
    [Header("Değer")]
    public int value;              // Değeri (para)
    public int purchasePrice;      // Satın alma fiyatı
    
    [Header("Satın Alma")]
    public DateTime purchaseDate;  // Satın alma tarihi
    public string purchaseDateString; // Tarih (string)
    
    [Header("Özellikler")]
    public int happinessBonus;     // Mutluluk bonusu (0-100)
    
    public LuxuryItem()
    {
        luxuryType = LuxuryType.Car;
        purchaseDate = DateTime.Now;
        purchaseDateString = purchaseDate.ToString("yyyy-MM-dd");
        value = 0;
        purchasePrice = 0;
        happinessBonus = 0;
    }
}

