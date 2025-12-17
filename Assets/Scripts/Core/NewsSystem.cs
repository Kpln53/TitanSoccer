using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Basın ve haber sistemi
/// </summary>
public class NewsSystem : MonoBehaviour
{
    public static NewsSystem Instance;

    [Header("Haber Ayarları")]
    [SerializeField] private int maxRecentNews = 20; // Maksimum son haber sayısı

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Maç sonrası haber oluştur
    /// </summary>
    public void CreateMatchNews(PlayerProfile profile, float matchRating, string opponentTeam, bool isWin)
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentSave == null)
            return;
        
        NewsItem news = new NewsItem();
        news.date = System.DateTime.Now.ToString("yyyy-MM-dd");
        
        if (matchRating >= 8.0f)
        {
            news.title = $"{profile.playerName} Mükemmel Performans!";
            news.content = $"{profile.playerName}, {opponentTeam} karşısında harika bir performans sergiledi. Rating: {matchRating:F1}";
            news.type = NewsType.Achievement;
        }
        else if (matchRating >= 7.0f)
        {
            news.title = $"{profile.playerName} İyi Oynadı";
            news.content = $"{profile.playerName}, {opponentTeam} maçında iyi bir performans gösterdi.";
            news.type = NewsType.MatchPerformance;
        }
        else if (matchRating < 5.0f)
        {
            news.title = $"{profile.playerName} Kötü Performans";
            news.content = $"{profile.playerName}, {opponentTeam} maçında beklenen performansı gösteremedi.";
            news.type = NewsType.MatchPerformance;
        }
        else
        {
            news.title = $"{profile.playerName} Normal Performans";
            news.content = $"{profile.playerName}, {opponentTeam} maçında normal bir performans sergiledi.";
            news.type = NewsType.MatchPerformance;
        }
        
        AddNews(news);
    }

    /// <summary>
    /// Transfer haberi oluştur
    /// </summary>
    public void CreateTransferNews(string playerName, string teamName, bool isAccepted)
    {
        NewsItem news = new NewsItem();
        news.date = System.DateTime.Now.ToString("yyyy-MM-dd");
        
        if (isAccepted)
        {
            news.title = $"{playerName} {teamName}'a Transfer Oldu!";
            news.content = $"{playerName}, {teamName} ile sözleşme imzaladı.";
            news.type = NewsType.TransferRumor;
        }
        else
        {
            news.title = $"{playerName} Transfer Söylentileri";
            news.content = $"{playerName} için {teamName}'dan transfer teklifi geldiği söyleniyor.";
            news.type = NewsType.TransferRumor;
        }
        
        AddNews(news);
    }

    /// <summary>
    /// Disiplin haberi oluştur
    /// </summary>
    public void CreateDisciplineNews(PlayerProfile profile, string reason)
    {
        NewsItem news = new NewsItem();
        news.date = System.DateTime.Now.ToString("yyyy-MM-dd");
        news.title = $"{profile.playerName} Disiplin Sorunu";
        news.content = $"{profile.playerName} hakkında disiplin sorunu: {reason}";
        news.type = NewsType.Discipline;
        
        AddNews(news);
    }

    /// <summary>
    /// Haber ekle
    /// </summary>
    private void AddNews(NewsItem news)
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentSave == null)
            return;
        
        var mediaData = GameManager.Instance.CurrentSave.mediaData;
        mediaData.recentNews.Insert(0, news); // En yeni haber başa eklenir
        
        // Maksimum haber sayısını kontrol et
        if (mediaData.recentNews.Count > maxRecentNews)
        {
            mediaData.recentNews.RemoveAt(mediaData.recentNews.Count - 1);
        }
    }

    /// <summary>
    /// Okunmamış haber sayısını al
    /// </summary>
    public int GetUnreadNewsCount()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentSave == null)
            return 0;
        
        int count = 0;
        foreach (var news in GameManager.Instance.CurrentSave.mediaData.recentNews)
        {
            if (!news.isRead)
                count++;
        }
        
        return count;
    }

    /// <summary>
    /// Haberi okundu olarak işaretle
    /// </summary>
    public void MarkNewsAsRead(int index)
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentSave == null)
            return;
        
        var mediaData = GameManager.Instance.CurrentSave.mediaData;
        if (index >= 0 && index < mediaData.recentNews.Count)
        {
            mediaData.recentNews[index].isRead = true;
        }
    }
}


