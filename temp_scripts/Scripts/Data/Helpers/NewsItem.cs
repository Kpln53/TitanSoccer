using System;
using UnityEngine;

/// <summary>
/// Haber item'ı - Haber sistemi için
/// </summary>
[System.Serializable]
public class NewsItem
{
    [Header("Temel Bilgiler")]
    public string title;           // Haber başlığı
    public string content;         // Haber içeriği
    public NewsType type;          // Haber türü
    
    [Header("Tarih")]
    public DateTime date;          // Tarih (DateTime)
    public string dateString;      // Tarih (string - JSON uyumluluğu için)
    
    [Header("Ek Bilgiler")]
    public string source;          // Haber kaynağı (opsiyonel)
    public bool isRead;            // Okundu mu?
    
    public NewsItem()
    {
        date = DateTime.Now;
        dateString = date.ToString("yyyy-MM-dd HH:mm:ss");
        type = NewsType.League;
        isRead = false;
    }
}

