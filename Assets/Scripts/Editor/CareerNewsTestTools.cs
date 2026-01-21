using UnityEngine;
using UnityEditor;
using System;

/// <summary>
/// Career Hub haber sistemi test araçları
/// </summary>
public class CareerNewsTestTools : EditorWindow
{
    [MenuItem("Tools/Career News/Test Tools")]
    public static void ShowWindow()
    {
        GetWindow<CareerNewsTestTools>("Career News Test");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Career Hub Haber Sistemi Test Araçları", EditorStyles.boldLabel);
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("🧪 Test Haberi Oluştur"))
        {
            CreateTestNews();
        }
        
        if (GUILayout.Button("⚽ Maç Haberi Oluştur"))
        {
            CreateMatchNews();
        }
        
        if (GUILayout.Button("💰 Transfer Haberi Oluştur"))
        {
            CreateTransferNews();
        }
        
        if (GUILayout.Button("🏥 Sakatlık Haberi Oluştur"))
        {
            CreateInjuryNews();
        }
        
        if (GUILayout.Button("🏆 Lig Haberi Oluştur"))
        {
            CreateLeagueNews();
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("🗑️ Tüm Haberleri Temizle"))
        {
            ClearAllNews();
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("📊 Haber İstatistikleri"))
        {
            ShowNewsStats();
        }
    }
    
    private void CreateTestNews()
    {
        var save = GetOrCreateSave();
        
        var news = new NewsItem
        {
            title = "🧪 Test Haberi - " + DateTime.Now.ToString("HH:mm:ss"),
            content = "Bu bir test haberidir. Sistem düzgün çalışıyor mu kontrol ediyoruz. Zaman: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"),
            type = NewsType.Match,
            source = "Test Sistemi",
            date = DateTime.Now,
            isRead = false
        };
        news.dateString = news.date.ToString("dd.MM.yyyy HH:mm");
        
        save.mediaData.AddNews(news);
        
        Debug.Log($"✅ Test haberi oluşturuldu: {news.title}");
        EditorUtility.DisplayDialog("Başarılı", "Test haberi oluşturuldu!", "Tamam");
    }
    
    private void CreateMatchNews()
    {
        var save = GetOrCreateSave();
        
        string playerName = save.playerProfile?.playerName ?? "Test Oyuncu";
        string teamName = save.playerProfile?.currentClubName ?? "Test FC";
        
        var news = new NewsItem
        {
            title = $"⚽ {playerName} Muhteşem Gol Attı!",
            content = $"{playerName}, {teamName} formasıyla oynadığı maçta muhteşem bir gol atarak takımını galibiyete taşıdı. Bu performans sezonun en iyi gollerinden biri olarak kayıtlara geçti.",
            type = NewsType.Match,
            source = "Spor Gazetesi",
            date = DateTime.Now,
            isRead = false
        };
        news.dateString = news.date.ToString("dd.MM.yyyy HH:mm");
        
        save.mediaData.AddNews(news);
        
        Debug.Log($"⚽ Maç haberi oluşturuldu: {news.title}");
        EditorUtility.DisplayDialog("Başarılı", "Maç haberi oluşturuldu!", "Tamam");
    }
    
    private void CreateTransferNews()
    {
        var save = GetOrCreateSave();
        
        string playerName = save.playerProfile?.playerName ?? "Test Oyuncu";
        
        var news = new NewsItem
        {
            title = $"💰 {playerName} İçin Transfer Teklifi!",
            content = $"Avrupa kulüplerinden {playerName} için 25 milyon euroluk transfer teklifi geldi. Kulüp yönetimi teklifi değerlendiriyor.",
            type = NewsType.Transfer,
            source = "Transfer Merkezi",
            date = DateTime.Now,
            isRead = false
        };
        news.dateString = news.date.ToString("dd.MM.yyyy HH:mm");
        
        save.mediaData.AddNews(news);
        
        Debug.Log($"💰 Transfer haberi oluşturuldu: {news.title}");
        EditorUtility.DisplayDialog("Başarılı", "Transfer haberi oluşturuldu!", "Tamam");
    }
    
    private void CreateInjuryNews()
    {
        var save = GetOrCreateSave();
        
        string playerName = save.playerProfile?.playerName ?? "Test Oyuncu";
        
        var news = new NewsItem
        {
            title = $"🏥 {playerName} Sakatlık Yaşadı",
            content = $"{playerName} antrenman sırasında hafif bir sakatlık geçirdi. Doktorlar 1-2 haftalık dinlenme önerdi. Oyuncu: 'Çabuk iyileşip sahalara döneceğim' dedi.",
            type = NewsType.Injury,
            source = "Sağlık Raporu",
            date = DateTime.Now,
            isRead = false
        };
        news.dateString = news.date.ToString("dd.MM.yyyy HH:mm");
        
        save.mediaData.AddNews(news);
        
        Debug.Log($"🏥 Sakatlık haberi oluşturuldu: {news.title}");
        EditorUtility.DisplayDialog("Başarılı", "Sakatlık haberi oluşturuldu!", "Tamam");
    }
    
    private void CreateLeagueNews()
    {
        var save = GetOrCreateSave();
        
        string teamName = save.playerProfile?.currentClubName ?? "Test FC";
        
        var news = new NewsItem
        {
            title = $"🏆 {teamName} Liderliğini Sürdürüyor",
            content = $"{teamName}, bu hafta oynadığı maç sonrası lig tablosundaki liderliğini korudu. Takım şampiyonluk yolunda emin adımlarla ilerliyor.",
            type = NewsType.League,
            source = "Lig Haberleri",
            date = DateTime.Now,
            isRead = false
        };
        news.dateString = news.date.ToString("dd.MM.yyyy HH:mm");
        
        save.mediaData.AddNews(news);
        
        Debug.Log($"🏆 Lig haberi oluşturuldu: {news.title}");
        EditorUtility.DisplayDialog("Başarılı", "Lig haberi oluşturuldu!", "Tamam");
    }
    
    private void ClearAllNews()
    {
        if (EditorUtility.DisplayDialog("Onay", "Tüm haberleri silmek istediğinizden emin misiniz?", "Evet", "Hayır"))
        {
            var save = GetOrCreateSave();
            save.mediaData.recentNews?.Clear();
            
            Debug.Log("🗑️ Tüm haberler temizlendi");
            EditorUtility.DisplayDialog("Başarılı", "Tüm haberler temizlendi!", "Tamam");
        }
    }
    
    private void ShowNewsStats()
    {
        var save = GetOrCreateSave();
        
        if (save.mediaData?.recentNews == null)
        {
            EditorUtility.DisplayDialog("İstatistikler", "Henüz haber yok!", "Tamam");
            return;
        }
        
        int totalNews = save.mediaData.recentNews.Count;
        int unreadNews = 0;
        
        foreach (var news in save.mediaData.recentNews)
        {
            if (!news.isRead) unreadNews++;
        }
        
        string stats = $"📊 Haber İstatistikleri\n\n";
        stats += $"📰 Toplam Haber: {totalNews}\n";
        stats += $"📬 Okunmamış: {unreadNews}\n";
        stats += $"👁️ Okunmuş: {totalNews - unreadNews}";
        
        EditorUtility.DisplayDialog("İstatistikler", stats, "Tamam");
        
        Debug.Log($"📊 Haber İstatistikleri - Toplam: {totalNews}, Okunmamış: {unreadNews}");
    }
    
    private SaveData GetOrCreateSave()
    {
        SaveData save = null;
        
        if (GameManager.Instance != null && GameManager.Instance.HasCurrentSave())
        {
            save = GameManager.Instance.CurrentSave;
        }
        else
        {
            // Test için basit save oluştur
            save = new SaveData();
            save.playerProfile = new PlayerProfile 
            { 
                playerName = "Test Oyuncu",
                currentClubName = "Test FC",
                position = PlayerPosition.SF
            };
        }
        
        if (save.mediaData == null)
        {
            save.mediaData = new MediaData();
        }
        
        return save;
    }
}