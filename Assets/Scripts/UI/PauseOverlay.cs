using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Oyun duraklatıldığında gri overlay gösterir
/// </summary>
public class PauseOverlay : MonoBehaviour
{
    [Header("UI Referansları")]
    [SerializeField] private Image overlayImage;
    [SerializeField] private Canvas overlayCanvas;

    [Header("Ayarlar")]
    [SerializeField] private Color overlayColor = new Color(0f, 0f, 0f, 0.5f); // Yarı saydam siyah (gri görünüm)
    [SerializeField] private float fadeSpeed = 5f;

    private bool isPaused = false;
    private float targetAlpha = 0f;

    void Awake()
    {
        // Canvas oluştur (eğer yoksa)
        if (overlayCanvas == null)
        {
            GameObject canvasObj = new GameObject("PauseOverlayCanvas");
            overlayCanvas = canvasObj.AddComponent<Canvas>();
            overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            overlayCanvas.sortingOrder = 1000; // En üstte
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // Image oluştur (eğer yoksa)
        if (overlayImage == null)
        {
            GameObject imageObj = new GameObject("OverlayImage");
            imageObj.transform.SetParent(overlayCanvas.transform, false);
            
            overlayImage = imageObj.AddComponent<Image>();
            overlayImage.color = new Color(overlayColor.r, overlayColor.g, overlayColor.b, 0f);
            
            // RectTransform'u tam ekran yap
            RectTransform rect = overlayImage.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
        }
    }

    void Update()
    {
        // Fade animasyonu
        Color currentColor = overlayImage.color;
        float currentAlpha = currentColor.a;
        float newAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.unscaledDeltaTime * fadeSpeed);
        overlayImage.color = new Color(overlayColor.r, overlayColor.g, overlayColor.b, newAlpha);
    }

    /// <summary>
    /// Oyunu duraklat ve overlay'i göster
    /// </summary>
    public void PauseGame()
    {
        if (isPaused) return;

        isPaused = true;
        targetAlpha = overlayColor.a;
        Time.timeScale = 0f; // Oyunu duraklat
        
        Debug.Log("Oyun duraklatıldı - Oyuncu hareket etmiyor");
    }

    /// <summary>
    /// Oyunu devam ettir ve overlay'i gizle
    /// </summary>
    public void ResumeGame()
    {
        if (!isPaused) return;

        isPaused = false;
        targetAlpha = 0f;
        Time.timeScale = 1f; // Oyunu devam ettir
        
        Debug.Log("Oyun devam ediyor - Oyuncu hareket ediyor");
    }

    /// <summary>
    /// Duraklatma durumunu kontrol et
    /// </summary>
    public bool IsPaused => isPaused;
}

