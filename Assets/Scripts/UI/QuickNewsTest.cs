using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// Hızlı haber testi - Prefab hazırlığı ile
/// </summary>
public class QuickNewsTest : MonoBehaviour
{
    [Header("Prefab References (İleride kullanılacak)")]
    public GameObject newsItemPrefab;
    public GameObject newsDetailPrefab;
    
    private Transform newsListParent;
    private SaveData currentSave;
    private GameObject detailPanel;
    private TextMeshProUGUI detailTitle;
    private TextMeshProUGUI detailContent;
    private TextMeshProUGUI detailDate;
    private TextMeshProUGUI detailSource;
    private TextMeshProUGUI detailType;
    
    private void Start()
    {
        CreateSimpleUI();
        TestNewsSystem();
    }
    
    private void Update()
    {
        // ESC tuşu ile detay panelini kapat
        if (Input.GetKeyDown(KeyCode.Escape) && detailPanel != null && detailPanel.activeSelf)
        {
            CloseDetailPanel();
        }
    }
    
    private void CreateSimpleUI()
    {
        // Ana panel oluştur
        GameObject panel = new GameObject("TestPanel");
        panel.transform.SetParent(transform);
        
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.2f, 0.9f);
        
        // Başlık text
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(panel.transform);
        
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.8f);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
        
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "📰 HABER SİSTEMİ TEST";
        titleText.fontSize = 24;
        titleText.color = Color.white;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.fontStyle = FontStyles.Bold;
        
        // Test butonu
        GameObject buttonObj = new GameObject("TestButton");
        buttonObj.transform.SetParent(panel.transform);
        
        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.1f, 0.7f);
        buttonRect.anchorMax = new Vector2(0.9f, 0.8f);
        buttonRect.offsetMin = Vector2.zero;
        buttonRect.offsetMax = Vector2.zero;
        
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.6f, 0.2f, 1f);
        
        Button button = buttonObj.AddComponent<Button>();
        button.onClick.AddListener(TestNewsSystem);
        
        // Buton text
        GameObject buttonTextObj = new GameObject("ButtonText");
        buttonTextObj.transform.SetParent(buttonObj.transform);
        
        RectTransform buttonTextRect = buttonTextObj.AddComponent<RectTransform>();
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.offsetMin = Vector2.zero;
        buttonTextRect.offsetMax = Vector2.zero;
        
        TextMeshProUGUI buttonText = buttonTextObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = "🧪 YENİ HABER OLUŞTUR";
        buttonText.fontSize = 16;
        buttonText.color = Color.white;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.fontStyle = FontStyles.Bold;
        
        // Haber listesi scroll area
        GameObject scrollObj = new GameObject("NewsScrollArea");
        scrollObj.transform.SetParent(panel.transform);
        
        RectTransform scrollRect = scrollObj.AddComponent<RectTransform>();
        scrollRect.anchorMin = new Vector2(0.05f, 0.1f);
        scrollRect.anchorMax = new Vector2(0.95f, 0.65f);
        scrollRect.offsetMin = Vector2.zero;
        scrollRect.offsetMax = Vector2.zero;
        
        Image scrollBg = scrollObj.AddComponent<Image>();
        scrollBg.color = new Color(0.05f, 0.05f, 0.1f, 0.8f);
        
        // Haber listesi parent
        GameObject listObj = new GameObject("NewsList");
        listObj.transform.SetParent(scrollObj.transform);
        
        RectTransform listRect = listObj.AddComponent<RectTransform>();
        listRect.anchorMin = Vector2.zero;
        listRect.anchorMax = Vector2.one;
        listRect.offsetMin = new Vector2(10, 10);
        listRect.offsetMax = new Vector2(-10, -10);
        
        VerticalLayoutGroup layout = listObj.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 5;
        layout.padding = new RectOffset(5, 5, 5, 5);
        layout.childControlHeight = false;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;
        
        ContentSizeFitter fitter = listObj.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        newsListParent = listObj.transform;
        
        // Haber detay paneli oluştur
        CreateDetailPanel(panel.transform);
        
        Debug.Log("✅ Basit UI oluşturuldu!");
    }
    
    private void CreateDetailPanel(Transform parent)
    {
        // Detay paneli (başlangıçta gizli)
        detailPanel = new GameObject("DetailPanel");
        detailPanel.transform.SetParent(parent);
        
        RectTransform detailRect = detailPanel.AddComponent<RectTransform>();
        detailRect.anchorMin = Vector2.zero;
        detailRect.anchorMax = Vector2.one;
        detailRect.offsetMin = Vector2.zero;
        detailRect.offsetMax = Vector2.zero;
        
        // Yarı şeffaf arka plan (tıklanabilir)
        Image detailBg = detailPanel.AddComponent<Image>();
        detailBg.color = new Color(0, 0, 0, 0.8f);
        
        // Arka plana tıklayınca kapat
        Button bgButton = detailPanel.AddComponent<Button>();
        bgButton.onClick.AddListener(CloseDetailPanel);
        
        // İçerik paneli
        GameObject contentPanel = new GameObject("ContentPanel");
        contentPanel.transform.SetParent(detailPanel.transform);
        
        RectTransform contentRect = contentPanel.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0.1f, 0.1f);
        contentRect.anchorMax = new Vector2(0.9f, 0.9f);
        contentRect.offsetMin = Vector2.zero;
        contentRect.offsetMax = Vector2.zero;
        
        Image contentBg = contentPanel.AddComponent<Image>();
        contentBg.color = new Color(0.1f, 0.1f, 0.2f, 0.95f);
        
        // Kapatma butonu
        GameObject closeButton = new GameObject("CloseButton");
        closeButton.transform.SetParent(contentPanel.transform);
        
        RectTransform closeRect = closeButton.AddComponent<RectTransform>();
        closeRect.anchorMin = new Vector2(0.9f, 0.9f);
        closeRect.anchorMax = new Vector2(1f, 1f);
        closeRect.offsetMin = new Vector2(-40, -40);
        closeRect.offsetMax = Vector2.zero;
        
        Image closeBg = closeButton.AddComponent<Image>();
        closeBg.color = new Color(0.8f, 0.2f, 0.2f, 1f);
        
        Button closeBtn = closeButton.AddComponent<Button>();
        closeBtn.onClick.AddListener(CloseDetailPanel);
        
        // Kapatma butonu text
        GameObject closeTextObj = new GameObject("CloseText");
        closeTextObj.transform.SetParent(closeButton.transform);
        
        RectTransform closeTextRect = closeTextObj.AddComponent<RectTransform>();
        closeTextRect.anchorMin = Vector2.zero;
        closeTextRect.anchorMax = Vector2.one;
        closeTextRect.offsetMin = Vector2.zero;
        closeTextRect.offsetMax = Vector2.zero;
        
        TextMeshProUGUI closeText = closeTextObj.AddComponent<TextMeshProUGUI>();
        closeText.text = "✕";
        closeText.fontSize = 20;
        closeText.color = Color.white;
        closeText.alignment = TextAlignmentOptions.Center;
        closeText.fontStyle = FontStyles.Bold;
        
        // Başlık
        GameObject titleObj = new GameObject("DetailTitle");
        titleObj.transform.SetParent(contentPanel.transform);
        
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.05f, 0.8f);
        titleRect.anchorMax = new Vector2(0.95f, 0.95f);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
        
        detailTitle = titleObj.AddComponent<TextMeshProUGUI>();
        detailTitle.fontSize = 20;
        detailTitle.color = Color.white;
        detailTitle.alignment = TextAlignmentOptions.TopLeft;
        detailTitle.fontStyle = FontStyles.Bold;
        detailTitle.enableWordWrapping = true;
        
        // Tür ve tarih bilgisi
        GameObject infoObj = new GameObject("DetailInfo");
        infoObj.transform.SetParent(contentPanel.transform);
        
        RectTransform infoRect = infoObj.AddComponent<RectTransform>();
        infoRect.anchorMin = new Vector2(0.05f, 0.7f);
        infoRect.anchorMax = new Vector2(0.95f, 0.8f);
        infoRect.offsetMin = Vector2.zero;
        infoRect.offsetMax = Vector2.zero;
        
        detailDate = infoObj.AddComponent<TextMeshProUGUI>();
        detailDate.fontSize = 12;
        detailDate.color = Color.gray;
        detailDate.alignment = TextAlignmentOptions.TopLeft;
        
        // Tür bilgisi
        GameObject typeObj = new GameObject("DetailType");
        typeObj.transform.SetParent(contentPanel.transform);
        
        RectTransform typeRect = typeObj.AddComponent<RectTransform>();
        typeRect.anchorMin = new Vector2(0.05f, 0.6f);
        typeRect.anchorMax = new Vector2(0.95f, 0.7f);
        typeRect.offsetMin = Vector2.zero;
        typeRect.offsetMax = Vector2.zero;
        
        detailType = typeObj.AddComponent<TextMeshProUGUI>();
        detailType.fontSize = 14;
        detailType.color = Color.cyan;
        detailType.alignment = TextAlignmentOptions.TopLeft;
        detailType.fontStyle = FontStyles.Bold;
        
        // İçerik
        GameObject contentObj = new GameObject("DetailContent");
        contentObj.transform.SetParent(contentPanel.transform);
        
        RectTransform contentTextRect = contentObj.AddComponent<RectTransform>();
        contentTextRect.anchorMin = new Vector2(0.05f, 0.2f);
        contentTextRect.anchorMax = new Vector2(0.95f, 0.6f);
        contentTextRect.offsetMin = Vector2.zero;
        contentTextRect.offsetMax = Vector2.zero;
        
        detailContent = contentObj.AddComponent<TextMeshProUGUI>();
        detailContent.fontSize = 14;
        detailContent.color = Color.white;
        detailContent.alignment = TextAlignmentOptions.TopLeft;
        detailContent.enableWordWrapping = true;
        
        // Kaynak bilgisi
        GameObject sourceObj = new GameObject("DetailSource");
        sourceObj.transform.SetParent(contentPanel.transform);
        
        RectTransform sourceRect = sourceObj.AddComponent<RectTransform>();
        sourceRect.anchorMin = new Vector2(0.05f, 0.05f);
        sourceRect.anchorMax = new Vector2(0.95f, 0.2f);
        sourceRect.offsetMin = Vector2.zero;
        sourceRect.offsetMax = Vector2.zero;
        
        detailSource = sourceObj.AddComponent<TextMeshProUGUI>();
        detailSource.fontSize = 12;
        detailSource.color = Color.yellow;
        detailSource.alignment = TextAlignmentOptions.BottomRight;
        detailSource.fontStyle = FontStyles.Italic;
        
        // Başlangıçta gizle
        detailPanel.SetActive(false);
    }
    
    private void TestNewsSystem()
    {
        Debug.Log("🚀 Haber sistemi testi başlıyor...");
        
        // Basit test kayıtı oluştur (eğer yoksa)
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
        
        // Rastgele haber oluştur
        string[] titles = {
            "⚽ Muhteşem Gol Atıldı!",
            "🏆 Lig Liderliği Devam Ediyor",
            "💰 Yeni Transfer Gerçekleşti",
            "🏥 Oyuncu Sakatlığı",
            "📊 Sezon İstatistikleri",
            "🗣️ Teknik Direktör Konuştu",
            "🏅 Ödül Kazanıldı",
            "👂 Transfer Söylentisi"
        };
        
        string[] contents = {
            "Takımımız muhteşem bir performans sergiledi ve önemli bir galibiyet aldı.",
            "Lig tablosunda liderliğimizi sürdürüyoruz. Oyuncular mükemmel form gösteriyor.",
            "Yeni transferimiz takımımızı güçlendirecek. Kulüp yönetimi başarılı bir hamle yaptı.",
            "Oyuncumuz hafif bir sakatlık geçirdi. Doktorlar 1-2 haftalık dinlenme önerdi.",
            "Bu sezonki istatistiklerimiz oldukça etkileyici. Hedeflerimize yaklaşıyoruz.",
            "Teknik direktörümüz basın toplantısında önemli açıklamalarda bulundu.",
            "Takımımız sezonun en iyi performansı ödülünü kazandı.",
            "Avrupa kulüplerinden transfer teklifleri gelmeye devam ediyor."
        };
        
        var newsTypes = new[] { NewsType.Match, NewsType.League, NewsType.Transfer, NewsType.Performance, NewsType.Injury };
        
        int index = UnityEngine.Random.Range(0, titles.Length);
        
        // Test haberi oluştur
        var testNews = new NewsItem
        {
            title = titles[index],
            content = contents[index],
            type = newsTypes[UnityEngine.Random.Range(0, newsTypes.Length)],
            source = "Test Sistemi",
            date = DateTime.Now,
            isRead = false
        };
        testNews.dateString = testNews.date.ToString("dd.MM.yyyy HH:mm");
        
        currentSave.mediaData.AddNews(testNews);
        
        // UI'da göster
        CreateNewsItemUI(testNews);
        
        Debug.Log($"✅ Test haberi oluşturuldu: {testNews.title}");
        Debug.Log($"📰 Toplam haber sayısı: {currentSave.mediaData.recentNews?.Count ?? 0}");
    }
    
    private void CreateNewsItemUI(NewsItem news)
    {
        if (newsListParent == null) return;
        
        // Eğer prefab varsa onu kullan, yoksa runtime oluştur
        if (newsItemPrefab != null)
        {
            CreateNewsItemFromPrefab(news);
        }
        else
        {
            CreateNewsItemRuntime(news);
        }
    }
    
    /// <summary>
    /// Prefab kullanarak haber item oluştur
    /// </summary>
    private void CreateNewsItemFromPrefab(NewsItem news)
    {
        GameObject itemObj = Instantiate(newsItemPrefab, newsListParent);
        
        // NewsItemUI bileşeni varsa kullan
        NewsItemUI itemUI = itemObj.GetComponent<NewsItemUI>();
        if (itemUI != null)
        {
            itemUI.Setup(news, OnNewsClicked);
            Debug.Log($"📰 Prefab ile haber oluşturuldu: {news.title}");
        }
        else
        {
            // Prefab'da NewsItemUI yoksa manuel setup
            SetupPrefabManually(itemObj, news);
        }
    }
    
    /// <summary>
    /// Prefab'ı manuel olarak setup et
    /// </summary>
    private void SetupPrefabManually(GameObject itemObj, NewsItem news)
    {
        // Button setup
        Button button = itemObj.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnNewsClicked(news));
        }
        
        // Text bileşenlerini bul ve ayarla
        TextMeshProUGUI[] texts = itemObj.GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length > 0)
        {
            texts[0].text = news.title;
            texts[0].color = news.isRead ? Color.gray : Color.white;
        }
        
        Debug.Log($"📰 Prefab manuel setup: {news.title}");
    }
    
    /// <summary>
    /// Runtime'da haber item oluştur (eski yöntem)
    /// </summary>
    private void CreateNewsItemRuntime(NewsItem news)
    {
        if (newsListParent == null) return;
        
    /// <summary>
    /// Runtime'da haber item oluştur (eski yöntem)
    /// </summary>
    private void CreateNewsItemRuntime(NewsItem news)
    {
        // Haber item container
        GameObject itemObj = new GameObject($"NewsItem_{news.title}");
        itemObj.transform.SetParent(newsListParent);
        
        RectTransform itemRect = itemObj.AddComponent<RectTransform>();
        itemRect.sizeDelta = new Vector2(0, 80);
        
        // Arka plan
        Image bg = itemObj.AddComponent<Image>();
        bg.color = new Color(0.15f, 0.15f, 0.25f, 0.9f);
        
        // Button (tıklanabilir)
        Button button = itemObj.AddComponent<Button>();
        button.onClick.AddListener(() => OnNewsClicked(news));
        
        // Başlık
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(itemObj.transform);
        
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.5f);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = new Vector2(10, 0);
        titleRect.offsetMax = new Vector2(-10, -5);
        
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = news.title;
        titleText.fontSize = 14;
        titleText.color = news.isRead ? Color.gray : Color.white;
        titleText.fontStyle = FontStyles.Bold;
        titleText.alignment = TextAlignmentOptions.TopLeft;
        
        // Tarih ve kaynak
        GameObject dateObj = new GameObject("Date");
        dateObj.transform.SetParent(itemObj.transform);
        
        RectTransform dateRect = dateObj.AddComponent<RectTransform>();
        dateRect.anchorMin = new Vector2(0, 0);
        dateRect.anchorMax = new Vector2(1, 0.5f);
        dateRect.offsetMin = new Vector2(10, 5);
        dateRect.offsetMax = new Vector2(-10, 0);
        
        TextMeshProUGUI dateText = dateObj.AddComponent<TextMeshProUGUI>();
        dateText.text = $"{news.dateString} - {news.source}";
        dateText.fontSize = 10;
        dateText.color = Color.gray;
        dateText.alignment = TextAlignmentOptions.BottomLeft;
        
        Debug.Log($"📰 Runtime haber oluşturuldu: {news.title}");
    }
    }
    
    private void OnNewsClicked(NewsItem news)
    {
        news.isRead = true;
        Debug.Log($"📖 Haber okundu: {news.title}");
        
        // Detay panelini aç
        ShowNewsDetail(news);
        
        // UI'ı yenile (renk değişimi için)
        RefreshNewsList();
    }
    
    private void ShowNewsDetail(NewsItem news)
    {
        if (detailPanel == null) return;
        
        // Detay bilgilerini doldur
        if (detailTitle != null)
            detailTitle.text = news.title;
            
        if (detailContent != null)
            detailContent.text = news.content;
            
        if (detailDate != null)
            detailDate.text = $"📅 {news.dateString}";
            
        if (detailSource != null)
            detailSource.text = $"📰 Kaynak: {news.source}";
            
        if (detailType != null)
        {
            string typeIcon = GetNewsTypeIcon(news.type);
            string typeName = GetNewsTypeName(news.type);
            detailType.text = $"{typeIcon} {typeName}";
        }
        
        // Paneli göster
        detailPanel.SetActive(true);
        
        Debug.Log($"📋 Detay paneli açıldı: {news.title}");
    }
    
    private void CloseDetailPanel()
    {
        Debug.Log("🔄 CloseDetailPanel çağrıldı");
        
        if (detailPanel != null)
        {
            detailPanel.SetActive(false);
            Debug.Log("❌ Detay paneli kapatıldı");
        }
        else
        {
            Debug.LogWarning("⚠️ detailPanel null!");
        }
    }
    
    private string GetNewsTypeIcon(NewsType type)
    {
        return type switch
        {
            NewsType.Match => "⚽",
            NewsType.Transfer => "💰",
            NewsType.Injury => "🏥",
            NewsType.Performance => "📊",
            NewsType.League => "🏆",
            NewsType.Contract => "✍️",
            NewsType.TeamManagement => "🗣️",
            NewsType.Achievement => "🏅",
            NewsType.Rumour => "👂",
            _ => "📰"
        };
    }
    
    private string GetNewsTypeName(NewsType type)
    {
        return type switch
        {
            NewsType.Match => "Maç Haberi",
            NewsType.Transfer => "Transfer Haberi",
            NewsType.Injury => "Sakatlık Haberi",
            NewsType.Performance => "Performans Haberi",
            NewsType.League => "Lig Haberi",
            NewsType.Contract => "Sözleşme Haberi",
            NewsType.TeamManagement => "Yönetim Haberi",
            NewsType.Achievement => "Başarı Haberi",
            NewsType.Rumour => "Söylenti",
            _ => "Genel Haber"
        };
    }
    
    private void RefreshNewsList()
    {
        if (newsListParent == null || currentSave?.mediaData?.recentNews == null) return;
        
        // Mevcut item'ları temizle
        for (int i = newsListParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(newsListParent.GetChild(i).gameObject);
        }
        
        // Tüm haberleri yeniden oluştur
        foreach (var news in currentSave.mediaData.recentNews)
        {
            CreateNewsItemUI(news);
        }
    }
}