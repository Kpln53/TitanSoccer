using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Haberler UI - Haberler paneli
/// </summary>
public class NewsUI : MonoBehaviour
{
    [Header("Haber Listesi")]
    public Transform newsListParent;
    public GameObject newsItemPrefab;

    [Header("Haber Detay Paneli")]
    public GameObject detailPanel;
    public TextMeshProUGUI detailTitleText;
    public TextMeshProUGUI detailContentText;
    public TextMeshProUGUI detailDateText;
    public TextMeshProUGUI detailSourceText;
    public Button closeDetailButton;

    private List<NewsItem> currentNews;
    private NewsItem selectedNews;

    private void OnEnable()
    {
        LoadNews();
        if (detailPanel != null)
            detailPanel.SetActive(false);
    }

    /// <summary>
    /// Haberleri yükle ve göster
    /// </summary>
    private void LoadNews()
    {
        if (GameManager.Instance == null || !GameManager.Instance.HasCurrentSave())
        {
            Debug.LogWarning("[NewsUI] No current save!");
            return;
        }

        MediaData mediaData = GameManager.Instance.CurrentSave.mediaData;
        if (mediaData == null)
        {
            Debug.LogWarning("[NewsUI] No media data!");
            return;
        }

        if (NewsSystem.Instance != null)
        {
            currentNews = NewsSystem.Instance.GetRecentNews(mediaData, 20);
        }
        else
        {
            currentNews = mediaData.recentNews != null ? mediaData.recentNews.Take(20).ToList() : new List<NewsItem>();
        }

        DisplayNews();
    }

    /// <summary>
    /// Haberleri göster
    /// </summary>
    private void DisplayNews()
    {
        if (newsListParent == null)
        {
            Debug.LogWarning("[NewsUI] News list parent not found!");
            return;
        }

        // Mevcut item'ları temizle
        foreach (Transform child in newsListParent)
        {
            Destroy(child.gameObject);
        }

        // Her haber için item oluştur
        foreach (var news in currentNews)
        {
            CreateNewsItem(news);
        }
    }

    /// <summary>
    /// Haber item'ı oluştur
    /// </summary>
    private void CreateNewsItem(NewsItem news)
    {
        GameObject itemObj;

        if (newsItemPrefab != null)
        {
            itemObj = Instantiate(newsItemPrefab, newsListParent);
        }
        else
        {
            // Prefab yoksa runtime'da oluştur
            itemObj = new GameObject($"NewsItem_{news.title}");
            itemObj.transform.SetParent(newsListParent);

            RectTransform rect = itemObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(800, 120);

            Image bg = itemObj.AddComponent<Image>();
            bg.color = new Color(0.1f, 0.1f, 0.15f, 0.8f);

            Button button = itemObj.AddComponent<Button>();
            button.onClick.AddListener(() => OnNewsItemClicked(news));

            // Haber başlığı
            GameObject titleObj = new GameObject("NewsTitle");
            titleObj.transform.SetParent(itemObj.transform);
            RectTransform titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0.5f);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.offsetMin = new Vector2(10, 5);
            titleRect.offsetMax = new Vector2(-10, -5);

            TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = news.title;
            titleText.fontSize = 18;
            titleText.color = news.isRead ? Color.gray : Color.white;
            titleText.alignment = TextAlignmentOptions.TopLeft;
            titleText.fontStyle = FontStyles.Bold;

            // Haber tarihi
            GameObject dateObj = new GameObject("NewsDate");
            dateObj.transform.SetParent(itemObj.transform);
            RectTransform dateRect = dateObj.AddComponent<RectTransform>();
            dateRect.anchorMin = new Vector2(0, 0);
            dateRect.anchorMax = new Vector2(1, 0.5f);
            dateRect.offsetMin = new Vector2(10, 5);
            dateRect.offsetMax = new Vector2(-10, -5);

            TextMeshProUGUI dateText = dateObj.AddComponent<TextMeshProUGUI>();
            dateText.text = news.dateString;
            dateText.fontSize = 14;
            dateText.color = Color.gray;
            dateText.alignment = TextAlignmentOptions.BottomLeft;
        }

        // Prefab varsa bile tıklama event'i ekle
        Button itemButton = itemObj.GetComponent<Button>();
        if (itemButton != null)
        {
            itemButton.onClick.RemoveAllListeners();
            itemButton.onClick.AddListener(() => OnNewsItemClicked(news));
        }

        // Prefab içinde TextMeshProUGUI varsa güncelle
        TextMeshProUGUI[] texts = itemObj.GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length > 0)
        {
            texts[0].text = news.title;
            texts[0].color = news.isRead ? Color.gray : Color.white;
        }
    }

    /// <summary>
    /// Haber item'ına tıklandığında
    /// </summary>
    private void OnNewsItemClicked(NewsItem news)
    {
        selectedNews = news;

        // Haberi okundu olarak işaretle
        if (NewsSystem.Instance != null && GameManager.Instance != null && GameManager.Instance.HasCurrentSave())
        {
            NewsSystem.Instance.MarkNewsAsRead(GameManager.Instance.CurrentSave.mediaData, news);
        }
        else
        {
            news.isRead = true;
        }

        ShowNewsDetail(news);
    }

    /// <summary>
    /// Haber detayını göster
    /// </summary>
    private void ShowNewsDetail(NewsItem news)
    {
        if (detailPanel != null)
            detailPanel.SetActive(true);

        if (detailTitleText != null)
            detailTitleText.text = news.title;

        if (detailContentText != null)
            detailContentText.text = news.content;

        if (detailDateText != null)
            detailDateText.text = news.dateString;

        if (detailSourceText != null)
            detailSourceText.text = news.source;

        if (closeDetailButton != null)
            closeDetailButton.onClick.RemoveAllListeners();
            closeDetailButton.onClick.AddListener(OnCloseDetailButton);
    }

    private void OnCloseDetailButton()
    {
        if (detailPanel != null)
            detailPanel.SetActive(false);

        selectedNews = null;
        LoadNews(); // Haberleri yenile (okundu işaretleri için)
    }
}

