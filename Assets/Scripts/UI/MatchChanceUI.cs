using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 2D pozisyon sahnesi UI - Zaman barı, pas zinciri göstergesi
/// </summary>
public class MatchChanceUI : MonoBehaviour
{
    [Header("Zaman Barı")]
    public Slider timeBarSlider;
    public Image timeBarFill;
    public TextMeshProUGUI timeBarText;

    [Header("Pas Zinciri")]
    public GameObject passChainPanel;
    public TextMeshProUGUI passChainText;
    public TextMeshProUGUI passChainBonusText;

    [Header("Pozisyon Bilgileri")]
    public TextMeshProUGUI minuteText;
    public TextMeshProUGUI positionTypeText;

    [Header("Spiker")]
    public TextMeshProUGUI commentatorText;

    private void Start()
    {
        SubscribeToEvents();
        InitializeUI();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    /// <summary>
    /// Event'lere abone ol
    /// </summary>
    private void SubscribeToEvents()
    {
        if (MatchChanceSceneManager.Instance != null)
        {
            MatchChanceSceneManager.Instance.OnTimeAmountChanged += UpdateTimeBar;
            MatchChanceSceneManager.Instance.OnPassChainChanged += UpdatePassChain;
            MatchChanceSceneManager.Instance.OnPositionFinished += OnPositionFinished;
        }
    }

    /// <summary>
    /// Event'lerden abonelikten çık
    /// </summary>
    private void UnsubscribeFromEvents()
    {
        if (MatchChanceSceneManager.Instance != null)
        {
            MatchChanceSceneManager.Instance.OnTimeAmountChanged -= UpdateTimeBar;
            MatchChanceSceneManager.Instance.OnPassChainChanged -= UpdatePassChain;
            MatchChanceSceneManager.Instance.OnPositionFinished -= OnPositionFinished;
        }
    }

    /// <summary>
    /// UI'ı başlat
    /// </summary>
    private void InitializeUI()
    {
        // Pozisyon bilgilerini göster
        MatchChanceData chance = MatchChanceManager.CurrentChance;
        if (chance != null)
        {
            if (minuteText != null)
                minuteText.text = $"{chance.minute}'";

            if (positionTypeText != null)
            {
                string typeText = GetPositionTypeText(chance.chanceType);
                positionTypeText.text = typeText;
            }
        }

        // Pas zinciri panelini gizle
        if (passChainPanel != null)
            passChainPanel.SetActive(false);
    }

    /// <summary>
    /// Zaman barını güncelle
    /// </summary>
    private void UpdateTimeBar(float normalizedTime)
    {
        if (timeBarSlider != null)
        {
            timeBarSlider.value = normalizedTime;
        }

        if (timeBarFill != null)
        {
            // Kırmızıdan yeşile geçiş (zaman azaldıkça kırmızı)
            timeBarFill.color = Color.Lerp(Color.red, Color.green, normalizedTime);
        }

        if (timeBarText != null)
        {
            float seconds = normalizedTime * MatchChanceSceneManager.Instance.maxTimeAmount;
            timeBarText.text = $"{seconds:F1}s";
        }
    }

    /// <summary>
    /// Pas zincirini güncelle
    /// </summary>
    private void UpdatePassChain(int chainCount)
    {
        if (passChainPanel != null)
        {
            passChainPanel.SetActive(chainCount > 0);
        }

        if (passChainText != null)
        {
            passChainText.text = $"Pas Zinciri: {chainCount}";
        }

        if (passChainBonusText != null)
        {
            string bonusText = GetPassChainBonusText(chainCount);
            passChainBonusText.text = bonusText;
        }
    }

    /// <summary>
    /// Pas zinciri bonus metnini al
    /// </summary>
    private string GetPassChainBonusText(int chainCount)
    {
        switch (chainCount)
        {
            case 0:
                return "";
            case 1:
                return "";
            case 2:
                return "Savunma reaksiyonu yavaşladı!";
            case 3:
                return "Şut isabeti artıyor!";
            case 4:
                return "Kaleci hata yapabilir!";
            case 5:
                return "ALTIN POZİSYON!";
            default:
                return "Mükemmel paslaşma!";
        }
    }

    /// <summary>
    /// Pozisyon türü metnini al
    /// </summary>
    private string GetPositionTypeText(MatchChanceType type)
    {
        switch (type)
        {
            case MatchChanceType.Shot:
                return "Şut Fırsatı";
            case MatchChanceType.Pass:
                return "Pas Fırsatı";
            case MatchChanceType.Dribble:
                return "Çalım Fırsatı";
            case MatchChanceType.Cross:
                return "Orta Fırsatı";
            case MatchChanceType.OneOnOne:
                return "1v1 Pozisyonu";
            default:
                return "Pozisyon";
        }
    }

    /// <summary>
    /// Spiker metnini güncelle
    /// </summary>
    public void UpdateCommentary(string text)
    {
        if (commentatorText != null)
            commentatorText.text = text;
    }

    /// <summary>
    /// Pozisyon bittiğinde
    /// </summary>
    private void OnPositionFinished(MatchChanceResult result)
    {
        // UI'ı temizle veya sonuç göster
        Debug.Log($"[MatchChanceUI] Position finished with result: {result}");
    }
}
