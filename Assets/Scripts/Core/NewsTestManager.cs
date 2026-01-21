using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Haber sistemi test yöneticisi - Basit test
/// </summary>
public class NewsTestManager : MonoBehaviour
{
    public List<NewsItem> testNews = new List<NewsItem>();
    
    private void Start()
    {
        Debug.Log("🚀 NewsTestManager başlatıldı!");
        
        // Temel sistemleri başlat
        InitializeSystems();
        
        // Test haberleri oluştur
        CreateTestNews();
        
        // Sonuçları göster
        ShowResults();
    }
    
    private void InitializeSystems()
    {
        Debug.Log("🔧 Sistemler başlatılıyor...");
        
        // GameManager kontrolü
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("⚠️ GameManager bulunamadı - Yeni GameObject oluşturuluyor");
            
            GameObject gmObj = new GameObject("GameManager");
            gmObj.AddComponent<GameManager>();
        }
        
        // NewsSystem kontrolü
        if (NewsSystem.Instance == null)
        {
            Debug.LogWarning("⚠️ NewsSystem bulunamadı - Yeni GameObject oluşturuluyor");
            
            GameObject nsObj = new GameObject("NewsSystem");
            nsObj.AddComponent<NewsSystem>();
        }
        
        // NewsGenerator kontrolü
        if (NewsGenerator.Instance == null)
        {
            Debug.LogWarning("⚠️ NewsGenerator bulunamadı - Yeni GameObject oluşturuluyor");
            
            GameObject ngObj = new GameObject("NewsGenerator");
            ngObj.AddComponent<NewsGenerator>();
        }
        
        Debug.Log("✅ Sistemler hazır!");
    }
    
    private void CreateTestNews()
    {
        Debug.Log("📰 Test haberleri oluşturuluyor...");
        
        // Minimal test kayıtı oluştur
        var testSave = new SaveData();
        testSave.playerProfile = new PlayerProfile 
        { 
            playerName = "Test Oyuncu",
            currentClubName = "Test FC",
            position = PlayerPosition.SF
        };
        testSave.clubData = new ClubData { clubName = "Test FC" };
        testSave.seasonData = new SeasonData 
        { 
            matchesPlayed = 10,
            goals = 5,
            assists = 3,
            leaguePosition = 3,
            leaguePoints = 25
        };
        testSave.mediaData = new MediaData();
        
        // GameManager'a set et
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetCurrentSave(testSave, 0);
            Debug.Log("✅ Test kayıtı GameManager'a set edildi");
        }
        
        // Manuel test haberleri
        var manualNews = new[]
        {
            new { title = "🏆 Test Ligi Başladı!", content = "Test ligi muhteşem bir açılışla başladı.", type = NewsType.League },
            new { title = "⚽ İlk Gol Atıldı!", content = "Test oyuncusu ilk golünü attı.", type = NewsType.Match },
            new { title = "💰 Transfer Haberi", content = "Yeni transfer gerçekleşti.", type = NewsType.Transfer }
        };
        
        foreach (var news in manualNews)
        {
            var newsItem = new NewsItem
            {
                title = news.title,
                content = news.content,
                type = news.type,
                source = "Test Sistemi",
                date = DateTime.Now.AddHours(-UnityEngine.Random.Range(1, 24)),
                isRead = false
            };
            newsItem.dateString = newsItem.date.ToString("dd.MM.yyyy HH:mm");
            
            testNews.Add(newsItem);
            testSave.mediaData.AddNews(newsItem);
        }
        
        Debug.Log($"✅ {manualNews.Length} manuel haber oluşturuldu");
        
        // NewsGenerator ile otomatik haber
        if (NewsGenerator.Instance != null)
        {
            NewsGenerator.Instance.GenerateTransferNews("Test Oyuncu", "Eski Takım", "Test FC", 15.5f, 3);
            NewsGenerator.Instance.GenerateInjuryNews("Test Oyuncu", "Kas zorlanması", 2);
            
            Debug.Log("✅ NewsGenerator ile 2 otomatik haber oluşturuldu");
        }
    }
    
    private void ShowResults()
    {
        Debug.Log("📊 Test sonuçları:");
        
        if (GameManager.Instance != null && GameManager.Instance.HasCurrentSave())
        {
            var mediaData = GameManager.Instance.CurrentSave.mediaData;
            if (mediaData != null && mediaData.recentNews != null)
            {
                Debug.Log($"📰 Toplam haber sayısı: {mediaData.recentNews.Count}");
                
                foreach (var news in mediaData.recentNews)
                {
                    Debug.Log($"   • {news.title} ({news.type}) - {news.dateString}");
                }
            }
            else
            {
                Debug.LogWarning("❌ MediaData veya recentNews null!");
            }
        }
        else
        {
            Debug.LogWarning("❌ GameManager veya CurrentSave null!");
        }
        
        Debug.Log("🎯 Test tamamlandı!");
    }
    
    /// <summary>
    /// Manuel test metodu - Editor'dan çağrılabilir
    /// </summary>
    [ContextMenu("Test Haberleri Oluştur")]
    public void ManualCreateNews()
    {
        CreateTestNews();
        ShowResults();
    }
    
    /// <summary>
    /// Haberleri temizle
    /// </summary>
    [ContextMenu("Haberleri Temizle")]
    public void ClearNews()
    {
        testNews.Clear();
        
        if (GameManager.Instance != null && GameManager.Instance.HasCurrentSave())
        {
            var mediaData = GameManager.Instance.CurrentSave.mediaData;
            if (mediaData != null && mediaData.recentNews != null)
            {
                mediaData.recentNews.Clear();
                Debug.Log("🗑️ Tüm haberler temizlendi");
            }
        }
    }
}