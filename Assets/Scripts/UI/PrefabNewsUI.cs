using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Prefab tabanlı haber UI sistemi
/// </summary>
public class PrefabNewsUI : MonoBehaviour
{
    [Header("Prefab References")]
    public GameObject newsItemPrefab;
    public GameObject newsDetailPrefab;
    
    [Header("UI References")]
    public Transform newsListParent;
    public Button createNewsButton;
    public TextMeshProUGUI statusText;
    public ScrollRect scrollRect;
    
    [Header("Settings")]
    public int maxNewsCount = 50;
    public bool autoRefresh = true;
    
    private SaveData currentSave;
    private List<NewsItemUI> activeNewsItems = new List<NewsItemUI>();
    private NewsDetailUI currentDetailPanel;
    
    private void Start()
    {
        SetupUI();
        InitializeNewsSystem();
        LoadNews();
    }
    
    /// <summary>
    /// UI setup
    /// </summary>
    private void SetupUI()
    {
        if (createNewsButton != null)
        {
            createNewsButton.onClick.AddListener(CreateRandomNews);
        }
        
        UpdateStatusText();
    }
    
    /// <summary>
    /// Haber sistemini başlat
    /// </summary>
    private void InitializeNewsSystem()
    {
        // Test kayıtı oluştur
        if (currentSave == null)
        {
            currentSave = new SaveData();
            currentSave.playerProfile = new PlayerProfile 
            { 
                playerName = "Test Oyuncu",
                currentClubName = "Test FC",
                position = PlayerPosition.SF
            };
            currentSave.mediaData = new MediaData();
        }
        
        Debug.Log("📰 Prefab tabanlı haber sistemi başlatıldı!");
    }
    
    /// <summary>
    /// Haberleri yükle
    /// </summary>
    public void LoadNews()
    {
        if (currentSave?.mediaData?.recentNews == null) return;
        
        ClearNewsList();
        
        var sortedNews = currentSave.mediaData.recentNews
            .OrderByDescending(n => n.date)
            .Take(maxNewsCount)
            .ToList();
        
        foreach (var news in sortedNews)
        {
            CreateNewsItemUI(news);
        }
        
        UpdateStatusText();
        
        Debug.Log($"📰 {sortedNews.Count} haber yüklendi");
    }
    
    /// <summary>
    /// Haber item UI oluştur (Prefab kullanarak)
    /// </summary>
    private void CreateNewsItemUI(NewsItem news)
    {
        if (newsItemPrefab == null || newsListParent == null) 
        {
            Debug.LogWarning("NewsItemPrefab veya NewsListParent null!");
            return;
        }
        
        GameObject itemObj = Instantiate(newsItemPrefab, newsListParent);
        NewsItemUI itemUI = itemObj.GetComponent<NewsItemUI>();
        
        if (itemUI != null)
        {
            itemUI.Setup(news, OnNewsItemClicked);
            activeNewsItems.Add(itemUI);
        }
        else
        {
            Debug.LogWarning("NewsItemPrefab'da NewsItemUI bileşeni bulunamadı!");
        }
    }
    
    /// <summary>
    /// Haber item'ına tıklandığında
    /// </summary>
    private void OnNewsItemClicked(NewsItem news)
    {
        Debug.Log($"📖 Haber tıklandı: {news.title}");
        
        ShowNewsDetail(news);
        UpdateStatusText();
    }
    
    /// <summary>
    /// Haber detayını göster (Prefab kullanarak)
    /// </summary>
    private void ShowNewsDetail(NewsItem news)
    {
        if (newsDetailPrefab == null)
        {
            Debug.LogWarning("NewsDetailPrefab null!");
            return;
        }
        
        // Mevcut detay paneli varsa kapat
        if (currentDetailPanel != null)
        {
            Destroy(currentDetailPanel.gameObject);
        }
        
        // Yeni detay paneli oluştur
        GameObject detailObj = Instantiate(newsDetailPrefab, transform.root);
        currentDetailPanel = detailObj.GetComponent<NewsDetailUI>();
        
        if (currentDetailPanel != null)
        {
            currentDetailPanel.ShowDetail(news, OnDetailClosed);
        }
        else
        {
            Debug.LogWarning("NewsDetailPrefab'da NewsDetailUI bileşeni bulunamadı!");
        }
    }
    
    /// <summary>
    /// Detay paneli kapatıldığında
    /// </summary>
    private void OnDetailClosed()
    {
        if (currentDetailPanel != null)
        {
            Destroy(currentDetailPanel.gameObject);
            currentDetailPanel = null;
        }
        
        // Haberleri yenile (okundu durumu için)
        RefreshNewsList();
    }
    
    /// <summary>
    /// Rastgele haber oluştur
    /// </summary>
    public void CreateRandomNews()
    {
        if (currentSave?.mediaData == null) return;
        
        string[] titles = {
            "⚽ Muhteşem Gol Şovu!",
            "🏆 Şampiyonluk Yolunda",
            "💰 Bomba Transfer!",
            "🏥 Sakatlık Şoku",
            "📊 Rekor Performans",
            "🗣️ Önemli Açıklama",
            "🏅 Prestijli Ödül",
            "👂 Sıcak Söylenti",
            "✍️ Sözleşme İmzalandı"
        };
        
        string[] contents = {
            "Takımımız muhteşem bir performans sergileyerek rakibini mağlup etti. Taraftarlar coşkuyla maçı izledi.",
            "Lig tablosunda liderliğimizi sürdürüyoruz. Şampiyonluk hedefimize emin adımlarla ilerliyoruz.",
            "Yeni transferimiz takımımızı güçlendirecek. Kulüp yönetimi başarılı bir hamle gerçekleştirdi.",
            "Oyuncumuz maçta sakatlık yaşadı. Doktorlar detaylı muayene sonrası tedavi sürecini başlattı.",
            "Bu sezonki performansımız tüm rekorları kırıyor. İstatistikler takımımızın gücünü gösteriyor.",
            "Teknik direktörümüz basın toplantısında gelecek planları hakkında önemli açıklamalarda bulundu.",
            "Takımımız sezonun en başarılı takımı ödülünü kazandı. Bu başarı tüm camianın gururu.",
            "Avrupa kulüplerinden gelen transfer teklifleri gündemde. Yönetim henüz karar vermedi.",
            "Yıldız oyuncumuzla yeni sözleşme imzalandı. Anlaşma 3 yıl sürecek."
        };
        
        var newsTypes = new[] { 
            NewsType.Match, NewsType.League, NewsType.Transfer, 
            NewsType.Performance, NewsType.Injury, NewsType.TeamManagement,
            NewsType.Achievement, NewsType.Rumour, NewsType.Contract
        };
        
        int index = Random.Range(0, titles.Length);
        
        var news = new NewsItem
        {
            title = titles[index],
            content = contents[index],
            type = newsTypes[index],
            source = "Prefab Test Sistemi",
            date = System.DateTime.Now,
            isRead = false
        };
        news.dateString = news.date.ToString("dd.MM.yyyy HH:mm");
        
        currentSave.mediaData.AddNews(news);
        
        // Yeni haberi UI'a ekle
        CreateNewsItemUI(news);
        UpdateStatusText();
        
        Debug.Log($"✅ Yeni haber oluşturuldu: {news.title}");
    }
    
    /// <summary>
    /// Haber listesini temizle
    /// </summary>
    private void ClearNewsList()
    {
        foreach (var item in activeNewsItems)
        {
            if (item != null)
                Destroy(item.gameObject);
        }
        
        activeNewsItems.Clear();
    }
    
    /// <summary>
    /// Haber listesini yenile
    /// </summary>
    public void RefreshNewsList()
    {
        foreach (var item in activeNewsItems)
        {
            if (item != null)
                item.RefreshData();
        }
        
        UpdateStatusText();
    }
    
    /// <summary>
    /// Durum metnini güncelle
    /// </summary>
    private void UpdateStatusText()
    {
        if (statusText == null || currentSave?.mediaData?.recentNews == null) return;
        
        int totalNews = currentSave.mediaData.recentNews.Count;
        int unreadNews = currentSave.mediaData.recentNews.Count(n => !n.isRead);
        
        statusText.text = $"📰 Toplam: {totalNews} | 📬 Okunmamış: {unreadNews}";
    }
    
    /// <summary>
    /// Tüm haberleri temizle
    /// </summary>
    public void ClearAllNews()
    {
        if (currentSave?.mediaData?.recentNews != null)
        {
            currentSave.mediaData.recentNews.Clear();
        }
        
        ClearNewsList();
        UpdateStatusText();
        
        Debug.Log("🗑️ Tüm haberler temizlendi");
    }
    
    private void OnDestroy()
    {
        // Cleanup
        if (currentDetailPanel != null)
        {
            Destroy(currentDetailPanel.gameObject);
        }
        
        activeNewsItems.Clear();
    }
}