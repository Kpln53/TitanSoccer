using UnityEngine;
using UnityEditor;
using System;

/// <summary>
/// Haber sistemi test araçları - Editor menüsünden çalıştır
/// </summary>
public class NewsTestTools
{
    [MenuItem("TitanSoccer/News/🧪 Create Test News")]
    public static void CreateTestNews()
    {
        Debug.Log("🧪 Test haberleri oluşturuluyor...");
        
        // Eğer kayıt yoksa minimal bir test kayıtı oluştur
        if (GameManager.Instance == null || !GameManager.Instance.HasCurrentSave())
        {
            Debug.Log("🔧 Kayıt bulunamadı, test kayıtı oluşturuluyor...");
            
            if (GameManager.Instance == null)
            {
                EditorUtility.DisplayDialog("Hata", "GameManager bulunamadı!\nÖnce oyunu çalıştır.", "Tamam");
                return;
            }
            
            // Minimal test kayıtı oluştur
            CreateMinimalTestSave();
        }
        
        var mediaData = GameManager.Instance.CurrentSave.mediaData;
        if (mediaData == null)
        {
            GameManager.Instance.CurrentSave.mediaData = new MediaData();
            mediaData = GameManager.Instance.CurrentSave.mediaData;
        }
        
        // Test haberleri
        var testNews = new[]
        {
            new { title = "🏆 Lig Liderliği Devam Ediyor!", content = "Takımımız bu hafta aldığı galibiyetle lig liderliğini sürdürüyor. Oyuncular mükemmel bir performans sergiledi.", type = NewsType.League, source = "Spor Gazetesi" },
            new { title = "⚽ Muhteşem Hat-trick Performansı", content = "Genç yıldızımız dün akşamki maçta hat-trick yaparak takımını zafere taşıdı. Bu performans tüm futbol dünyasında konuşuluyor.", type = NewsType.Match, source = "Futbol Haberleri" },
            new { title = "💰 Yeni Transfer Bombası!", content = "Kulübümüz yaz transfer döneminde büyük bir hamle yaparak ünlü oyuncuyu kadrosuna kattı. Transfer bedeli 15 milyon euro olarak açıklandı.", type = NewsType.Transfer, source = "Transfer Merkezi" },
            new { title = "🏥 Sakatlık Endişesi", content = "Takımın yıldız oyuncusu antrenman sırasında yaralandı. Doktorlar 2-3 haftalık dinlenme önerdi.", type = NewsType.Injury, source = "Sağlık Raporu" },
            new { title = "📊 Sezon İstatistikleri", content = "Bu sezon takımımız 25 maçta 18 galibiyet, 4 beraberlik ve 3 mağlubiyet aldı. Gol ortalaması 2.1.", type = NewsType.Performance, source = "İstatistik Merkezi" },
            new { title = "🗣️ Teknik Direktör Açıklaması", content = "Teknik direktörümüz basın toplantısında: 'Hedefimiz şampiyonluk. Oyuncularımın performansından çok memnunum.'", type = NewsType.TeamManagement, source = "Basın Toplantısı" },
            new { title = "💼 Sözleşme Yenilendi", content = "Takımın kaptanı ile 3 yıllık yeni sözleşme imzalandı. Oyuncu: 'Bu kulüpte kalmaktan mutluyum.'", type = NewsType.Contract, source = "Kulüp Resmi" },
            new { title = "🏅 Ayın Oyuncusu Seçildi", content = "Geçen ay gösterdiği performansla ayın oyuncusu seçilen yıldızımız ödülünü aldı.", type = NewsType.Achievement, source = "Lig Organizasyonu" },
            new { title = "👂 Transfer Söylentileri", content = "Avrupa'dan gelen haberlere göre yıldız oyuncumuz için 20 milyon euroluk teklif geldi. Kulüp henüz açıklama yapmadı.", type = NewsType.Rumour, source = "Transfer Söylentileri" }
        };
        
        int addedCount = 0;
        foreach (var news in testNews)
        {
            var newsItem = new NewsItem
            {
                title = news.title,
                content = news.content,
                type = news.type,
                source = news.source,
                date = DateTime.Now.AddHours(-UnityEngine.Random.Range(1, 48)), // Son 48 saat içinde
                isRead = false
            };
            newsItem.dateString = newsItem.date.ToString("dd.MM.yyyy HH:mm");
            
            mediaData.AddNews(newsItem);
            addedCount++;
        }
        
        Debug.Log($"✅ {addedCount} test haberi oluşturuldu!");
        
        EditorUtility.DisplayDialog("Test Haberleri", 
            $"✅ {addedCount} test haberi oluşturuldu!\n\n" +
            "Haberler panelini açarak kontrol edebilirsin.", "Tamam");
    }
    
    /// <summary>
    /// Minimal test kayıtı oluştur
    /// </summary>
    private static void CreateMinimalTestSave()
    {
        var testSave = new SaveData();
        
        // Temel oyuncu profili
        testSave.playerProfile = new PlayerProfile
        {
            playerName = "Test Oyuncu",
            currentClubName = "Test FC",
            position = PlayerPosition.SF
        };
        
        // Temel kulüp verisi
        testSave.clubData = new ClubData
        {
            clubName = "Test FC"
        };
        
        // Temel sezon verisi
        testSave.seasonData = new SeasonData
        {
            seasonNumber = 1,
            seasonName = "2025-2026",
            matchesPlayed = 10,
            goals = 5,
            assists = 3,
            leaguePosition = 3,
            leaguePoints = 25
        };
        
        // Media data
        testSave.mediaData = new MediaData();
        
        // GameManager'a set et
        GameManager.Instance.SetCurrentSave(testSave, 0);
        
        Debug.Log("🔧 Minimal test kayıtı oluşturuldu!");
    }
    
    [MenuItem("TitanSoccer/News/📊 Show News Status")]
    public static void ShowNewsStatus()
    {
        if (GameManager.Instance == null || !GameManager.Instance.HasCurrentSave())
        {
            EditorUtility.DisplayDialog("Hata", "Oyun kayıtlı değil!", "Tamam");
            return;
        }
        
        var mediaData = GameManager.Instance.CurrentSave.mediaData;
        if (mediaData == null || mediaData.recentNews == null)
        {
            EditorUtility.DisplayDialog("Haber Durumu", "Henüz haber yok.", "Tamam");
            return;
        }
        
        int totalNews = mediaData.recentNews.Count;
        int unreadNews = 0;
        
        foreach (var news in mediaData.recentNews)
        {
            if (!news.isRead) unreadNews++;
        }
        
        string status = $"📰 Toplam Haber: {totalNews}\n" +
                       $"📬 Okunmamış: {unreadNews}\n" +
                       $"📖 Okunmuş: {totalNews - unreadNews}";
        
        Debug.Log($"📊 Haber Durumu:\n{status}");
        
        EditorUtility.DisplayDialog("Haber Durumu", status, "Tamam");
    }
    
    [MenuItem("TitanSoccer/News/🗑️ Clear All News")]
    public static void ClearAllNews()
    {
        if (GameManager.Instance == null || !GameManager.Instance.HasCurrentSave())
        {
            EditorUtility.DisplayDialog("Hata", "Oyun kayıtlı değil!", "Tamam");
            return;
        }
        
        bool confirm = EditorUtility.DisplayDialog("Haberleri Temizle", 
            "Tüm haberleri silmek istediğinden emin misin?", "Evet", "Hayır");
            
        if (!confirm) return;
        
        var mediaData = GameManager.Instance.CurrentSave.mediaData;
        if (mediaData != null && mediaData.recentNews != null)
        {
            int count = mediaData.recentNews.Count;
            mediaData.recentNews.Clear();
            
            Debug.Log($"🗑️ {count} haber silindi!");
            EditorUtility.DisplayDialog("Temizlendi", $"{count} haber silindi!", "Tamam");
        }
    }
    
    [MenuItem("TitanSoccer/News/⚡ Quick Add News")]
    public static void QuickAddNews()
    {
        string title = EditorUtility.DisplayDialogComplex("Hızlı Haber Ekle", 
            "Hangi türde haber eklemek istiyorsun?", 
            "Maç Haberi", "Transfer Haberi", "İptal") switch
        {
            0 => "⚽ Muhteşem Galibiyet!",
            1 => "💰 Yeni Transfer!",
            _ => null
        };
        
        if (title == null) return;
        
        if (GameManager.Instance == null || !GameManager.Instance.HasCurrentSave())
        {
            EditorUtility.DisplayDialog("Hata", "Oyun kayıtlı değil!", "Tamam");
            return;
        }
        
        var mediaData = GameManager.Instance.CurrentSave.mediaData;
        if (mediaData == null)
        {
            GameManager.Instance.CurrentSave.mediaData = new MediaData();
            mediaData = GameManager.Instance.CurrentSave.mediaData;
        }
        
        var newsItem = new NewsItem
        {
            title = title,
            content = title.Contains("Galibiyet") ? 
                "Takımımız dün akşamki maçta muhteşem bir performans sergileyerek rakibini 3-1 mağlup etti." :
                "Kulübümüz yeni sezon için önemli bir transfer gerçekleştirdi.",
            type = title.Contains("Galibiyet") ? NewsType.Match : NewsType.Transfer,
            source = "Test Haberi",
            date = DateTime.Now,
            isRead = false
        };
        newsItem.dateString = newsItem.date.ToString("dd.MM.yyyy HH:mm");
        
        mediaData.AddNews(newsItem);
        
        Debug.Log($"⚡ Hızlı haber eklendi: {title}");
        EditorUtility.DisplayDialog("Haber Eklendi", $"✅ {title}", "Tamam");
    }
}