using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

/// <summary>
/// Basit haber test UI - Bağımsız çalışır
/// </summary>
public class SimpleNewsTestUI : MonoBehaviour
{
    [Header("UI Elemanları")]
    public Button createNewsButton;
    public Button generateNewsButton;
    public Button clearNewsButton;
    public TextMeshProUGUI statusText;
    public Transform newsListParent;
    public ScrollRect scrollRect;
    
    [Header("Test Verileri")]
    public List<NewsItem> testNews = new List<NewsItem>();
    
    private void Start()
    {
        Debug.Log("🚀 SimpleNewsTestUI başlatıldı!");
        SetupUI();
        UpdateStatus();
    }
    
    private void SetupUI()
    {
        if (createNewsButton != null)
            createNewsButton.onClick.AddListener(CreateTestNews);
            
        if (generateNewsButton != null)
            generateNewsButton.onClick.AddListener(GenerateRandomNews);
            
        if (clearNewsButton != null)
            clearNewsButton.onClick.AddListener(ClearAllNews);
    }
    
    /// <summary>
    /// Test haberi oluştur
    /// </summary>
    public void CreateTestNews()
    {
        var news = new NewsItem
        {
            title = $"🧪 Test Haberi #{testNews.Count + 1}",
            content = $"Bu bir test haberidir. Oluşturulma zamanı: {DateTime.Now:HH:mm:ss}",
            type = NewsType.League,
            source = "Test Sistemi",
            date = DateTime.Now,
            isRead = false
        };
        news.dateString = news.date.ToString("dd.MM.yyyy HH:mm");
        
        testNews.Add(news);
        RefreshNewsList();
        UpdateStatus();
        
        Debug.Log($"✅ Test haberi oluşturuldu: {news.title}");
    }
    
    /// <summary>
    /// Rastgele haber üret
    /// </summary>
    public void GenerateRandomNews()
    {
        string[] titles = {
            "⚽ Muhteşem Gol!",
            "🏆 Şampiyonluk Yarışı",
            "💰 Transfer Bombası",
            "🏥 Sakatlık Haberi",
            "📊 Sezon İstatistikleri",
            "🗣️ Teknik Direktör Konuştu",
            "🏅 Ödül Töreni",
            "👂 Transfer Söylentisi"
        };
        
        string[] contents = {
            "Takımımız muhteşem bir performans sergiledi.",
            "Lig tablosunda önemli değişiklikler yaşandı.",
            "Yeni transfer kulübü güçlendirecek.",
            "Oyuncu tedavi sürecine başladı.",
            "Bu sezonki rakamlar etkileyici.",
            "Basın toplantısında önemli açıklamalar yapıldı.",
            "Başarılı sezon ödüllendirildi.",
            "Kulüpler arası görüşmeler sürüyor."
        };
        
        var newsTypes = new[] { NewsType.Match, NewsType.Transfer, NewsType.League, NewsType.Performance };
        
        int index = UnityEngine.Random.Range(0, titles.Length);
        
        var news = new NewsItem
        {
            title = titles[index],
            content = contents[index],
            type = newsTypes[UnityEngine.Random.Range(0, newsTypes.Length)],
            source = "Rastgele Haber",
            date = DateTime.Now.AddHours(-UnityEngine.Random.Range(0, 24)),
            isRead = false
        };
        news.dateString = news.date.ToString("dd.MM.yyyy HH:mm");
        
        testNews.Add(news);
        RefreshNewsList();
        UpdateStatus();
        
        Debug.Log($"🎲 Rastgele haber oluşturuldu: {news.title}");
    }
    
    /// <summary>
    /// Tüm haberleri temizle
    /// </summary>
    public void ClearAllNews()
    {
        testNews.Clear();
        RefreshNewsList();
        UpdateStatus();
        
        Debug.Log("🗑️ Tüm haberler temizlendi");
    }
    
    /// <summary>
    /// Haber listesini yenile
    /// </summary>
    private void RefreshNewsList()
    {
        if (newsListParent == null) return;
        
        // Mevcut item'ları temizle
        for (int i = newsListParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(newsListParent.GetChild(i).gameObject);
        }
        
        // Yeni item'ları oluştur
        foreach (var news in testNews)
        {
            CreateNewsItemUI(news);
        }
    }
    
    /// <summary>
    /// Haber item UI oluştur
    /// </summary>
    private void CreateNewsItemUI(NewsItem news)
    {
        // Ana container
        GameObject itemObj = new GameObject($"NewsItem_{news.title}");
        itemObj.transform.SetParent(newsListParent);
        
        RectTransform rect = itemObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, 80);
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(0.5f, 1);
        
        // Arka plan
        Image bg = itemObj.AddComponent<Image>();
        bg.color = new Color(0.2f, 0.2f, 0.3f, 0.8f);
        
        // Button
        Button button = itemObj.AddComponent<Button>();
        button.onClick.AddListener(() => OnNewsClicked(news));
        
        // Başlık text
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(itemObj.transform);
        
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.5f);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = new Vector2(10, 0);
        titleRect.offsetMax = new Vector2(-10, -5);
        
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = news.title;
        titleText.fontSize = 16;
        titleText.color = news.isRead ? Color.gray : Color.white;
        titleText.fontStyle = FontStyles.Bold;
        
        // Tarih text
        GameObject dateObj = new GameObject("Date");
        dateObj.transform.SetParent(itemObj.transform);
        
        RectTransform dateRect = dateObj.AddComponent<RectTransform>();
        dateRect.anchorMin = new Vector2(0, 0);
        dateRect.anchorMax = new Vector2(1, 0.5f);
        dateRect.offsetMin = new Vector2(10, 5);
        dateRect.offsetMax = new Vector2(-10, 0);
        
        TextMeshProUGUI dateText = dateObj.AddComponent<TextMeshProUGUI>();
        dateText.text = $"{news.dateString} - {news.source}";
        dateText.fontSize = 12;
        dateText.color = Color.gray;
    }
    
    /// <summary>
    /// Habere tıklandığında
    /// </summary>
    private void OnNewsClicked(NewsItem news)
    {
        news.isRead = true;
        RefreshNewsList();
        
        Debug.Log($"📰 Haber okundu: {news.title}");
        Debug.Log($"📝 İçerik: {news.content}");
    }
    
    /// <summary>
    /// Durum metnini güncelle
    /// </summary>
    private void UpdateStatus()
    {
        if (statusText != null)
        {
            int unreadCount = 0;
            foreach (var news in testNews)
            {
                if (!news.isRead) unreadCount++;
            }
            
            statusText.text = $"📰 Toplam: {testNews.Count} | 📬 Okunmamış: {unreadCount}";
        }
    }
    
    /// <summary>
    /// NewsGenerator ile test
    /// </summary>
    public void TestNewsGenerator()
    {
        // Minimal test verisi oluştur
        var testSave = new SaveData();
        testSave.playerProfile = new PlayerProfile 
        { 
            playerName = "Test Oyuncu",
            currentClubName = "Test FC"
        };
        testSave.mediaData = new MediaData();
        
        // GameManager'a set et (eğer varsa)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetCurrentSave(testSave, 0);
        }
        
        // NewsGenerator test et
        if (NewsGenerator.Instance != null)
        {
            NewsGenerator.Instance.GenerateTransferNews("Test Oyuncu", "Eski Takım", "Yeni Takım", 15.5f, 3);
            Debug.Log("🔧 NewsGenerator test edildi");
        }
        else
        {
            Debug.LogWarning("NewsGenerator bulunamadı!");
        }
    }
}