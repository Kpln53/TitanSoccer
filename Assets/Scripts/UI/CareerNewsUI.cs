using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Career Hub için gelişmiş haber UI sistemi - Prefab tabanlı
/// </summary>
public class CareerNewsUI : MonoBehaviour
{
    [Header("Prefab References")]
    public GameObject newsItemPrefab;
    public GameObject newsDetailPrefab;
    
    [Header("UI References")]
    public Transform newsListParent;
    public ScrollRect scrollRect;
    public TextMeshProUGUI statusText;
    public Button refreshButton;
    public Button generateTestNewsButton;
    
    [Header("Featured News")]
    public NewsItemUI featuredNewsDisplay;

    [Header("Filter Options")]
    public Dropdown newsTypeFilter;
    public Toggle showOnlyUnreadToggle;
    
    [Header("Settings")]
    public int maxNewsCount = 50;
    public bool autoRefresh = true;
    
    private SaveData currentSave;
    private List<NewsItemUI> activeNewsItems = new List<NewsItemUI>();
    private NewsDetailUI currentDetailPanel;
    private NewsType? currentFilterType = null; // Null = All
    
    private void OnEnable()
    {
        SetupReferences();
        InitializeNewsSystem();
        LoadNews();
        SetupUI();
    }
    
    /// <summary>
    /// Referansları otomatik olarak ayarla
    /// </summary>
    private void SetupReferences()
    {
        // newsListParent'i Content olarak ayarla
        if (newsListParent == null)
        {
            // Sadece bu panelin altındaki Content'i bul
            Transform contentTr = transform.Find("ListSection/NewsScrollView/Viewport/Content");
            if (contentTr != null)
            {
                newsListParent = contentTr;
                Debug.Log("[CareerNewsUI] newsListParent otomatik ayarlandı: Content");
            }
        }
        
        // scrollRect'i NewsScrollView olarak ayarla
        if (scrollRect == null)
        {
            Transform scrollTr = transform.Find("ListSection/NewsScrollView");
            if (scrollTr != null)
            {
                scrollRect = scrollTr.GetComponent<ScrollRect>();
                Debug.Log("[CareerNewsUI] scrollRect otomatik ayarlandı: NewsScrollView");
            }
        }
        
        // statusText'i StatusText olarak ayarla
        if (statusText == null)
        {
            Transform statusTr = transform.Find("StatusText");
            if (statusTr != null)
            {
                statusText = statusTr.GetComponent<TextMeshProUGUI>();
                Debug.Log("[CareerNewsUI] statusText otomatik ayarlandı: StatusText");
            }
        }
        
        // refreshButton'ı RefreshButton olarak ayarla
        if (refreshButton == null)
        {
            Transform refreshTr = transform.Find("FeaturedSection/RefreshButton");
            if (refreshTr != null)
            {
                refreshButton = refreshTr.GetComponent<Button>();
                Debug.Log("[CareerNewsUI] refreshButton otomatik ayarlandı: RefreshButton");
            }
        }
        
        // generateTestNewsButton'ı TestNewsButton olarak ayarla
        if (generateTestNewsButton == null)
        {
            Transform testTr = transform.Find("TestNewsButton");
            if (testTr != null)
            {
                generateTestNewsButton = testTr.GetComponent<Button>();
                Debug.Log("[CareerNewsUI] generateTestNewsButton otomatik ayarlandı: TestNewsButton");
            }
        }

        // Filtre butonlarını otomatik bağla
        SetupFilterButtonsRuntime();
    }

    /// <summary>
    /// Filtre butonlarını runtime'da bul ve bağla
    /// </summary>
    private void SetupFilterButtonsRuntime()
    {
        Transform filterSection = transform.Find("FilterSection");
        if (filterSection != null)
        {
            WireRuntimeFilter(filterSection, "Tümü", "Tümü");
            WireRuntimeFilter(filterSection, "Transfer", "Transfer");
            WireRuntimeFilter(filterSection, "Lig", "Lig");
            WireRuntimeFilter(filterSection, "Kulüp", "Kulüp");
        }
    }

    private void WireRuntimeFilter(Transform parent, string btnName, string category)
    {
        Transform btnTr = parent.Find(btnName);
        if (btnTr != null)
        {
            Button btn = btnTr.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => FilterByString(category));
                Debug.Log($"[CareerNewsUI] Runtime filter wired: {btnName}");
            }
        }
    }
    
    private void OnDisable()
    {
        // Detay paneli varsa kapat
        if (currentDetailPanel != null)
        {
            Destroy(currentDetailPanel.gameObject);
            currentDetailPanel = null;
        }
    }
    
    /// <summary>
    /// UI setup
    /// </summary>
    private void SetupUI()
    {
        if (refreshButton != null)
        {
            refreshButton.onClick.RemoveAllListeners();
            refreshButton.onClick.AddListener(RefreshNews);
        }
        
        if (generateTestNewsButton != null)
        {
            generateTestNewsButton.onClick.RemoveAllListeners();
            generateTestNewsButton.onClick.AddListener(GenerateTestNews);
        }
        
        if (newsTypeFilter != null)
        {
            newsTypeFilter.onValueChanged.RemoveAllListeners();
            newsTypeFilter.onValueChanged.AddListener(OnNewsTypeFilterChanged);
            SetupNewsTypeFilter();
        }
        
        if (showOnlyUnreadToggle != null)
        {
            showOnlyUnreadToggle.onValueChanged.RemoveAllListeners();
            showOnlyUnreadToggle.onValueChanged.AddListener(OnUnreadFilterChanged);
        }
        
        UpdateStatusText();
    }
    
    /// <summary>
    /// Haber türü filtresini setup et
    /// </summary>
    private void SetupNewsTypeFilter()
    {
        if (newsTypeFilter == null) return;
        
        newsTypeFilter.ClearOptions();
        
        var options = new List<string> { "Tüm Haberler" };
        var newsTypes = System.Enum.GetValues(typeof(NewsType));
        
        foreach (NewsType type in newsTypes)
        {
            string typeName = NewsTemplateManager.GetTypeName(type);
            string typeIcon = NewsTemplateManager.GetTypeIcon(type);
            options.Add($"{typeIcon} {typeName}");
        }
        
        newsTypeFilter.AddOptions(options);
    }
    
    /// <summary>
    /// Haber sistemini başlat
    /// </summary>
    private void InitializeNewsSystem()
    {
        if (GameManager.Instance != null && GameManager.Instance.HasCurrentSave())
        {
            currentSave = GameManager.Instance.CurrentSave;
            
            if (currentSave.mediaData == null)
            {
                currentSave.mediaData = new MediaData();
            }
        }
        else
        {
            // Test için basit save oluştur
            currentSave = new SaveData();
            currentSave.playerProfile = new PlayerProfile 
            { 
                playerName = "Test Oyuncu",
                currentClubName = "Test FC",
                position = PlayerPosition.SF
            };
            currentSave.mediaData = new MediaData();
            
            Debug.LogWarning("[CareerNewsUI] GameManager yok, test modu aktif");
        }
    }
    
    /// <summary>
    /// Haberleri yükle
    /// </summary>
    public void LoadNews()
    {
        Debug.Log("[CareerNewsUI] LoadNews başlatıldı");
        
        if (currentSave?.mediaData?.recentNews == null) 
        {
            Debug.LogWarning("[CareerNewsUI] Haber verisi yok - currentSave veya mediaData null");
            
            // Test için birkaç haber oluştur
            if (currentSave?.mediaData != null)
            {
                Debug.Log("[CareerNewsUI] Test haberleri oluşturuluyor...");
                GenerateMultipleTestNews();
                return;
            }
            return;
        }
        
        Debug.Log($"[CareerNewsUI] Toplam haber sayısı: {currentSave.mediaData.recentNews.Count}");
        
        ClearNewsList();
        
        var allNews = currentSave.mediaData.recentNews;
        
        // Filtreleme uygula
        var filteredNews = ApplyFilters(allNews);
        
        Debug.Log($"[CareerNewsUI] Filtrelenmiş haber sayısı: {filteredNews.Count}");
        
        // Tarihe göre sırala ve limitle
        var sortedNews = filteredNews
            .OrderByDescending(n => n.date)
            .Take(maxNewsCount)
            .ToList();
        
        Debug.Log($"[CareerNewsUI] Gösterilecek haber sayısı: {sortedNews.Count}");
        
        if (newsListParent == null)
        {
            Debug.LogError("[CareerNewsUI] newsListParent null! Referanslar ayarlanmamış.");
            return;
        }

        // Featured News (Günün Manşeti)
        if (featuredNewsDisplay != null && sortedNews.Count > 0)
        {
            // En yeni haberi manşete koy
            var featuredNews = sortedNews[0];
            featuredNewsDisplay.Setup(featuredNews, OnNewsItemClicked);
            
            // Listeden çıkar (opsiyonel, ama manşetteki haber listede de olsun mu?)
            // sortedNews.RemoveAt(0); 
        }
        
        foreach (var news in sortedNews)
        {
            // Manşetteki haberi listede tekrar gösterme (isteğe bağlı)
            if (featuredNewsDisplay != null && sortedNews.Count > 0 && news == sortedNews[0]) continue;

            Debug.Log($"[CareerNewsUI] Haber oluşturuluyor: {news.title}");
            try
            {
                CreateNewsItemUI(news);
                Debug.Log($"[CareerNewsUI] Haber başarıyla oluşturuldu: {news.title}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[CareerNewsUI] Haber oluşturulurken hata: {news.title} - {e.Message}");
            }
        }
        
        UpdateStatusText();
        
        Debug.Log($"[CareerNewsUI] {sortedNews.Count} haber yüklendi, UI'da {activeNewsItems.Count} item var");
    }
    
    /// <summary>
    /// Filtreleri uygula
    /// </summary>
    private List<NewsItem> ApplyFilters(List<NewsItem> news)
    {
        var filtered = news.AsEnumerable();
        
        // Tür filtresi (currentFilterType)
        if (currentFilterType.HasValue)
        {
            filtered = filtered.Where(n => n.type == currentFilterType.Value);
        }
        // Fallback: Dropdown (eğer currentFilterType null ise ve dropdown varsa)
        else if (newsTypeFilter != null && newsTypeFilter.value > 0)
        {
            var newsTypes = System.Enum.GetValues(typeof(NewsType));
            if (newsTypeFilter.value - 1 < newsTypes.Length)
            {
                var selectedType = (NewsType)newsTypes.GetValue(newsTypeFilter.value - 1);
                filtered = filtered.Where(n => n.type == selectedType);
            }
        }
        
        // Okunmamış filtresi
        if (showOnlyUnreadToggle != null && showOnlyUnreadToggle.isOn)
        {
            filtered = filtered.Where(n => !n.isRead);
        }
        
        return filtered.ToList();
    }
    
    /// <summary>
    /// Haber item UI oluştur (Prefab kullanarak)
    /// </summary>
    private void CreateNewsItemUI(NewsItem news)
    {
        Debug.Log($"[CareerNewsUI] CreateNewsItemUI çağrıldı: {news.title}");
        
        if (newsListParent == null)
        {
            Debug.LogError("[CareerNewsUI] newsListParent null! Haber item oluşturulamıyor.");
            return;
        }
        
        Debug.Log($"[CareerNewsUI] newsListParent bulundu: {newsListParent.name}");
        
        if (newsItemPrefab == null)
        {
            Debug.LogWarning("[CareerNewsUI] NewsItemPrefab null, runtime oluşturuluyor");
            CreateNewsItemRuntime(news);
            return;
        }
        
        Debug.Log("[CareerNewsUI] Prefab ile haber item oluşturuluyor...");
        
        GameObject itemObj = Instantiate(newsItemPrefab, newsListParent);
        NewsItemUI itemUI = itemObj.GetComponent<NewsItemUI>();
        
        if (itemUI != null)
        {
            itemUI.Setup(news, OnNewsItemClicked);
            activeNewsItems.Add(itemUI);
            Debug.Log($"[CareerNewsUI] Prefab ile haber oluşturuldu: {news.title}");
        }
        else
        {
            Debug.LogWarning("[CareerNewsUI] NewsItemPrefab'da NewsItemUI bileşeni bulunamadı, manuel setup yapılıyor");
            SetupNewsItemManually(itemObj, news);
        }
    }
    
    /// <summary>
    /// Prefab'ı manuel setup et
    /// </summary>
    private void SetupNewsItemManually(GameObject itemObj, NewsItem news)
    {
        Button button = itemObj.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnNewsItemClicked(news));
        }
        
        TextMeshProUGUI[] texts = itemObj.GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length > 0)
        {
            texts[0].text = news.title;
            texts[0].color = news.isRead ? Color.gray : Color.white;
        }
        if (texts.Length > 1)
        {
            texts[1].text = news.dateString;
        }
        if (texts.Length > 2)
        {
            texts[2].text = news.source;
        }
    }
    
    /// <summary>
    /// Runtime'da haber item oluştur (fallback)
    /// </summary>
    private void CreateNewsItemRuntime(NewsItem news)
    {
        Debug.Log($"[CareerNewsUI] Runtime haber oluşturuluyor: {news.title}");
        
        if (newsListParent == null)
        {
            Debug.LogError("[CareerNewsUI] newsListParent null! Runtime haber oluşturulamıyor.");
            return;
        }
        
        GameObject itemObj = new GameObject($"NewsItem_{news.title}");
        itemObj.transform.SetParent(newsListParent, false); // worldPositionStays = false
        
        RectTransform itemRect = itemObj.AddComponent<RectTransform>();
        // VerticalLayoutGroup için basit ayarlar
        itemRect.anchorMin = Vector2.zero;
        itemRect.anchorMax = Vector2.one;
        itemRect.pivot = new Vector2(0.5f, 0.5f);
        itemRect.anchoredPosition = Vector2.zero;
        itemRect.sizeDelta = Vector2.zero; // Layout Group width'i kontrol edecek
        
        Image bg = itemObj.AddComponent<Image>();
        bg.color = new Color(0.15f, 0.15f, 0.25f, 0.9f);
        
        Button button = itemObj.AddComponent<Button>();
        button.onClick.AddListener(() => OnNewsItemClicked(news));
        
        // LayoutElement ekle (VerticalLayoutGroup için) - Bu çok önemli!
        LayoutElement layoutElement = itemObj.AddComponent<LayoutElement>();
        layoutElement.minHeight = 80;
        layoutElement.preferredHeight = 80;
        layoutElement.flexibleHeight = 0;
        layoutElement.minWidth = -1; // Layout Group'un width kontrolünü bırak
        layoutElement.preferredWidth = -1;
        layoutElement.flexibleWidth = 1;
        
        // Başlık
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(itemObj.transform, false);
        
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
        dateObj.transform.SetParent(itemObj.transform, false);
        
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
        
        Debug.Log($"[CareerNewsUI] Runtime haber oluşturuldu: {news.title}, Parent: {newsListParent.name}");
        Debug.Log($"[CareerNewsUI] Item pozisyon: {itemRect.anchoredPosition}, Boyut: {itemRect.sizeDelta}");
        Debug.Log($"[CareerNewsUI] Item rect: {itemRect.rect}");
        Debug.Log($"[CareerNewsUI] Parent child count: {newsListParent.childCount}");
        
        // Layout'u zorla güncelle
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(newsListParent.GetComponent<RectTransform>());
        
        Debug.Log($"[CareerNewsUI] Layout rebuild sonrası - Item boyut: {itemRect.sizeDelta}, rect: {itemRect.rect}");
    }
    
    /// <summary>
    /// Haber item'ına tıklandığında
    /// </summary>
    private void OnNewsItemClicked(NewsItem news)
    {
        Debug.Log($"[CareerNewsUI] Haber tıklandı: {news.title}");
        
        // Haberi okundu olarak işaretle
        news.isRead = true;
        
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
            Debug.LogWarning("[CareerNewsUI] NewsDetailPrefab null!");
            return;
        }
        
        // Mevcut detay paneli varsa kapat
        if (currentDetailPanel != null)
        {
            Destroy(currentDetailPanel.gameObject);
        }
        
        // Yeni detay paneli oluştur (Canvas'ın en üstünde)
        Canvas rootCanvas = FindObjectOfType<Canvas>();
        if (rootCanvas == null)
        {
            Debug.LogError("[CareerNewsUI] Root Canvas bulunamadı!");
            return;
        }
        
        GameObject detailObj = Instantiate(newsDetailPrefab, rootCanvas.transform);
        currentDetailPanel = detailObj.GetComponent<NewsDetailUI>();
        
        if (currentDetailPanel != null)
        {
            currentDetailPanel.ShowDetail(news, OnDetailClosed);
        }
        else
        {
            Debug.LogWarning("[CareerNewsUI] NewsDetailPrefab'da NewsDetailUI bileşeni bulunamadı!");
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
        
        // Eğer newsListParent'ta başka child'lar varsa onları da temizle
        if (newsListParent != null)
        {
            for (int i = newsListParent.childCount - 1; i >= 0; i--)
            {
                Transform child = newsListParent.GetChild(i);
                if (child != null)
                    Destroy(child.gameObject);
            }
        }
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
    /// Haberleri yenile (yeniden yükle)
    /// </summary>
    public void RefreshNews()
    {
        Debug.Log("[CareerNewsUI] Haberler yenileniyor...");
        LoadNews();
    }
    
    /// <summary>
    /// Test haberi oluştur
    /// </summary>
    public void GenerateTestNews()
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
            "👂 Sıcak Söylenti"
        };
        
        string[] contents = {
            "Takımımız muhteşem bir performans sergileyerek rakibini mağlup etti.",
            "Lig tablosunda liderliğimizi sürdürüyoruz. Şampiyonluk hedefimize yaklaşıyoruz.",
            "Yeni transferimiz takımımızı güçlendirecek. Kulüp yönetimi başarılı bir hamle yaptı.",
            "Oyuncumuz maçta sakatlık yaşadı. Doktorlar tedavi sürecini başlattı.",
            "Bu sezonki performansımız tüm rekorları kırıyor. İstatistikler etkileyici.",
            "Teknik direktörümüz gelecek planları hakkında önemli açıklamalarda bulundu.",
            "Takımımız sezonun en başarılı takımı ödülünü kazandı.",
            "Avrupa kulüplerinden transfer teklifleri gelmeye devam ediyor."
        };
        
        var newsTypes = new[] { 
            NewsType.Match, NewsType.League, NewsType.Transfer, 
            NewsType.Performance, NewsType.Injury, NewsType.TeamManagement,
            NewsType.Achievement, NewsType.Rumour
        };
        
        int index = Random.Range(0, titles.Length);
        
        var news = new NewsItem
        {
            title = titles[index],
            content = contents[index],
            type = newsTypes[index],
            source = "Career Test Sistemi",
            date = System.DateTime.Now,
            isRead = false
        };
        news.dateString = news.date.ToString("dd.MM.yyyy HH:mm");
        
        currentSave.mediaData.AddNews(news);
        
        // Yeni haberi UI'a ekle
        CreateNewsItemUI(news);
        UpdateStatusText();
        
        Debug.Log($"[CareerNewsUI] Test haberi oluşturuldu: {news.title}");
    }
    
    /// <summary>
    /// Hızlı test - 5 haber birden oluştur
    /// </summary>
    public void GenerateMultipleTestNews()
    {
        for (int i = 0; i < 5; i++)
        {
            GenerateTestNews();
        }
        
        Debug.Log("[CareerNewsUI] 5 test haberi oluşturuldu!");
    }
    
    /// <summary>
    /// Durum metnini güncelle
    /// </summary>
    private void UpdateStatusText()
    {
        if (statusText == null || currentSave?.mediaData?.recentNews == null) return;
        
        int totalNews = currentSave.mediaData.recentNews.Count;
        int unreadNews = currentSave.mediaData.recentNews.Count(n => !n.isRead);
        int displayedNews = activeNewsItems.Count;
        
        statusText.text = $"📰 Toplam: {totalNews} | 📬 Okunmamış: {unreadNews} | 👁️ Gösterilen: {displayedNews}";
    }
    
    /// <summary>
    /// Haber türü filtresi değiştiğinde
    /// </summary>
    private void OnNewsTypeFilterChanged(int value)
    {
        Debug.Log($"[CareerNewsUI] Haber türü filtresi değişti: {value}");
        LoadNews();
    }

    /// <summary>
    /// String ile filtreleme (Butonlar için)
    /// </summary>
    public void FilterByString(string category)
    {
        Debug.Log($"[CareerNewsUI] Filtre seçildi: {category}");
        
        switch (category)
        {
            case "Tümü": 
                currentFilterType = null; 
                break;
            case "Transfer": 
                currentFilterType = NewsType.Transfer; 
                break;
            case "Lig": 
                currentFilterType = NewsType.League; 
                break;
            case "Kulüp": 
                currentFilterType = NewsType.TeamManagement; 
                break;
            default: 
                currentFilterType = null; 
                break;
        }
        
        LoadNews();
    }
    
    /// <summary>
    /// Okunmamış filtresi değiştiğinde
    /// </summary>
    private void OnUnreadFilterChanged(bool showOnlyUnread)
    {
        Debug.Log($"[CareerNewsUI] Okunmamış filtresi: {showOnlyUnread}");
        LoadNews();
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
        
        Debug.Log("[CareerNewsUI] Tüm haberler temizlendi");
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