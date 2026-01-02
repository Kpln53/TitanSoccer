using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Haber sistemi - Haber üretimi ve yönetimi (Singleton)
/// </summary>
public class NewsSystem : MonoBehaviour
{
    public static NewsSystem Instance { get; private set; }

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log("[NewsSystem] NewsSystem initialized.");
    }

    /// <summary>
    /// Son haberleri getir
    /// </summary>
    public List<NewsItem> GetRecentNews(MediaData mediaData, int count = 20)
    {
        if (mediaData == null || mediaData.recentNews == null)
        {
            return new List<NewsItem>();
        }

        // En yeni haberlerden belirli sayıda getir
        var sortedNews = mediaData.recentNews.OrderByDescending(n => n.date).Take(count).ToList();
        return sortedNews;
    }

    /// <summary>
    /// Haber ekle
    /// </summary>
    public void AddNews(MediaData mediaData, NewsItem news)
    {
        if (mediaData == null)
        {
            Debug.LogWarning("[NewsSystem] MediaData is null! Cannot add news.");
            return;
        }

        if (news == null)
        {
            Debug.LogWarning("[NewsSystem] NewsItem is null! Cannot add news.");
            return;
        }

        mediaData.AddNews(news);
        Debug.Log($"[NewsSystem] News added: {news.title}");
    }

    /// <summary>
    /// Haberi okundu olarak işaretle
    /// </summary>
    public void MarkNewsAsRead(MediaData mediaData, NewsItem news)
    {
        if (mediaData == null || mediaData.recentNews == null || news == null)
            return;

        news.isRead = true;
        Debug.Log($"[NewsSystem] News marked as read: {news.title}");
    }

    /// <summary>
    /// Okunmamış haber sayısını getir
    /// </summary>
    public int GetUnreadNewsCount(MediaData mediaData)
    {
        if (mediaData == null || mediaData.recentNews == null)
            return 0;

        return mediaData.recentNews.Count(n => !n.isRead);
    }

    /// <summary>
    /// Belirli türde haberler getir
    /// </summary>
    public List<NewsItem> GetNewsByType(MediaData mediaData, NewsType type)
    {
        if (mediaData == null || mediaData.recentNews == null)
            return new List<NewsItem>();

        return mediaData.recentNews.Where(n => n.type == type).OrderByDescending(n => n.date).ToList();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}





