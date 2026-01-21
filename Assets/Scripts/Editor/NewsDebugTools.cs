using UnityEngine;
using UnityEditor;
using System;

/// <summary>
/// Haber sistemi debug araçları
/// </summary>
public class NewsDebugTools
{
    [MenuItem("TitanSoccer/News/🔍 Debug News System")]
    public static void DebugNewsSystem()
    {
        Debug.Log("🔍 Haber sistemi debug başlıyor...");
        
        // GameManager kontrolü
        if (GameManager.Instance == null)
        {
            Debug.LogError("❌ GameManager.Instance NULL!");
            EditorUtility.DisplayDialog("Debug", "GameManager bulunamadı!", "Tamam");
            return;
        }
        
        Debug.Log("✅ GameManager bulundu");
        
        // Save kontrolü
        if (!GameManager.Instance.HasCurrentSave())
        {
            Debug.LogWarning("⚠️ Aktif kayıt yok!");
            
            // Basit test kayıtı oluştur
            var testSave = new SaveData();
            testSave.playerProfile = new PlayerProfile { playerName = "Debug Player" };
            testSave.mediaData = new MediaData();
            
            GameManager.Instance.SetCurrentSave(testSave, 0);
            Debug.Log("🔧 Test kayıtı oluşturuldu");
        }
        
        var save = GameManager.Instance.CurrentSave;
        Debug.Log($"✅ Save bulundu: {save.playerProfile?.playerName}");
        
        // MediaData kontrolü
        if (save.mediaData == null)
        {
            save.mediaData = new MediaData();
            Debug.Log("🔧 MediaData oluşturuldu");
        }
        
        // Haber sayısı
        int newsCount = save.mediaData.recentNews?.Count ?? 0;
        Debug.Log($"📰 Mevcut haber sayısı: {newsCount}");
        
        // Test haberi ekle
        var testNews = new NewsItem
        {
            title = "🔍 Debug Test Haberi",
            content = "Bu bir debug test haberidir. Sistem çalışıyor!",
            type = NewsType.League,
            source = "Debug",
            date = DateTime.Now,
            isRead = false
        };
        testNews.dateString = testNews.date.ToString("dd.MM.yyyy HH:mm");
        
        save.mediaData.AddNews(testNews);
        Debug.Log("✅ Test haberi eklendi");
        
        // NewsSystem kontrolü
        if (NewsSystem.Instance == null)
        {
            Debug.LogWarning("⚠️ NewsSystem.Instance NULL!");
        }
        else
        {
            Debug.Log("✅ NewsSystem bulundu");
        }
        
        // NewsUI kontrolü
        var newsUI = GameObject.FindObjectOfType<NewsUI>();
        if (newsUI == null)
        {
            Debug.LogWarning("⚠️ NewsUI bulunamadı!");
        }
        else
        {
            Debug.Log($"✅ NewsUI bulundu: {newsUI.gameObject.name}");
            Debug.Log($"   - Aktif: {newsUI.gameObject.activeInHierarchy}");
            Debug.Log($"   - Enabled: {newsUI.enabled}");
        }
        
        // Sonuç
        string result = $"Debug Sonuçları:\n" +
                       $"• GameManager: ✅\n" +
                       $"• Save: ✅\n" +
                       $"• MediaData: ✅\n" +
                       $"• Haber Sayısı: {save.mediaData.recentNews?.Count ?? 0}\n" +
                       $"• NewsSystem: {(NewsSystem.Instance != null ? "✅" : "❌")}\n" +
                       $"• NewsUI: {(newsUI != null ? "✅" : "❌")}";
        
        EditorUtility.DisplayDialog("Debug Sonuçları", result, "Tamam");
    }
    
    [MenuItem("TitanSoccer/News/🔄 Force Refresh NewsUI")]
    public static void ForceRefreshNewsUI()
    {
        var newsUI = GameObject.FindObjectOfType<NewsUI>();
        if (newsUI != null)
        {
            newsUI.gameObject.SetActive(false);
            newsUI.gameObject.SetActive(true);
            Debug.Log("🔄 NewsUI yenilendi");
            EditorUtility.DisplayDialog("Yenileme", "NewsUI yenilendi!", "Tamam");
        }
        else
        {
            Debug.LogWarning("NewsUI bulunamadı!");
            EditorUtility.DisplayDialog("Hata", "NewsUI bulunamadı!", "Tamam");
        }
    }
}