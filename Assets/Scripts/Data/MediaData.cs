using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Medya verisi - Sadece haberler
/// </summary>
[System.Serializable]
public class MediaData
{
    [Header("Haberler")]
    public List<NewsItem> recentNews;      // Son haberler
    
    [Header("Ayarlar")]
    public int maxNewsCount = 50;          // Maksimum saklanacak haber sayısı
    
    public MediaData()
    {
        recentNews = new List<NewsItem>();
    }
    
    /// <summary>
    /// Haber ekle (maksimum sayıyı kontrol et)
    /// </summary>
    public void AddNews(NewsItem news)
    {
        if (recentNews == null)
        {
            recentNews = new List<NewsItem>();
        }
        
        recentNews.Add(news);
        
        // Eski haberleri sil (maksimum sayıyı aşmamak için)
        if (recentNews.Count > maxNewsCount)
        {
            recentNews.RemoveAt(0);
        }
    }
}