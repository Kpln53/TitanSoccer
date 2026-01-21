using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// Haber detay UI bileşeni - Prefab için
/// </summary>
public class NewsDetailUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI contentText;
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI sourceText;
    public TextMeshProUGUI typeText;
    public Image typeIcon;
    public Button closeButton;
    public Button backgroundButton;
    public GameObject contentPanel;
    
    [Header("Animation Settings")]
    public bool useAnimation = true;
    public float animationDuration = 0.3f;
    
    private NewsItem currentNews;
    private Action onCloseCallback;
    
    private void Awake()
    {
        SetupButtons();
        
        // Başlangıçta gizle
        gameObject.SetActive(false);
    }
    
    private void Update()
    {
        // ESC tuşu ile kapat
        if (Input.GetKeyDown(KeyCode.Escape) && gameObject.activeSelf)
        {
            CloseDetail();
        }
    }
    
    /// <summary>
    /// Detay panelini göster
    /// </summary>
    public void ShowDetail(NewsItem news, Action onCloseCallback = null)
    {
        this.currentNews = news;
        this.onCloseCallback = onCloseCallback;
        
        UpdateUI();
        
        gameObject.SetActive(true);
        
        if (useAnimation)
        {
            PlayOpenAnimation();
        }
        
        Debug.Log($"📋 Detay paneli açıldı: {news.title}");
    }
    
    /// <summary>
    /// Detay panelini kapat
    /// </summary>
    public void CloseDetail()
    {
        Debug.Log("🔄 CloseDetail çağrıldı");
        
        if (useAnimation)
        {
            PlayCloseAnimation(() => {
                gameObject.SetActive(false);
                onCloseCallback?.Invoke();
            });
        }
        else
        {
            gameObject.SetActive(false);
            onCloseCallback?.Invoke();
        }
        
        Debug.Log("❌ Detay paneli kapatıldı");
    }
    
    /// <summary>
    /// UI'ı güncelle
    /// </summary>
    private void UpdateUI()
    {
        if (currentNews == null) return;
        
        // Başlık
        if (titleText != null)
            titleText.text = currentNews.title;
            
        // İçerik
        if (contentText != null)
            contentText.text = currentNews.content;
            
        // Tarih
        if (dateText != null)
            dateText.text = $"📅 {currentNews.dateString}";
            
        // Kaynak
        if (sourceText != null)
            sourceText.text = $"📰 Kaynak: {currentNews.source}";
            
        // Tür
        if (typeText != null)
        {
            string typeIcon = GetNewsTypeIcon(currentNews.type);
            string typeName = GetNewsTypeName(currentNews.type);
            typeText.text = $"{typeIcon} {typeName}";
        }
        
        // Tür ikonu
        if (typeIcon != null)
        {
            typeIcon.color = GetNewsTypeColor(currentNews.type);
        }
    }
    
    /// <summary>
    /// Butonları setup et
    /// </summary>
    private void SetupButtons()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(CloseDetail);
        }
        
        if (backgroundButton != null)
        {
            backgroundButton.onClick.RemoveAllListeners();
            backgroundButton.onClick.AddListener(CloseDetail);
        }
    }
    
    /// <summary>
    /// Açılma animasyonu
    /// </summary>
    private void PlayOpenAnimation()
    {
        if (contentPanel != null)
        {
            contentPanel.transform.localScale = Vector3.zero;
            // Simple scale animation without LeanTween
            StartCoroutine(ScaleAnimation(contentPanel.transform, Vector3.zero, Vector3.one, animationDuration));
        }
    }
    
    /// <summary>
    /// Kapanma animasyonu
    /// </summary>
    private void PlayCloseAnimation(Action onComplete)
    {
        if (contentPanel != null)
        {
            // Simple scale animation without LeanTween
            StartCoroutine(ScaleAnimation(contentPanel.transform, Vector3.one, Vector3.zero, animationDuration, onComplete));
        }
        else
        {
            onComplete?.Invoke();
        }
    }
    
    /// <summary>
    /// Simple scale animation coroutine
    /// </summary>
    private System.Collections.IEnumerator ScaleAnimation(Transform target, Vector3 from, Vector3 to, float duration, Action onComplete = null)
    {
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Ease out back effect (simplified)
            t = 1f - Mathf.Pow(1f - t, 3f);
            
            target.localScale = Vector3.Lerp(from, to, t);
            yield return null;
        }
        
        target.localScale = to;
        onComplete?.Invoke();
    }
    
    /// <summary>
    /// Haber türü ikonu
    /// </summary>
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
    
    /// <summary>
    /// Haber türü adı
    /// </summary>
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
    
    /// <summary>
    /// Haber türü rengi
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
            NewsType.Achievement => new Color(1f, 0.5f, 0f),
            NewsType.Rumour => Color.gray,
            _ => Color.white
        };
    }
}