using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// Haber item UI bileşeni - Prefab için
/// </summary>
public class NewsItemUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI sourceText;
    public Image backgroundImage;
    public Image typeIcon;
    public Button itemButton;
    
    [Header("Visual Settings")]
    public Color readColor = Color.gray;
    public Color unreadColor = Color.white;
    public Color readBgColor = new Color(0.1f, 0.1f, 0.15f, 0.8f);
    public Color unreadBgColor = new Color(0.15f, 0.15f, 0.25f, 0.9f);
    
    private NewsItem currentNews;
    private Action<NewsItem> onClickCallback;
    
    /// <summary>
    /// Haber item'ını setup et
    /// </summary>
    public void Setup(NewsItem news, Action<NewsItem> onClickCallback)
    {
        this.currentNews = news;
        this.onClickCallback = onClickCallback;
        
        UpdateUI();
        SetupButton();
    }
    
    /// <summary>
    /// UI'ı güncelle
    /// </summary>
    private void UpdateUI()
    {
        if (currentNews == null) return;
        
        // Başlık
        if (titleText != null)
        {
            titleText.text = currentNews.title;
            titleText.color = currentNews.isRead ? readColor : unreadColor;
        }
        
        // Tarih
        if (dateText != null)
        {
            dateText.text = currentNews.dateString;
        }
        
        // Kaynak
        if (sourceText != null)
        {
            sourceText.text = currentNews.source;
        }
        
        // Arka plan rengi
        if (backgroundImage != null)
        {
            backgroundImage.color = currentNews.isRead ? readBgColor : unreadBgColor;
        }
        
        // Tür ikonu (eğer varsa)
        if (typeIcon != null)
        {
            // İkon sprite'ını ayarla (şimdilik renk ile gösterelim)
            typeIcon.color = GetNewsTypeColor(currentNews.type);
        }
    }
    
    /// <summary>
    /// Button setup
    /// </summary>
    private void SetupButton()
    {
        if (itemButton != null)
        {
            itemButton.onClick.RemoveAllListeners();
            itemButton.onClick.AddListener(OnItemClicked);
        }
    }
    
    /// <summary>
    /// Item'a tıklandığında
    /// </summary>
    private void OnItemClicked()
    {
        if (currentNews != null)
        {
            currentNews.isRead = true;
            UpdateUI();
            onClickCallback?.Invoke(currentNews);
        }
    }
    
    /// <summary>
    /// Haber türüne göre renk getir
    /// </summary>
    private Color GetNewsTypeColor(NewsType type)
    {
        return type switch
        {
            NewsType.Match => Color.green,
            NewsType.Transfer => Color.yellow,
            NewsType.Injury => Color.red,
            NewsType.Performance => Color.blue,
            NewsType.League => Color.cyan,
            NewsType.Contract => Color.magenta,
            NewsType.TeamManagement => Color.white,
            NewsType.Achievement => new Color(1f, 0.5f, 0f), // Orange
            NewsType.Rumour => Color.gray,
            _ => Color.white
        };
    }
    
    /// <summary>
    /// Haber verilerini güncelle (refresh için)
    /// </summary>
    public void RefreshData()
    {
        UpdateUI();
    }
}